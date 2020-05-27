using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using static Jogger.Drivers.IDriver;

namespace Jogger.Drivers
{
    public class DriverStub : IDriver
    {
        public bool isDisposed = false;
        public ulong SlaveMask => throw new NotImplementedException();

        public ActionStatus initializationStatus { get; set; } = ActionStatus.OK;
        public byte[] ReceivedData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event CommunicationLogEventHandler CommunicationLogChanged;
        public event EventHandler InitializationFailed;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public ActionStatus Initialize()
        {
            return initializationStatus;
        }

        public void OnInitializationFailed()
        {
            throw new NotImplementedException();
        }

        public string Receive()
        {
            throw new NotImplementedException();
        }

        public string Send()
        {
            throw new NotImplementedException();
        }

        public void SetSendData(byte[] data, byte id, byte dataLengthCode, ulong accessMask)
        {
            throw new NotImplementedException();
        }

        public bool WakeUp()
        {
            throw new NotImplementedException();
        }

        public ActionStatus Initialize(int numberOfChannels)
        {
            return initializationStatus;
        }

        public void SetSendData(byte[] data, byte id, byte dataLengthCode, int channelNumber)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
           isDisposed = true;
    }
    }
}
