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

        public event LinDriver.CommunicationLogEventHandler CommunicationLogChanged;
        public event EventHandler InitializationFailed;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool Initialize()
        {
            throw new NotImplementedException();
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
    }
}
