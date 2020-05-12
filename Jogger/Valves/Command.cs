using Jogger.Drivers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jogger.Valves
{
    public class Command
    {
        public readonly static byte dataLengthCode = 8;
        public readonly byte id;
        public readonly byte[] sendData = new byte[dataLengthCode];
        public Command(byte id, byte[] sendData = null)
        {
            for (int i = 0; i < (sendData?.Length ?? 0); i++)
            {
                this.sendData[i] = sendData[i];
            }
            this.id = id;
        }
    }
}
