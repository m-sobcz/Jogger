using Jogger.Models;
using Jogger.Receive;
using Jogger.Services;
using System.Threading.Tasks;

namespace Jogger.Valves
{
    public interface IValveManager
    {
        bool IsTestingDone { get; set; }

        event Receiver.ErrorsEventHandler ActiveErrorsChanged;
        event ValveManager.CommunicationLogEventHandler CommunicationLogChanged;
        event Receiver.ErrorsEventHandler OccuredErrorsChanged;
        event ValveManager.ResultEventHandler ResultChanged;

        ActionStatus Initialize(int channelsCount);
        Task ReceiveData();
        Task SendData();
        void SetNextProcessedChannel();
        ActionStatus Start(TestSettings testSettings, string valveType);
        void Stop();
    }
}