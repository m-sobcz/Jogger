using Jogger.Services;
using System.Threading.Tasks;

namespace Jogger.Communication
{
    public interface ICommunication
    {
        bool IsTestingDone { get; set; }

       // event LinCommunication.ErrorsEventHandler ActiveErrorsChanged;
       // event LinCommunication.CommunicationLogEventHandler CommunicationLogChanged;
       // event LinCommunication.ErrorsEventHandler OccuredErrorsChanged;
       // event LinCommunication.ResultEventHandler ResultChanged;

        //Result CheckResult();
        void Dispose();
        //BitArray GetDigitalInputs();
        ActionStatus Initialize(int hardwareChannelCount);
        void InitializeReceivers();
        void OnActiveErrorsChanged(object sender, string errors, int channelNumber);
        void OnCommunicationLogChanged(object sender, string log);
        void OnOccuredErrorsChanged(object sender, string errors, int channelNumber);
        void OnResultChanged(object sender, Result result, int channelNumber);
        Task<byte[]> ReadIO();
        Task ReceiveData();
        Task SendData();
        void SetNextProcessedChannel();
        void SetRepetitionsAndValveActivationTime();
        bool SetSequencerAndReceiver(string valveType);
        ActionStatus Start(TestSettings testSettings, string valveType);
        void StartSequencers();
        void StopAll();
    }
}