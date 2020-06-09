using Jogger.ValveTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jogger.Valves
{
    public class ValveStub : IValve
    {
        public bool executeStepDone;
        public bool receiveDone;
        public bool Stopped { get; set; }

        public bool QueryFinished { get; set; }
        public List<string> ActiveErrorList { get; set; }
        public string ActiveErrors { get; set; }
        public int ValveNumber { get; set; }

        public bool HasAnyErrorCodeRead { get; }

        public bool HasCriticalError { get; }

        public bool HasReceivedAnyMessage { get; }

        public bool IsDeflated { get; set; }
        public bool IsDone { get; set; }
        public bool IsInflated { get; set; }
        public bool IsStarted { get; set; }
        public bool isUntimelyDone { get; set; }
        public List<string> OccuredErrorList { get; set; }
        public string OccuredErrors { get; set; }
        public Result Result { get; set; }

        public event IValve.ErrorsEventHandler ActiveErrorsChanged;
        public event IValve.ErrorsEventHandler OccuredErrorsChanged;
        public event IValve.ResultEventHandler ResultChanged;

        public void CheckResult()
        {
            
        }

        public Task<string> ExecuteStep()
        {
            executeStepDone = true;
            return Task.FromResult<string>(nameof(ExecuteStep));
        }

        public Task<string> Receive()
        {
            receiveDone = true;
            return Task.FromResult<string>(nameof(Receive));
        }

        public void Start(IValveType valveType)
        {
            
        }

        public void Stop()
        {
            Stopped = true;
        }

        public void WakeUp()
        {
            throw new NotImplementedException();
        }
    }
}
