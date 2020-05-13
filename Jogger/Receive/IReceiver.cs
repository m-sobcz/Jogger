using Jogger.Drivers;
using Jogger.Valves;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Jogger.Receive.Receiver;

namespace Jogger.Receive
{
    public interface IReceiver
    {
        event ErrorsEventHandler ActiveErrorsChanged;
        event ErrorsEventHandler OccuredErrorsChanged;
        Valve parentValve { get; set; }
        List<string> ActiveErrorList { get; set; }
        bool HasAnyErrorCodeRead { get; }
        bool HasCriticalError { get; }
        bool HasReceivedAnyMessage { get; }
        List<string> OccuredErrorList { get; set; }

        void Initialize();
        Task<string> Receive();
        void SetDriver(IDriver driver);
    }
}