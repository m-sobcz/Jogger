using Jogger.Services;
using System;

namespace Jogger.Drivers
{
    public interface IDriver : IDisposable
    {
        byte[] ReceivedData { get; set; }
        event Action<string> CommunicationLogChanged;
        ActionStatus Initialize(int numberOfChannels);
        string Receive();
        string Send();
        void SetSendData(byte[] data, byte id, byte dataLengthCode, int channelNumber);
        bool WakeUp();
    }
}