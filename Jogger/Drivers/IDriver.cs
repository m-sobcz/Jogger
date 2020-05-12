using Jogger.Services;
using System;

namespace Jogger.Drivers
{
    public interface IDriver
    {
        ulong[] MasterMask { get; set; }
        byte[] ReceivedData { get; set; }
        ulong SlaveMask { get; }

        event LinDriver.CommunicationLogEventHandler CommunicationLogChanged;
        event EventHandler InitializationFailed;

        void Close();
        ActionStatus Initialize();
        void OnInitializationFailed();
        string Receive();
        string Send();
        void SetSendData(byte[] data, byte id, byte dataLengthCode, ulong accessMask);
        bool WakeUp();
    }
}