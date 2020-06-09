using Jogger.ValveTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jogger.Valves
{
    public interface IValve
    {
        public delegate void ResultEventHandler(object sender, Result result, int channelNumber);
        public delegate void ErrorsEventHandler(object sender, string errors, int channelNumber);
        List<string> ActiveErrorList { get; set; }
        string ActiveErrors { get; set; }
        int ValveNumber { get; set; }
        bool HasAnyErrorCodeRead { get; }
        bool HasReceivedAnyMessage { get; }
        bool IsDeflated { get; set; }
        bool IsDone { get; set; }
        bool IsInflated { get; set; }
        void Stop();
        List<string> OccuredErrorList { get; set; }
        string OccuredErrors { get; set; }
        Result Result { get; set; }

        event ErrorsEventHandler ActiveErrorsChanged;
        event ErrorsEventHandler OccuredErrorsChanged;
        event ResultEventHandler ResultChanged;
        Task<string> ExecuteStep();
        Task<string> Receive();
        void Start(IValveType valveType);
    }
}