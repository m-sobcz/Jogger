using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Receive
{
    public class Receiver2Up : Receiver
    {
        public Receiver2Up() : base()
        {
            byte[] b = new byte[4];
            b[0] = 0x01;
            b[1] = 0x01;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Error in calibration CRC");
            b[1] = 0x03;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Error in EEPROM CRC");
            b[1] = 0x02;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Error in module ID CRC");
            b[1] = 0x04;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Watchdog triggered");
            b[1] = 0x05;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "EEPROM access error");
            b[0] = 0x02;
            b[1] = 0x01;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Supply voltage low");
            b[0] = 0x02;
            b[1] = 0x02;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Supply voltage high");
            b[0] = 0x02;
            b[1] = 0x03;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Supply voltage out of range");
            b[0] = 0x03;
            b[1] = 0x01;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Temperature low");
            b[0] = 0x03;
            b[1] = 0x02;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Temperature high");
            b[0] = 0x03;
            b[1] = 0x03;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Temperature out of range");
            b[0] = 0x04;
            b[1] = 0x01;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Pump is in error");
            b[0] = 0x05;
            b[1] = 0x01;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Pressure sensor out of range");
            b[0] = 0x08;
            b[1] = 0x01;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Valve 1 failure");
            b[0] = 0x08;
            b[1] = 0x02;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Valve 2 failure");
            b[0] = 0x08;
            b[1] = 0x03;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Valve 3 failure");
            b[0] = 0x08;
            b[1] = 0x04;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Valve 4 failure");
            b[0] = 0x09;
            b[1] = 0x01;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Input switch stuck");
            b[0] = 0x09;
            b[1] = 0x02;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Input switch out of range");
            b[0] = 0x09;
            b[1] = 0x03;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Input switch short to GND");
            b[0] = 0x09;
            b[1] = 0x04;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Input switch short to VCC");
            b[0] = 0x0B;
            b[1] = 0x01;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Ram validation failed");
            b[0] = 0x0D;
            b[1] = 0x01;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "Current out of range");
        }
    }
}
