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
        event Action TestingFinished;
        event Action<string> CommunicationLogChanged;
        event ErrorsEventHandler ActiveErrorsChanged;
        event ErrorsEventHandler OccuredErrorsChanged;
        public delegate void ErrorsEventHandler(int channelNumber, string errors);
        event ResultEventHandler ResultChanged;
        public delegate void ResultEventHandler(int channelNumber, Result result);
    }
}