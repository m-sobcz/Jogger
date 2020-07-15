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
        public ActionStatus stopStatus { get; set; } = ActionStatus.OK;
        public TestSettings TestSettings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler TestingFinished;
        public event IValveManager.ErrorsEventHandler ActiveErrorsChanged;
        public event IValveManager.CommunicationLogEventHandler CommunicationLogChanged;
        public event IValveManager.ErrorsEventHandler OccuredErrorsChanged;
        public event IValveManager.ResultEventHandler ResultChanged;

        public ActionStatus Initialize(int channelsCount)
        {
            return initializationStatus;
        }

        public Task<bool> Receive()
        {
            return Task.FromResult<bool>(true);
        }

        public Task<bool> Send()
        {
            return Task.FromResult<bool>(true);
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
            return stopStatus;
        }

        public bool SetValveSensorsState(int valveNumber, bool isInflated, bool isDeflated)
        {
            throw new NotImplementedException();
        }

        public int GetNumberOfValves()
        {
            throw new NotImplementedException();
        }

        public void SetNextProcessedValve()
        {
            throw new NotImplementedException();
        }
    }
}
