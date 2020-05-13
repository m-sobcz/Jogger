using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Drivers
{
    public class DriverStub : IDriver
    {
        public ulong[] MasterMask { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public byte[] ReceivedData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ulong SlaveMask => throw new NotImplementedException();

        public ActionStatus initializeStatus { get; set; } = ActionStatus.OK;

        public event Driver.CommunicationLogEventHandler CommunicationLogChanged;
        public event EventHandler InitializationFailed;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public ActionStatus Initialize()
        {
            return initializeStatus;
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
            throw new NotImplementedException();
        }
    }
}
