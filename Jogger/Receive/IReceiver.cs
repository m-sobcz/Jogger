using Jogger.Drivers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jogger.Receive
{
    public interface IReceiver
    {
        List<string> ActiveErrorList { get; set; }
        bool HasAnyErrorCodeRead { get; }
        bool HasCriticalError { get; }
        bool HasReceivedAnyMessage { get; }
        bool isNewActiveListAvailable { get; }
        bool isNewOccuredListAvailable { get; }
        List<string> OccuredErrorList { get; set; }

        void Initialize();
        Task<string> Receive();
        void SetDriver(IDriver driver);
    }
}