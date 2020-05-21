using Jogger.Services;
using System;

namespace Jogger.Drivers
{
    public interface IDriver : IDisposable
    {
        public delegate void CommunicationLogEventHandler(object sender, string log);
        byte[] ReceivedData { get; set; }
        event CommunicationLogEventHandler CommunicationLogChanged;
        ActionStatus Initialize(int numberOfChannels);
        string Receive();
        string Send();
        void SetSendData(byte[] data, byte id, byte dataLengthCode, int channelNumber);
        bool WakeUp();
    }
}