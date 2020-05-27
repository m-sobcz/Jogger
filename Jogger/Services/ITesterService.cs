using Jogger.Models;
using System;
using System.Threading.Tasks;

namespace Jogger.Services
{
    public interface ITesterService
    {
        ProgramState State { get; set; }
        string ValveType { get; set; }

        event TesterService.ProgramStateEventHandler ProgramStateChanged;

        Task CommunicationLoop();
        void Dispose();
        ActionStatus Initialize(ConfigurationSettings configurationSettings);
        ActionStatus Start(TestSettings testSettings);
        ActionStatus Stop();
    }
}