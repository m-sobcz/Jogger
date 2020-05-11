using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using vxlapi_NET;

namespace Jogger.Drivers
{
    public class VectorHardware
    {
        uint hardwareIndex = 0;
        uint hardwareChannel = 0;
        readonly int numberOfChannels;
        public UInt64[] accessMaskMaster;
        public int[] channelIndex;
        XLDefine.XL_HardwareType hwType = XLDefine.XL_HardwareType.XL_HWTYPE_NONE;
        readonly String applicationName;
        readonly XLDriver driver;
        XLClass.xl_driver_config driverConfig;
        public VectorHardware(XLDriver driver, XLClass.xl_driver_config driverConfig, String applicationName, int numberOfChannels)
        {
            this.driver = driver;
            this.driverConfig = driverConfig;
            this.applicationName = applicationName;
            this.numberOfChannels = numberOfChannels;
            this.accessMaskMaster = new UInt64[numberOfChannels];
            this.channelIndex = new int[numberOfChannels];
        }
        public XLDefine.XL_Status SetConfiguration()
        {
            bool existNotAssignedChannel = false;
            XLDefine.XL_Status[] status = new XLDefine.XL_Status[numberOfChannels];
            for (int i = 0; i < numberOfChannels; i++)
            {
                status[i] = driver.XL_GetApplConfig(applicationName, (uint)i, ref hwType, ref hardwareIndex, ref hardwareChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_LIN);
                if (status[i] != XLDefine.XL_Status.XL_SUCCESS)
                {
                    existNotAssignedChannel = true;
                }
            }
            if (existNotAssignedChannel)
            {
                Trace.WriteLine("Application configuration not found, creating new one.");
                for (int i = 0; i < numberOfChannels; i++)
                {
                    driver.XL_SetApplConfig(applicationName, (uint)i, 0, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_LIN);
                }
            }
            XLDefine.XL_Status maxChannelStatus = status.Max();
            Trace.WriteLine("Get Application Configuration: " + maxChannelStatus);
            return maxChannelStatus;
        }
        public XLDefine.XL_Status AssignChannels()
        {
            bool success = true;
            for (int i = 0; i < numberOfChannels; i++)
            {
                if (!GetApplicationChannel(i, ref accessMaskMaster[i], ref channelIndex[i]))
                {
                    success = false;
                }
            }
            Trace.WriteLine("Get Application Channel: " + success);
            XLDefine.XL_Status status = success ? XLDefine.XL_Status.XL_SUCCESS : XLDefine.XL_Status.XL_ERROR;
            return status;
        }
        [STAThread]
        private bool GetApplicationChannel(int appChIdx, ref UInt64 chMask, ref int chIdx)
        {
            XLDefine.XL_Status status = driver.XL_GetApplConfig(applicationName, (uint)appChIdx, ref hwType, ref hardwareIndex, ref hardwareChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_LIN);
            Trace.WriteLine("XL_GetApplConfig : " + status);
            chMask = driver.XL_GetChannelMask(hwType, (int)hardwareIndex, (int)hardwareChannel);
            chIdx = driver.XL_GetChannelIndex(hwType, (int)hardwareIndex, (int)hardwareChannel);
            Trace.WriteLine("chMask : " + chMask + " chIdx: " + chIdx);
            if (chIdx < 0 || chIdx >= driverConfig.channelCount)
            {
                return false;
            }
            return (driverConfig.channel[chIdx].channelBusCapabilities & XLDefine.XL_BusCapabilities.XL_BUS_ACTIVE_CAP_LIN) != 0;
        }
    }
}
