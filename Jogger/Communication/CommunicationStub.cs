using Jogger.Services;
using Jogger.Valves;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jogger.Communication
{
    public class CommunicationStub : ICommunication
    {
        public bool IsTestingDone { get; set; }
        public ActionStatus initializeStatus { get; set; } = ActionStatus.OK;
        public ActionStatus startStatus { get; set; } = ActionStatus.OK;
        public bool StopAllExecuted { get; private set; }

        public void Dispose()
        {

        }

        public ActionStatus Initialize(int hardwareChannelCount)
        {
            return initializeStatus;
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
            return startStatus;
        }

        public void StartSequencers()
        {
            throw new NotImplementedException();
        }

        public void StopAll()
        {
            StopAllExecuted = true;
        }

    }
}
