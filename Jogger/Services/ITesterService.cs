using System;
using System.Threading.Tasks;

namespace Jogger.Services
{
    public interface ITesterService
    {
        ProgramState State { get; set; }
        string ValveType { get; set; }

        event TesterService.ErrorsEventHandler ActiveErrorsChanged;
        event TesterService.CommunicationLogEventHandler CommunicationLogChanged;
        event TesterService.DigitalIOEventHandler DigitalIOChanged;
        event TesterService.ErrorsEventHandler OccuredErrorsChanged;
        event TesterService.ProgramStateEventHandler ProgramStateChanged;
        event TesterService.ResultEventHandler ResultChanged;

        Task CommunicationLoop();
        void Dispose();
        ActionStatus Initialize(ConfigurationSettings configurationSettings);
        ActionStatus Start(Func<TestSettings, string, ActionStatus> startFunc, TestSettings testSettings);
        ActionStatus Stop(Action stopFunc);
    }
}