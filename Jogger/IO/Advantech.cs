using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Automation.BDaq;
using Jogger.Services;

namespace Jogger.IO
{
    public class Advantech : IDigitalIO
    {
        readonly string deviceDescription = "USB-4761,BID#0";//z device manager
        readonly string profilePath = "../../Resources/staticDi4761.xml";
        readonly int startPort = 0;
        readonly int portCount = 1;
        readonly byte[] buffer = new byte[64];
        InstantDiCtrl instantDiCtrl = new InstantDiCtrl();
        ErrorCode errorCode = ErrorCode.ErrorUndefined;
        DioPort[] dioPort;
        public ActionStatus Initialize()
        {
            try
            {
                instantDiCtrl.SelectedDevice = new DeviceInformation(deviceDescription);
                errorCode = instantDiCtrl.LoadProfile(profilePath);
                dioPort = instantDiCtrl.Ports;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Advantech init exception {e.Message} {e.StackTrace}!");
            }
            Trace.WriteLine($"Advantech Init: {errorCode}");
            ActionStatus actionStatus;
            if (errorCode.ToString().Contains("Error")) actionStatus = ActionStatus.Error;
            else if (errorCode.ToString().Contains("Warning")) actionStatus = ActionStatus.WarnigInExecution;
            else actionStatus = ActionStatus.OK;
            return actionStatus;
        }
        public async Task<(string, byte[])> ReadInputs()
        {
            try
            {
                errorCode = await Task<string>.Run(() => instantDiCtrl.Read(startPort, portCount, buffer));
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Advantech read exception {e.Message} {e.StackTrace}!");
            }
            return (errorCode.ToString(), buffer);
        }
        public void Dispose()
        {
            instantDiCtrl.Dispose();
        }
    }
}
