using Jogger.Models;
using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jogger.Valves
{
   public class ValveManagerStub : IValveManager
    {
        public ActionStatus initializationStatus { get; set; } = ActionStatus.OK;
        public ActionStatus startStatus { get; set; } = ActionStatus.OK;
        public event EventHandler TestingFinished;
        public event IValveManager.ErrorsEventHandler ActiveErrorsChanged;
        public event IValveManager.CommunicationLogEventHandler CommunicationLogChanged;
        public event IValveManager.ErrorsEventHandler OccuredErrorsChanged;
        public event IValveManager.ResultEventHandler ResultChanged;

        public ActionStatus Initialize(int channelsCount)
        {
            return initializationStatus;
        }

        public Task Receive()
        {
            throw new NotImplementedException();
        }

        public Task Send()
        {
            throw new NotImplementedException();
        }

        public void SetTestSettings(TestSettings testSettings)
        {
            throw new NotImplementedException();
        }

        public ActionStatus Start(TestSettings testSettings, string valveType)
        {
            return startStatus;
        }


        public void OnTestingFinished() 
        {
            TestingFinished?.Invoke(this, EventArgs.Empty);
        }

        public ActionStatus Stop()
        {
            throw new NotImplementedException();
        }
    }
}
