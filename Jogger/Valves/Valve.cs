using Jogger.Drivers;
using Jogger.Models;
using Jogger.Services;
using Jogger.ValveTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Jogger.Valves.IValve;

namespace Jogger.Valves
{
    public class Valve : IValve
    {
        private readonly string alarmProcessingTxt = "...";
        private readonly string alarmInactiveTxt = "---";
        private readonly string messageReceiveTxt = "RX";
        private readonly byte ActiveErrorTypeByte = 0x14;
        private readonly byte OccuredErrorTypeByte = 0x15;
        private readonly Timer minStepTimer;
        private readonly Timer maxStepTimer;
        protected uint accessMask = 0;
        protected int Step { get; set; }
        public bool IsDone { get; set; } = false;
        public bool IsDeflated { get; set; } = false;
        public bool IsInflated { get; set; } = false;
        bool IsMinStepTimerDone { get; set; } = false;
        bool IsMaxStepTimerDone { get; set; } = false;

        private IValveManager valveManager;
        protected IDriver driver;
        private readonly IValveDecoder valveDecoder;
        private IValveType valveType;
        private int actualRepetition;
        public event ErrorsEventHandler ActiveErrorsChanged;
        public event ErrorsEventHandler OccuredErrorsChanged;
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
                ActiveErrorsChanged?.Invoke(this, errors, ValveNumber);
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
                OccuredErrorsChanged?.Invoke(this, errors, ValveNumber);
            }
        }
        bool isReadingActiveError;
        bool isReadingOccuredError;
        public bool HasAnyErrorCodeRead { get; private set; }
        public bool HasReceivedAnyMessage { get; private set; }

        static int valveCount = 0;
        public string ActiveErrors { get; set; }
        public string OccuredErrors { get; set; }
        public event ResultEventHandler ResultChanged;

        public int ValveNumber { get; set; }
        private Result result = Result.Idle;
        public bool canSetNextProcessedChannel = false;
        public Result Result
        {
            get => result;
            set
            {
                result = value;
                ResultChanged?.Invoke(this, result, ValveNumber);
            }
        }
        public Valve(IValveManager valveManager, IDriver driver, IValveDecoder valveDecoder)
        {
            this.valveManager = valveManager;
            this.driver = driver;
            this.valveDecoder = valveDecoder;
            minStepTimer = new Timer(new TimerCallback((o) => IsMinStepTimerDone = true), null, 0, Timeout.Infinite);
            maxStepTimer = new Timer(new TimerCallback((o) => IsMaxStepTimerDone = true), null, 0, Timeout.Infinite);
            ActiveErrors = alarmInactiveTxt;
            OccuredErrors = alarmInactiveTxt;
            ValveNumber = valveCount;
            valveCount++;
        }
        public void Start(IValveType valveType)
        {
            this.valveType = valveType;
            HasReceivedAnyMessage = false;
            HasAnyErrorCodeRead = false;
            Result = Result.Testing;
            ActiveErrorList.Clear();
            ActiveErrorList.Add(alarmProcessingTxt);
            OccuredErrorList.Clear();
            OccuredErrorList.Add(alarmProcessingTxt);
        }
        public async Task<string> Receive()
        {
            string dataFromDriver = await Task.Run(() => driver.Receive());
            HasReceivedAnyMessage |= dataFromDriver.Contains(messageReceiveTxt); ;
            ActiveErrorList = CheckErrorInData(ref isReadingActiveError, ActiveErrorList, ActiveErrorTypeByte);
            OccuredErrorList = CheckErrorInData(ref isReadingOccuredError, OccuredErrorList, OccuredErrorTypeByte);
            return dataFromDriver;
        }
        public void Stop()
        {
            Result = Result.Stopped;
            UntimelyFinish();
        }
        public async Task<string> ExecuteStep()
        {
            if (Result!= Result.Testing) return null;
            string message = await valveType.QueryList[Step].ExecuteStep(driver, ValveNumber);
            if (valveType.QueryList[Step].isDone)
            {
                ProcessStepDone();
            }
            return message;
        }
        void ProcessStepDone() 
        {
            {
                if (IsMaxStepTimerDone)
                {
                    UntimelyFinish();
                }
                if (CheckStandardProcessingDone())
                {
                    Step++;
                    if (Step >= valveType.QueryList.Count)
                    {
                        RepetitionDone();
                    }
                    else
                    {
                        SetupNewStepTimers();
                    }
                }
                else
                {
                    valveType.QueryList[Step].Restart();
                }
                canSetNextProcessedChannel = true;
            }
        }
        void SetupNewStepTimers() 
        {
            switch (valveType.QueryList[Step].queryType)
            {
                case QueryType.inflate:
                    minStepTimer.Change(valveManager.TestSettings.ValveMinInflateTime, Timeout.Infinite);
                    maxStepTimer.Change(valveManager.TestSettings.ValveMaxInflateTime, Timeout.Infinite);
                    break;
                case QueryType.deflate:
                    minStepTimer.Change(valveManager.TestSettings.ValveMinDeflateTime, Timeout.Infinite);
                    maxStepTimer.Change(valveManager.TestSettings.ValveMaxDeflateTime, Timeout.Infinite);
                    break;
                default:
                    minStepTimer.Change(0, Timeout.Infinite);
                    maxStepTimer.Change(0, Timeout.Infinite);
                    break;
            }
            IsMinStepTimerDone = false;
            IsMaxStepTimerDone = false;
        }
        bool CheckStandardProcessingDone() 
        {
            return IsMinStepTimerDone & ((valveType.QueryList[Step].queryType == QueryType.singleExecution) ||
                    (IsInflated && !IsDeflated && valveType.QueryList[Step].queryType == QueryType.inflate) ||
                    (IsDeflated && !IsInflated && valveType.QueryList[Step].queryType == QueryType.deflate));
        }
        protected List<string> CheckErrorInData(ref bool isReadingActive, List<string> list, byte errorType)
        {
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
            return list;
        }
        protected void AddError(List<string> list, byte data0, byte data1)
        {
            HasAnyErrorCodeRead = true;
            byte[] data = { data1, data0 };
            if (!(valveType.ErrorCodes.TryGetValue(BitConverter.ToInt16(data, 0), out string errorTxt)))
            {
                errorTxt = data[1].ToString("x2") + data[0].ToString("x2");
            }
            if (valveType.CriticalErrorCodes.TryGetValue(BitConverter.ToInt16(data, 0), out _))
            {
                Result = Result.DoneErrorCriticalCode;
            }
            list.Add(errorTxt);
        }
        protected void UntimelyFinish()
        {
            Result = HasReceivedAnyMessage ? Result.DoneErrorTimeout : Result.DoneErrorConnection;
            SuccessfullyEndTesting();
        }
        protected void RepetitionDone()
        {
            if (actualRepetition + 1 < valveManager.TestSettings.Repetitions)
            {
                actualRepetition++;
                Reset();
            }
            else
            {
                SuccessfullyEndTesting();
            }     
        }

        protected void SuccessfullyEndTesting()
        {
            Result = Result.DoneOk;
            actualRepetition = 0;
            Reset();
        }
        void Reset()
        {
            Step = 0;
            foreach (Query query in valveType.QueryList)
            {
                query.Restart();
            }
        }
    }
}
