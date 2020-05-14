using Jogger.Models;
using Jogger.Services;
using System.Threading.Tasks;

namespace Jogger.Valves
{
    public interface IValveManager
    {
        public delegate void ErrorsEventHandler(object sender, string errors, int channelNumber);
        public delegate void CommunicationLogEventHandler(object sender, string log);
        public delegate void ResultEventHandler(object sender, Result result, int channelNumber);
        bool IsTestingDone { get; set; }
        event ErrorsEventHandler ActiveErrorsChanged;
        event CommunicationLogEventHandler CommunicationLogChanged;
        event ErrorsEventHandler OccuredErrorsChanged;
        event ResultEventHandler ResultChanged;
        

        ActionStatus Initialize(int channelsCount);
        Task ReceiveData();
        Task SendData();
        void SetNextProcessedChannel();
        ActionStatus Start(TestSettings testSettings, string valveType);
        void Stop();
        void SetTestSettings(TestSettings testSettings);
    }
}