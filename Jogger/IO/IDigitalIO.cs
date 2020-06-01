
using Jogger.Services;
using System;
using System.Threading.Tasks;

namespace Jogger.IO
{
    public interface IDigitalIO
    {
        void Dispose();
        ActionStatus Initialize();
        Task<(string, byte[])> ReadInputs();
        Task<string> WriteOutputs(byte[] outpudData);
        event CommunicationLogEventHandler CommunicationLogChanged;
        delegate void CommunicationLogEventHandler(object sender, string log);
        event InputsReadEventHandler InputsRead;  
        delegate void InputsReadEventHandler(object sender, string errorCode, byte[] buffer);
    }
}