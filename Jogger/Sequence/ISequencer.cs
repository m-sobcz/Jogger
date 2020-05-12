using Jogger.Drivers;
using System.Threading.Tasks;

namespace Jogger.Sequence
{
    public interface ISequencer
    {
        bool IsDeflated { get; set; }
        bool IsDone { get; set; }
        bool IsInflated { get; set; }
        bool IsStarted { get; set; }
        bool IsStopRequested { get; set; }
        bool isUntimelyDone { get; set; }
        int Repetitions { get; set; }
        int valveMaxDeflateTime { get; set; }
        int valveMaxInflateTime { get; set; }
        int valveMinDeflateTime { get; set; }
        int valveMinInflateTime { get; set; }

        void AddAllTestQueries();
        Task<string> ExecuteStep(ulong accessMask);
        void Start();
        void WakeUp();
    }
}