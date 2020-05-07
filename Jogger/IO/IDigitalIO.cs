
using Jogger.Services;
using System.Threading.Tasks;

namespace Jogger.IO
{
    public interface IDigitalIO
    {
        void Dispose();
        ActionStatus Initialize();
        Task<(string, byte[])> ReadInputs();
    }
}