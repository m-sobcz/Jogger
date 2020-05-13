using Jogger.Models;
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
        ActionStatus Start(TestSettings testSettings);
        ActionStatus Stop();
    }
}