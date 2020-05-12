﻿using Jogger.Drivers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jogger.Receive
{
    public class Receiver : IReceiver
    {
        IDriver driver;
        public List<string> ActiveErrorList { get; set; } = new List<string>();
        public List<string> OccuredErrorList { get; set; } = new List<string>();
        public bool isNewActiveListAvailable { get; private set; }
        public bool isNewOccuredListAvailable { get; private set; }
        public bool HasCriticalError { get; private set; }
        public bool HasAnyErrorCodeRead { get; private set; }
        public bool HasReceivedAnyMessage { get; private set; }
        protected Dictionary<Int16, string> errorCodes = new Dictionary<Int16, string>();
        bool isReadingActiveError;
        bool isReadingOccuredError;
        public Receiver()
        {
            byte[] b = new byte[4];
            b[0] = 0x00;
            b[1] = 0x00;
            errorCodes.Add(BitConverter.ToInt16(b, 0), "<no error>");
        }
        public void SetDriver(IDriver driver)
        {
            this.driver = driver;
        }
        public async Task<string> Receive()
        {
            string dataFromDriver = await Task<string>.Run(() => driver.Receive());
            HasReceivedAnyMessage = HasReceivedAnyMessage | dataFromDriver.Contains("RX"); ;
            isNewActiveListAvailable = CheckErrorInData(ref isReadingActiveError, ActiveErrorList, 0x14);
            isNewOccuredListAvailable = CheckErrorInData(ref isReadingOccuredError, OccuredErrorList, 0x15);
            return dataFromDriver;
        }

        protected bool CheckErrorInData(ref bool isReadingActive, List<string> list, byte errorType)
        {
            bool newErrorListAvailable = false;
            if (isReadingActive & (driver.ReceivedData[1] == 0x20 | driver.ReceivedData[1] == 0x21 |
                driver.ReceivedData[1] == 0x22 | driver.ReceivedData[1] == 0x23))
            {
                if (!(driver.ReceivedData[1] == 0x23))
                {
                    for (int i = 2; i < 7; i += 2)
                    {
                        AddError(list, driver.ReceivedData[i], driver.ReceivedData[i + 1]);
                    }
                }
                else
                {
                    AddError(list, driver.ReceivedData[2], driver.ReceivedData[3]);
                }
            }
            else
            {
                if (isReadingActive)
                {
                    newErrorListAvailable = true;
                    isReadingActive = false;
                }
            }
            if (driver.ReceivedData[0] == 0x90 & driver.ReceivedData[1] == 0x10 & driver.ReceivedData[2] == 0x14 & driver.ReceivedData[3] == 0x62 &
                driver.ReceivedData[4] == 0xFD & driver.ReceivedData[5] == errorType)
            {
                list.Clear();
                AddError(list, driver.ReceivedData[6], driver.ReceivedData[7]);
                isReadingActive = true;
            }
            return newErrorListAvailable;
        }

        protected void AddError(List<string> list, byte data0, byte data1)
        {
            HasAnyErrorCodeRead = true;
            byte[] b = { data0, data1 };
            string s = "???";
            if (!(errorCodes.TryGetValue(BitConverter.ToInt16(b, 0), out s)))
            {
                s = b[0].ToString() + b[1].ToString();
            }

            if (s.Contains("Valve"))
            {
                HasCriticalError = true;
            }
            list.Add(s);
        }

        public void Initialize()
        {
            HasReceivedAnyMessage = false;
            HasAnyErrorCodeRead = false;
            HasCriticalError = false;
        }
    }
}