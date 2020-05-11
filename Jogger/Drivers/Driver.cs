using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Drivers
{
    public class Driver : IDriver
    {
        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool Initialize()
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
