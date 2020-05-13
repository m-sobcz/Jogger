
using Jogger.Services;
using System;
using System.Threading.Tasks;

namespace Jogger.IO
{
    public interface IDigitalIO
    {
        event InputsReadEventHandler InputsRead;
        delegate void InputsReadEventHandler(object sender, string errorCode, byte[] buffer);
        void Dispose();
        ActionStatus Initialize();
        Task<(string, byte[])> ReadInputs();

    }
}