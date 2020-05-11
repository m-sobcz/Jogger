using Jogger.Drivers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jogger.Valve
{
    public class Command
    {
        readonly static byte dataLengthCode = 8;
        readonly UInt64 accessMask;
        readonly byte id;
        readonly byte[] sendData = new byte[dataLengthCode];
        public Command(UInt64 accessMask, byte id, byte[] sendData = null)
        {
            for (int i = 0; i < (sendData?.Length ?? 0); i++)
            {
                this.sendData[i] = sendData[i];
            }
            this.id = id;
            this.accessMask = accessMask;
        }

        public async Task<string> SendDriverRequest(IDriver driver)
        {
            driver.SetSendData(sendData, id, dataLengthCode, accessMask);
            return await Task<string>.Run(() => driver.Send());
        }
    }
}
