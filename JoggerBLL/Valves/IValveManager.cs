using Jogger.Models;
using Jogger.Services;
using System;
using System.Threading.Tasks;

namespace Jogger.Valves
{
    public interface IValveManager
    {
        ActionStatus Initialize(int numberOfValves);
        ActionStatus Start(TestSettings testSettings, string valveType);
        ActionStatus Stop();
        Task<bool> Receive();
        void SetNextProcessedValve();
        Task<bool> Send();
        TestSettings TestSettings{get;set;}
        bool SetValveSensorsState(int valveNumber, bool isInflated, bool isDeflated);
        int GetNumberOfValves();
        event EventHandler TestingFinished;
        event CommunicationLogEventHandler CommunicationLogChanged;
        public delegate void CommunicationLogEventHandler(object sender, string log);
        event ErrorsEventHandler ActiveErrorsChanged;
        event ErrorsEventHandler OccuredErrorsChanged;
        public delegate void ErrorsEventHandler(object sender, string errors, int channelNumber);
        event ResultEventHandler ResultChanged;
        public delegate void ResultEventHandler(object sender, Result result, int channelNumber);
    }
}