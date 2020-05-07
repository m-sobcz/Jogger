using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jogger.Communication
{
    public class CommunicationStub : ICommunication
    {
        public bool IsTestingDone { get; set; }
        public ActionStatus InitializeActionStatus { get; set; }

        public void Dispose()
        {

        }

        public ActionStatus Initialize(int hardwareChannelCount)
        {
            return InitializeActionStatus;
        }


        public void InitializeReceivers()
        {
            throw new NotImplementedException();
        }

        public void OnActiveErrorsChanged(object sender, string errors, int channelNumber)
        {
            throw new NotImplementedException();
        }

        public void OnCommunicationLogChanged(object sender, string log)
        {
            throw new NotImplementedException();
        }

        public void OnOccuredErrorsChanged(object sender, string errors, int channelNumber)
        {
            throw new NotImplementedException();
        }

        public void OnResultChanged(object sender, Result result, int channelNumber)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadIO()
        {
            throw new NotImplementedException();
        }

        public Task ReceiveData()
        {
            throw new NotImplementedException();
        }

        public Task SendData()
        {
            throw new NotImplementedException();
        }

        public void SetNextProcessedChannel()
        {
            throw new NotImplementedException();
        }

        public void SetRepetitionsAndValveActivationTime()
        {
            throw new NotImplementedException();
        }

        public bool SetSequencerAndReceiver(string valveType)
        {
            throw new NotImplementedException();
        }

        public ActionStatus Start(TestSettings testSettings, string valveType)
        {
            throw new NotImplementedException();
        }

        public void StartSequencers()
        {
            throw new NotImplementedException();
        }

        public void StopAll()
        {
            throw new NotImplementedException();
        }

    }
}
