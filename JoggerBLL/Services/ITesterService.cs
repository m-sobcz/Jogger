using Jogger.Models;
using System;
using System.Threading.Tasks;

namespace Jogger.Services
{
    public interface ITesterService
    {
        ProgramState State { get; set; }
        string ValveType { get; set; }

        event Action<ProgramState> ProgramStateChanged;

        Task CommunicationLoop();
        void Dispose();
        ActionStatus Initialize(int hardwareChannelCount=4);
        ActionStatus Start(TestSettings testSettings, string valveType);
        ActionStatus Stop();
    }
}