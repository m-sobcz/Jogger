using Jogger.Drivers;
using Jogger.Models;
using Jogger.Receive;
using Jogger.Sequence;
using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Jogger.Valves
{
    public class Valve
    {
        public static TestSettings testSettings;
        private readonly Timer minStepTimer;
        private readonly Timer maxStepTimer;
        protected UInt64 accessMask = 0;
        public bool queryFinished;
        protected int Step { get; set; }
        public bool IsStarted { get; set; }
        public bool IsDone { get; set; } = false;
        public bool IsDeflated { get; set; } = false;
        public bool IsInflated { get; set; } = false;
        public bool isUntimelyDone { get; set; } = false;
        bool IsMinStepTimerDone { get; set; } = false;
        bool IsMaxStepTimerDone { get; set; } = false;
        protected IDriver driver;
        List<Query> Queries = new List<Query>();
        private int actualRepetition;
        public bool IsStopRequested { get; set; }
        public event ErrorsEventHandler ActiveErrorsChanged;
        public event ErrorsEventHandler OccuredErrorsChanged;
        public delegate void ErrorsEventHandler(object sender, string errors, int channelNumber);
        private List<string> activeErrorList = new List<string>();
        private List<string> occuredErrorList = new List<string>();
        public List<string> ActiveErrorList
        {
            get { return activeErrorList; }
            set
            {
                activeErrorList = value;
                string errors = "";
                foreach (string s in activeErrorList)
                {
                    errors += s + "\n";
                }
                ActiveErrorsChanged?.Invoke(this, errors, ChannelNumber);
            }
        }
        public List<string> OccuredErrorList
        {
            get { return occuredErrorList; }
            set
            {
                occuredErrorList = value;
                string errors = "";
                foreach (string s in occuredErrorList)
                {
                    errors += s + "\n";
                }
                OccuredErrorsChanged?.Invoke(this, errors, ChannelNumber);
            }
        }
        bool isReadingActiveError;
        bool isReadingOccuredError;
        public bool HasCriticalError { get; private set; }
        public bool HasAnyErrorCodeRead { get; private set; }
        public bool HasReceivedAnyMessage { get; private set; }

        protected Dictionary<Int16, string> errorCodes = new Dictionary<Int16, string>();
        static int count = 0;
        public string ActiveErrors { get; set; } = "---";
        public string OccuredErrors { get; set; } = "---";
        public event ResultEventHandler ResultChanged;
        public delegate void ResultEventHandler(object sender, Result result, int channelNumber);
        public int ChannelNumber { get; set; }
        private Result result = Result.Idle;
        public Result Result
        {
            get => result;
            set
            {
                result = value;
                ResultChanged?.Invoke(this, result, ChannelNumber);
            }
        }
        public Valve(IDriver driver)
        {
            this.driver = driver;
            minStepTimer = new Timer(new TimerCallback((o) => IsMinStepTimerDone = true), null, 0, Timeout.Infinite);
            maxStepTimer = new Timer(new TimerCallback((o) => IsMaxStepTimerDone = true), null, 0, Timeout.Infinite);
            byte[] b = new byte[4];
            b[0] = 0x00;
            b[1] = 0x00;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "<no error>");
            ChannelNumber = count;
            count++;
        }
        public async Task<string> Receive()
        {
            string dataFromDriver = await Task<string>.Run(() => driver.Receive());
            HasReceivedAnyMessage = HasReceivedAnyMessage | dataFromDriver.Contains("RX"); ;
            CheckErrorInData(ref isReadingActiveError, ActiveErrorList, 0x14);
            CheckErrorInData(ref isReadingOccuredError, OccuredErrorList, 0x15);
            return dataFromDriver;
        }

        protected bool CheckErrorInData(ref bool isReadingActive, List<string> list, byte errorType)
        {
            bool newErrorListAvailable = false;
            if (isReadingActive & (driver.ReceivedData[1] == 0x20 | driver.ReceivedData[1] == 0x21 |
                driver.ReceivedData[1] == 0x22 | driver.ReceivedData[1] == 0x23))
            {
                if (!(driver.ReceivedData[1] == 0x23))
                {
                    for (int i = 2; i < 7; i += 2)
                    {
                        AddError(list, driver.ReceivedData[i], driver.ReceivedData[i + 1]);
                    }
                }
                else
                {
                    AddError(list, driver.ReceivedData[2], driver.ReceivedData[3]);
                }
            }
            else
            {
                if (isReadingActive)
                {
                    newErrorListAvailable = true;
                    isReadingActive = false;
                }
            }
            if (driver.ReceivedData[0] == 0x90 & driver.ReceivedData[1] == 0x10 & driver.ReceivedData[2] == 0x14 & driver.ReceivedData[3] == 0x62 &
                driver.ReceivedData[4] == 0xFD & driver.ReceivedData[5] == errorType)
            {
                list.Clear();
                AddError(list, driver.ReceivedData[6], driver.ReceivedData[7]);
                isReadingActive = true;
            }
            return newErrorListAvailable;
        }

        protected void AddError(List<string> list, byte data0, byte data1)
        {
            HasAnyErrorCodeRead = true;
            byte[] b = { data0, data1 };
            string s = "???";
            if (!(errorCodes.TryGetValue(BitConverter.ToInt16(b, 0), out s)))
            {
                s = b[0].ToString() + b[1].ToString();
            }

            if (s.Contains("Valve"))
            {
                HasCriticalError = true;
            }
            list.Add(s);
        }


        public async Task<string> ExecuteStep(ulong accessMask)
        {
            string message = await Queries[Step].ExecuteStep(driver, accessMask);
            if (IsStopRequested)
            {
                UntimelyFinish();
            }
            if (Queries[Step].isDone)
            {
                {
                    bool isStandardProcessingFinished = (Queries[Step].queryType == QueryType.singleExecution) ||
                        (IsInflated && !IsDeflated && Queries[Step].queryType == QueryType.inflate) ||
                        (IsDeflated && !IsInflated && Queries[Step].queryType == QueryType.deflate);
                    Trace.WriteLine($"QueryType {Queries[Step].queryType },Repetition {actualRepetition},Step {Step}, IsInflated {IsInflated}, IsDeflated {IsDeflated}");
                    if (IsMaxStepTimerDone | (IsMinStepTimerDone & (isStandardProcessingFinished)))
                    {
                        Step++;
                        if (!isStandardProcessingFinished)
                        {
                            UntimelyFinish();
                        }
                        if (Step >= Queries.Count)
                        {
                            RepetitionDone();
                        }
                        else
                        {
                            switch (Queries[Step].queryType)
                            {
                                case QueryType.inflate:
                                    minStepTimer.Change(valveMinInflateTime, Timeout.Infinite);
                                    maxStepTimer.Change(valveMaxInflateTime, Timeout.Infinite);
                                    break;
                                case QueryType.deflate:
                                    minStepTimer.Change(valveMinDeflateTime, Timeout.Infinite);
                                    maxStepTimer.Change(valveMaxDeflateTime, Timeout.Infinite);
                                    break;
                                default:
                                    minStepTimer.Change(0, Timeout.Infinite);
                                    maxStepTimer.Change(0, Timeout.Infinite);
                                    break;
                            }
                            IsMinStepTimerDone = false;
                            IsMaxStepTimerDone = false;
                        }
                    }
                    else
                    {
                        Queries[Step].Restart();
                    }
                }
            }
            queryFinished = Queries[Step].isDone;
            return (message);
        }

        public void WakeUp()
        {
            driver.WakeUp();
        }
        protected void UntimelyFinish()
        {
            isUntimelyDone = true;
            RepetitionDone(false);
        }
        protected void RepetitionDone(bool canContinue = true)
        {
            if ((actualRepetition + 1 < testSettings.Repetitions) & canContinue)
            {
                actualRepetition++;
            }
            else
            {
                IsDone = true;
                IsStarted = false;
                actualRepetition = 0;
            }
            Step = 0;
            foreach (Query query in Queries)
            {
                query.Restart();
            }
        }
        public void Start()
        {
            HasReceivedAnyMessage = false;
            HasAnyErrorCodeRead = false;
            HasCriticalError = false;
            IsStopRequested = false;
            isUntimelyDone = false;
            IsStarted = true;
            Result = Result.Testing;
            ActiveErrors = "...";
            OccuredErrors = "...";
        }
    }
}
