using Jogger.Models;
using Jogger.Services;
using System.Threading.Tasks;

namespace Jogger.Valves
{
    public interface IValveManager
    {
        bool IsTestingDone { get; set; }

        ActionStatus Initialize(int channelsCount);
        Task ReceiveData();
        Task SendData();
        void SetNextProcessedChannel();
        ActionStatus Start(TestSettings testSettings, string valveType);
        void Stop();
    }
}