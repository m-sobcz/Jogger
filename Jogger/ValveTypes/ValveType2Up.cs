using Jogger.Valves;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.ValveTypes
{
    public class ValveType2Up : ValveType
    {
        public ValveType2Up()
        {
            byte[] b = new byte[4];
            b[0] = 0x01;
            b[1] = 0x01;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Error in calibration CRC");
            b[1] = 0x03;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Error in EEPROM CRC");
            b[1] = 0x02;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Error in module ID CRC");
            b[1] = 0x04;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Watchdog triggered");
            b[1] = 0x05;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "EEPROM access error");
            b[0] = 0x02;
            b[1] = 0x01;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Supply voltage low");
            b[0] = 0x02;
            b[1] = 0x02;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Supply voltage high");
            b[0] = 0x02;
            b[1] = 0x03;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Supply voltage out of range");
            b[0] = 0x03;
            b[1] = 0x01;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Temperature low");
            b[0] = 0x03;
            b[1] = 0x02;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Temperature high");
            b[0] = 0x03;
            b[1] = 0x03;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Temperature out of range");
            b[0] = 0x04;
            b[1] = 0x01;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Pump is in error");
            //b[0] = 0x04;
            //b[1] = 0x09;
            //errorCodes.Add(BitConverter.ToInt16(b, 0), "TestLowError");//!
            b[0] = 0x05;
            b[1] = 0x01;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Pressure sensor out of range");
            b[0] = 0x08;
            b[1] = 0x01;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Valve 1 failure");
            b[0] = 0x08;
            b[1] = 0x02;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Valve 2 failure");
            b[0] = 0x08;
            b[1] = 0x03;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Valve 3 failure");
            b[0] = 0x08;
            b[1] = 0x04;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Valve 4 failure");
            b[0] = 0x09;
            b[1] = 0x01;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Input switch stuck");
            b[0] = 0x09;
            b[1] = 0x02;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Input switch out of range");
            b[0] = 0x09;
            b[1] = 0x03;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Input switch short to GND");
            b[0] = 0x09;
            b[1] = 0x04;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Input switch short to VCC");
            b[0] = 0x0B;
            b[1] = 0x01;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Ram validation failed");
            b[0] = 0x0D;
            b[1] = 0x01;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "Current out of range");

            QueryList.Add(GetTesterPresentQuery());
            QueryList.Add(GetTesterPresentQuery());
            QueryList.Add(GetActivateDebugQuery());
            QueryList.Add(GetEnablePumpQuery());
            QueryList.Add(GetValve1OnQuery());//Cell 1 inflate - lumbar top
            QueryList.Add(GetValve2OnQuery());//Cell 1 deflate - lumbar top
            QueryList.Add(GetValve3OnQuery());//Cell 2 inflate - lumbar bottom            
            QueryList.Add(GetValve4OnQuery());//Cell 2 deflate - lumbar bottom
            QueryList.Add(GetActiveErrors());
            QueryList.Add(GetOccuredErrors());
        }
        Query GetActiveErrors()
        {
            Query query = new Query();
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x03, 0x22, 0xFD, 0x14, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3d));
            query.AddCommand(new Command(0x3d));
            query.AddCommand(new Command(0x3d));
            query.AddCommand(new Command(0x3d));
            return query;
        }
        Query GetOccuredErrors()
        {
            Query query = new Query();
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x03, 0x22, 0xFD, 0x15, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3d));
            query.AddCommand(new Command(0x3d));
            query.AddCommand(new Command(0x3d));
            query.AddCommand(new Command(0x3d));
            return query;
        }
        Query GetActivateDebugQuery()
        {
            Query query = new Query();
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x04, 0x2E, 0xFD, 0x09, 0x01, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3d));
            return query;
        }
        Query GetEnablePumpQuery()
        {
            Query query = new Query();
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x10, 0x0C, 0x2E, 0xFD, 0x08, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x21, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x22, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3d));
            return query;
        }
        Query GetDisablePumpQuery()
        {
            Query query = new Query();
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x10, 0x0C, 0x2E, 0xFD, 0x08, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x21, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x22, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3d));
            return query;
        }
        Query GetValve1OnQuery()
        {
            Query query = new Query(QueryType.inflate);
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x10, 0x0C, 0x2E, 0xFD, 0x08, 0x01, 0x00 }));
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x21, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x22, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3d));
            return query;
        }
        Query GetValve2OnQuery()
        {
            Query query = new Query(QueryType.deflate);
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x10, 0x0C, 0x2E, 0xFD, 0x08, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x21, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 }));//query.AddCommand(new Command( 0x3c, new byte[] { 0x90, 0x21, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x22, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3d));
            return query;
        }
        Query GetValve3OnQuery()
        {
            Query query = new Query(QueryType.inflate);
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x10, 0x0C, 0x2E, 0xFD, 0x08, 0x02, 0x00 }));
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x21, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x22, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3d));
            return query;
        }
        Query GetValve4OnQuery()
        {
            Query query = new Query(QueryType.deflate);
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x10, 0x0C, 0x2E, 0xFD, 0x08, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x21, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x22, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3d));
            return query;
        }
        Query GetPressure()
        {
            Query query = new Query();
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x03, 0x22, 0xFD, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x3d));
            query.AddCommand(new Command(0x3d));
            query.AddCommand(new Command(0x3d));
            return query;
        }
        Query StandardCommandResponse()
        {
            Query query = new Query();
            query.AddCommand(new Command(0x00, new byte[] { 0x90, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x00, new byte[] { 0x90, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x00, new byte[] { 0x90, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x00, new byte[] { 0x90, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            query.AddCommand(new Command(0x01));
            query.AddCommand(new Command(0x01));
            query.AddCommand(new Command(0x01));
            query.AddCommand(new Command(0x01));
            return query;
        }
    }
}
