using Jogger.Models;
using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using vxlapi_NET;

namespace Jogger.Drivers
{
    public class Driver : IDriver
    {
        XLDefine.XL_LIN_CalcChecksum calcChecksumType;
        private  ConfigurationSettings configurationSettings;
        readonly XLDefine.XL_LIN_Mode linMode;
        readonly XLDefine.XL_LIN_Version linVersion;
        public event EventHandler InitializationFailed;
        VectorHardware vectorHardware;
        string InitializeInfo { get; set; } = "";
        public event CommunicationLogEventHandler CommunicationLogChanged;
        public delegate void CommunicationLogEventHandler(object sender, string log);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WaitForSingleObject(int handle, int timeOut);
        byte linSlaveId = 0x1;
        public int baudrate = 19200;
        readonly byte[] DLC = new byte[64];
        readonly byte[] linData = new byte[8];
        public byte[] ReceivedData { get; set; } = new byte[8];
        ulong currentAccessMask = 0;
        public ulong[] MasterMask { get; set; }
        byte dataLengthCode = 2;
        readonly XLDriver driver = new XLDriver();
        readonly string applicationName = "Jogger";
        XLClass.xl_driver_config driverConfig = new XLClass.xl_driver_config();
        public ulong SlaveMask { get; } = 0;
        int portHandle = -1;
        int eventHandle = -1;
        ulong permissionMask = 0;
        XLDefine.XL_Status status;
        private bool initializationWithoutErrors;
        public Driver(ConfigurationSettings configurationSettings)
        {
            byte zeroSize = 8;
            byte otherSize = 8;
            this.linVersion = XLDefine.XL_LIN_Version.XL_LIN_VERSION_2_0;
            this.linMode = XLDefine.XL_LIN_Mode.XL_LIN_MASTER;        
            this.calcChecksumType = XLDefine.XL_LIN_CalcChecksum.XL_LIN_CALC_CHECKSUM;
            this.configurationSettings = configurationSettings;
            DLC[0] = zeroSize;
            for (int i = 1; i < 63; i++)
            { DLC[i] = otherSize; }
            DLC[60] = zeroSize;
        }
        public void SetSendData(byte[] data, byte id, byte dataLengthCode, int channelNumber)
        {
            ulong accessMask = MasterMask[channelNumber];
            linSlaveId = id;
            bool isReceivingData = (id % 2 == 1);
            this.dataLengthCode = dataLengthCode;
            for (int i = 0; i < 8; i++)
            {
                linData[i] = data[i];
            }
            
            if (!isReceivingData)
            {
                SetLinSlave(accessMask);//accessMask
            }
            else
            {
                SetLinSlave(SlaveMask);//SlaveMask
            }
            currentAccessMask = accessMask;

        }
        public string Send()
        {
            XLDefine.XL_Status status;
            status = driver.XL_LinSendRequest(portHandle, currentAccessMask, linSlaveId, 0);
            string message = "linSlaveId: " + linSlaveId + ", accessMask: " + currentAccessMask + " data " + "porthHandle " + portHandle;
            foreach (byte b in linData)
            {
                message += b.ToString().PadLeft(4) + "   ";
            }
            message += ": " + status.ToString();
          DriverAction(message, status, false);
            return message;
        }
        public string Receive()
        {
            string dataFromDriver = "";
            for (int i = 0; i < 8; i++)
            {
                ReceivedData[i] = 0;
            }
            XLClass.xl_event receivedEvent = new XLClass.xl_event();
            XLDefine.XL_Status xlStatus = XLDefine.XL_Status.XL_SUCCESS;
            XLDefine.WaitResults waitResult = (XLDefine.WaitResults)WaitForSingleObject(eventHandle, 1000);
            if (waitResult != XLDefine.WaitResults.WAIT_TIMEOUT)
            {
                dataFromDriver += $"[{DateTime.Now.Hour:D2} : {DateTime.Now.Minute:D2} : {DateTime.Now.Second:D2} : {DateTime.Now.Millisecond:D3}]   ";
                while (xlStatus != XLDefine.XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                {
                    xlStatus = driver.XL_Receive(portHandle, ref receivedEvent);
                    switch (receivedEvent.tag)
                    {
                        case XLDefine.XL_EventTags.XL_LIN_MSG:
                            string dir = "RX".PadRight(3);
                            if ((receivedEvent.tagData.linMsgApi.linMsg.flags & XLDefine.XL_MessageFlags.XL_LIN_MSGFLAG_TX)
                              == XLDefine.XL_MessageFlags.XL_LIN_MSGFLAG_TX)
                            {
                                dir = "TX".PadRight(3);
                            }
                            else
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    ReceivedData[i] = receivedEvent.tagData.linMsgApi.linMsg.data[i];
                                }
                            }

                            dataFromDriver += ("XL_LIN_MSG, " + dir + " id: " +
                                  receivedEvent.tagData.linMsgApi.linMsg.id.ToString("X2").PadRight(3) + ", data: " +
                                  receivedEvent.tagData.linMsgApi.linMsg.data[0].ToString("X2") + " " +
                                  receivedEvent.tagData.linMsgApi.linMsg.data[1].ToString("X2") + " " +
                                  receivedEvent.tagData.linMsgApi.linMsg.data[2].ToString("X2") + " " +
                                  receivedEvent.tagData.linMsgApi.linMsg.data[3].ToString("X2") + " " +
                                  receivedEvent.tagData.linMsgApi.linMsg.data[4].ToString("X2") + " " +
                                  receivedEvent.tagData.linMsgApi.linMsg.data[5].ToString("X2") + " " +
                                  receivedEvent.tagData.linMsgApi.linMsg.data[6].ToString("X2") + " " +
                                  receivedEvent.tagData.linMsgApi.linMsg.data[7].ToString("X2")) + " (cha-1) " +
                                  receivedEvent.chanIndex.ToString();
                            break;

                        case XLDefine.XL_EventTags.XL_LIN_ERRMSG:
                            dataFromDriver += ("XL_LIN_ERRMSG");
                            break;

                        case XLDefine.XL_EventTags.XL_LIN_SYNCERR:
                            dataFromDriver += ("XL_LIN_SYNCERR");
                            break;

                        case XLDefine.XL_EventTags.XL_LIN_NOANS:
                            dataFromDriver += ("XL_LIN_NOANS" )  +" (cha-1) " +  receivedEvent.chanIndex.ToString();
                            break;

                        case XLDefine.XL_EventTags.XL_LIN_WAKEUP:
                            dataFromDriver += ("XL_LIN_WAKEUP");
                            break;

                        case XLDefine.XL_EventTags.XL_LIN_SLEEP:
                            dataFromDriver += ("XL_LIN_SLEEP");
                            break;

                        case XLDefine.XL_EventTags.XL_LIN_CRCINFO:
                            dataFromDriver += ("XL_LIN_CRCINFO");
                            break;
                    }
                }
            }
            else
            {
                dataFromDriver += "Timeout";
            }
            //Trace.WriteLine(dataFromDriver);
            driver.XL_FlushReceiveQueue(portHandle);
            return dataFromDriver;
        }
        public void Close()
        {
            DriverAction("Close port", (driver.XL_ClosePort(portHandle)));
            DriverAction("Close driver", driver.XL_CloseDriver());
        }
        public ActionStatus Initialize(int numberOfChannels)
        {
            initializationWithoutErrors = true;
            this.MasterMask = new ulong[numberOfChannels];
            vectorHardware = new VectorHardware(driver, driverConfig, applicationName, numberOfChannels);
            OpenDriver();
            GetDriverConfiguration();
            InitializeVectorHardware();
            permissionMask = 0;
            for (int i = 0; i < numberOfChannels; i++)
            {
                permissionMask |= vectorHardware.accessMaskMaster[i];
                MasterMask[i] = vectorHardware.accessMaskMaster[i];
            }
            OpenPort();
            SetLinChannelParameters();
            SetDataLengthControl();
            ActivateChannel();
            SetNotificationAndReadHandle();
            CommunicationLogChanged?.Invoke(this, InitializeInfo);
            return initializationWithoutErrors ? ActionStatus.OK : ActionStatus.Error;
        }
        bool OpenDriver()
        {
            XLDefine.XL_Status status = driver.XL_OpenDriver();
            return DriverAction("Open driver", status);
        }
        public bool WakeUp()
        {
            XLDefine.XL_Status status = driver.XL_LinWakeUp(portHandle, 0);
            return DriverAction("Wake Up", status, true);
        }
        bool OpenPort()
        {
            XLDefine.XL_Status status = driver.XL_OpenPort(ref portHandle, applicationName, permissionMask, ref permissionMask, 256, XLDefine.XL_InterfaceVersion.XL_INTERFACE_VERSION_V3, XLDefine.XL_BusTypes.XL_BUS_TYPE_LIN);
            return DriverAction("Open port", status);
        }
        bool SetLinChannelParameters()
        {
            XLClass.xl_linStatPar linChannelParameters = new XLClass.xl_linStatPar
            {
                baudrate = baudrate,
                LINMode = linMode,
                LINVersion = linVersion
            };
            XLDefine.XL_Status[] status = new XLDefine.XL_Status[configurationSettings.HardwareChannelCount];
            for (int i = 0; i < configurationSettings.HardwareChannelCount; i++)
            {
                status[i] = driver.XL_LinSetChannelParams(portHandle, MasterMask[i], linChannelParameters);
            }
            XLDefine.XL_Status maxChannelStatus = status.Max();
            return DriverAction("Set Lin Parameters", maxChannelStatus);
        }
        bool SetDataLengthControl()
        {
            XLDefine.XL_Status[] status = new XLDefine.XL_Status[configurationSettings.HardwareChannelCount];
            for (int i = 0; i < configurationSettings.HardwareChannelCount; i++)
            {
                status[i] = driver.XL_LinSetDLC(portHandle, MasterMask[i], DLC);
            }
            XLDefine.XL_Status maxChannelStatus = status.Max();
            return DriverAction("Set Data Length Control", maxChannelStatus);
        }
        bool SetLinSlave(ulong accessMask)
        {
            calcChecksumType = XLDefine.XL_LIN_CalcChecksum.XL_LIN_CALC_CHECKSUM;//short enum bug
            status = driver.XL_LinSetSlave(portHandle, accessMask, linSlaveId, linData, dataLengthCode, calcChecksumType);
            return DriverAction("Set Lin Slave", status, false);
        }
        bool ActivateChannel()
        {
            XLDefine.XL_Status[] status = new XLDefine.XL_Status[configurationSettings.HardwareChannelCount];
            for (int i = 0; i < configurationSettings.HardwareChannelCount; i++)
            {
                status[i] = driver.XL_ActivateChannel(portHandle, MasterMask[i], XLDefine.XL_BusTypes.XL_BUS_TYPE_LIN, XLDefine.XL_AC_Flags.XL_ACTIVATE_NONE);
            }
            XLDefine.XL_Status maxChannelStatus = status.Max();
            return DriverAction("Activate Channel", maxChannelStatus);
        }
        bool SetNotificationAndReadHandle()
        {
            status = driver.XL_SetNotification(portHandle, ref eventHandle, 1);
            return DriverAction("Set Notification&read handle", status);
        }
        bool InitializeVectorHardware()
        {
            status = vectorHardware.SetConfiguration();
            if (!DriverAction("Set VectorHardware configuration", status)) return false;

            status = vectorHardware.AssignChannels();
            if (DriverAction("Assign VectorHardware channels", status))
            {
                return true;
            }
            else
            {
                driver.XL_PopupHwConfig();
                return false;
            }
        }
        bool GetDriverConfiguration()
        {
            XLDefine.XL_Status status = driver.XL_GetDriverConfig(ref driverConfig);
            return DriverAction("Get Driver Configuration", status);
        }
        bool DriverAction(string actionName, XLDefine.XL_Status status, bool isTraced = true)
        {
            if (isTraced)
            {
                string msg = actionName + ": " + status + " ";
                Trace.WriteLine(msg);
                InitializeInfo += msg + "\n";
            }
            if (!status.Equals(XLDefine.XL_Status.XL_SUCCESS))
            {
                initializationWithoutErrors = false;
                OnInitializationFailed();
            }
            return status.Equals(XLDefine.XL_Status.XL_SUCCESS);
        }
        public void OnInitializationFailed()
        {
            InitializationFailed?.Invoke(this, EventArgs.Empty);
        }

    }
}
