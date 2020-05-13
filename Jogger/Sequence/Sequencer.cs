using Jogger.Drivers;
using Jogger.Valves;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jogger.Sequence

{
    public class Sequencer : ISequencer
    {
        public Valve parentValve { get; set; }
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

        public int Repetitions { get; set; } = 1;
        public int valveMinInflateTime { get; set; } = 0;
        public int valveMinDeflateTime { get; set; } = 0;
        public int valveMaxInflateTime { get; set; } = 0;
        public int valveMaxDeflateTime { get; set; } = 0;

        public Sequencer(IDriver driver)
        {
            this.driver = driver;
            minStepTimer = new Timer(new TimerCallback((o) => IsMinStepTimerDone = true), null, 0, Timeout.Infinite);
            maxStepTimer = new Timer(new TimerCallback((o) => IsMaxStepTimerDone = true), null, 0, Timeout.Infinite);
        }

        public async Task<string> ExecuteStep(ulong accessMask)
        {
            string message = await Queries[Step].ExecuteStep(driver,accessMask);
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

        protected virtual void AddQuery(Query query)
        {
            Queries.Add(query);
        }

        public virtual void AddAllTestQueries()
        {
            AddQuery(GetTesterPresentQuery());
            AddQuery(GetTesterPresentQuery());
        }

        protected Query GetTesterPresentQuery()
        {
            Query query = new Query();
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x0, 0x3E, 0x0 }));
            query.AddCommand(new Command(0x3d));
            return query;
        }
        protected void UntimelyFinish()
        {
            isUntimelyDone = true;
            RepetitionDone(false);
        }
        protected void RepetitionDone(bool canContinue = true)
        {
            if ((actualRepetition + 1 < Repetitions) & canContinue)
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
            IsStopRequested = false;
            isUntimelyDone = false;
            IsStarted = true;
        }
    }
}
