using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Automation.BDaq;
using Jogger.Services;
using Jogger.Valves;
using static Jogger.IO.IDigitalIO;

namespace Jogger.IO
{
    public class Advantech : IDigitalIO
    {
        readonly string deviceDescription = "USB-4761,BID#0";//z device manager
        readonly string profilePath = "../../../Resources/staticDi4761.xml";
        readonly int startPort = 0;
        readonly int portCount = 1;
        readonly byte[] buffer = new byte[64];
        readonly InstantDiCtrl advantechDIControl = new InstantDiCtrl();
        ErrorCode errorCode = ErrorCode.ErrorUndefined;
        private DioPort[] dioPort;
        public event InputsReadEventHandler InputsRead;
        public event Action<string> CommunicationLogChanged;
        public ActionStatus Initialize()
        {
            try
            {
                advantechDIControl.SelectedDevice = new DeviceInformation(deviceDescription);
                errorCode = advantechDIControl.LoadProfile(profilePath);
                dioPort = advantechDIControl.Ports;
            }
            catch (Exception e)
            {
                CommunicationLogChanged?.Invoke($"Advantech init exception {e.Message}!\n");
            }
            CommunicationLogChanged?.Invoke($"Advantech Init status: {errorCode}\n");
            return ErrorCodeToActionStatus(errorCode);
        }
        public async Task<(string, byte[])> ReadInputs()
        {
            try
            {
                errorCode = await Task<string>.Run(() => advantechDIControl.Read(startPort, portCount, buffer));
                OnInputsRead(this, errorCode.ToString(), buffer);
            }
            catch (Exception e)
            {
                CommunicationLogChanged?.Invoke($"Advantech read exception {e.Message} !\n");
            }
            return (errorCode.ToString(), buffer);
        }
        public void Dispose()
        {
            advantechDIControl?.Dispose();
        }
        ActionStatus ErrorCodeToActionStatus(ErrorCode errorCode)
        {
            ActionStatus actionStatus;
            if (errorCode.ToString().Contains("Error")) actionStatus = ActionStatus.Error;
            else if (errorCode.ToString().Contains("Warning")) actionStatus = ActionStatus.WarnigInExecution;
            else actionStatus = ActionStatus.OK;
            return actionStatus;
        }
        void OnInputsRead(object sender, string errorCode, byte[] buffer)
        {
            InputsRead.Invoke(sender, errorCode, buffer);
        }
        public Task<string> WriteOutputs(byte[] output)
        {
            throw new NotImplementedException();
        }
    }
}
