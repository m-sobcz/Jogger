using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jogger.Valves
{
    public interface IValve
    {
        bool QueryFinished { get; set; }
        List<string> ActiveErrorList { get; set; }
        string ActiveErrors { get; set; }
        int ChannelNumber { get; set; }
        bool HasAnyErrorCodeRead { get; }
        bool HasCriticalError { get; }
        bool HasReceivedAnyMessage { get; }
        bool IsDeflated { get; set; }
        bool IsDone { get; set; }
        bool IsInflated { get; set; }
        bool IsStarted { get; set; }
        bool IsStopRequested { get; set; }
        bool isUntimelyDone { get; set; }
        List<string> OccuredErrorList { get; set; }
        string OccuredErrors { get; set; }
        Result Result { get; set; }

        event Valve.ErrorsEventHandler ActiveErrorsChanged;
        event Valve.ErrorsEventHandler OccuredErrorsChanged;
        event Valve.ResultEventHandler ResultChanged;

        void CheckResult();
        Task<string> ExecuteStep();
        Task<string> Receive();
        void Start();
        void WakeUp();
    }
}