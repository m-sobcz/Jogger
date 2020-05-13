
using Jogger.Drivers;
using Jogger.IO;
using Jogger.Models;
using Jogger.Valves;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Jogger.Services
{
    public class TesterService : IDisposable, ITesterService
    {
        private IValveManager valveManager;
        IDigitalIO digitalIO;
        IDriver driver;
        ActionStatus actionStatus;
        ProgramState state = ProgramState.NotInitialized;
        public Result[] results = new Result[4];
        string communicationLog = "";
        public event ProgramStateEventHandler ProgramStateChanged;
        public delegate void ProgramStateEventHandler(object sender, ProgramState programState);
        public event ResultEventHandler ResultChanged;
        public delegate void ResultEventHandler(object sender, Result result, int channelNumber);
        public event ErrorsEventHandler ActiveErrorsChanged;
        public event ErrorsEventHandler OccuredErrorsChanged;
        public delegate void ErrorsEventHandler(object sender, string errors, int channelNumber);
        public event CommunicationLogEventHandler CommunicationLogChanged;
        public delegate void CommunicationLogEventHandler(object sender, string log);
        public event DigitalIOEventHandler DigitalIOChanged;
        public delegate void DigitalIOEventHandler(object sender, byte[] data);
        public string ValveType { get; set; } = "";
        public ProgramState State
        {
            get { return state; }
            set
            {
                state = value;
                Trace.WriteLine($"New program state: {state}");
                OnProgramStateChanged(state);
            }
        }
        public TesterService(IDigitalIO digitalIO, IDriver driver, IValveManager valveManager)
        {
            this.valveManager = valveManager;
            this.digitalIO = digitalIO;
            this.driver = driver;
        }
        public ActionStatus Initialize(ConfigurationSettings configurationSettings)
        {
            state = ProgramState.Initializing;
            List<ActionStatus> actionList = new List<ActionStatus>();
            actionList.Add(digitalIO.Initialize());
            actionList.Add(valveManager.Initialize(configurationSettings.HardwareChannelCount));
            actionList.Add(driver.Initialize());
            actionStatus = ActionListStatusToActionStatus(actionList);
            state = (actionStatus == ActionStatus.Error) ? ProgramState.Error : ProgramState.Initialized;
            //communication.OccuredErrorsChanged += OnOccuredErrorsChanged;
            //communication.ActiveErrorsChanged += OnActiveErrorsChanged;
            //communication.ResultChanged += OnResultChanged;
            //communication.CommunicationLogChanged += OnCommunicationLogChanged;
            driver.CommunicationLogChanged += OnCommunicationLogChanged;
            return actionStatus;
        }
        public ActionStatus Start(TestSettings testSettings)//Func<TestSettings, string, ActionStatus> startFunc, 
        {
            State = (ProgramState.Starting);
            actionStatus = valveManager.Start(testSettings, ValveType);//startFunc(testSettings, ValveType);
            if (actionStatus == ActionStatus.OK) State = (ProgramState.Started);
            else State = ProgramState.Error;
            return actionStatus;
        }
        public async Task CommunicationLoop()
        {
            while (true)
            {
                await digitalIO.ReadInputs();
                await valveManager.SendData();
                await Task.WhenAny(valveManager.ReceiveData(), Task.Delay(5000));
                valveManager.SetNextProcessedChannel();
            }
        }
        public ActionStatus Stop()
        {
            valveManager.Stop();
            State = ProgramState.Idle;
            return ActionStatus.OK;
        }
        public void Dispose()
        {
            digitalIO?.Dispose();
        }
        ActionStatus ActionListStatusToActionStatus(List<ActionStatus> actionList)
        {
            ActionStatus actionStatus;
            if (actionList.Contains(ActionStatus.Error))
            {
                actionStatus = ActionStatus.Error;
            }
            else if (actionList.Contains(ActionStatus.WarnigInExecution))
            {
                actionStatus = ActionStatus.WarnigInExecution;
            }
            else if (actionList.Contains(ActionStatus.WarningWrongParameter))
            {
                actionStatus = ActionStatus.WarningWrongParameter;
            }
            else
            {
                actionStatus = ActionStatus.OK;
            }
            return actionStatus;
        }
        void OnProgramStateChanged(ProgramState programState)
        {
            ProgramStateChanged?.Invoke(this, state);
        }
        void OnResultChanged(object sender, Result result, int channelNumber)
        {
            ResultChanged?.Invoke(this, result, channelNumber);
            if (valveManager.IsTestingDone)
            {
                state = ProgramState.Done;
            }
        }
        void OnActiveErrorsChanged(object sender, string errors, int channelNumber)
        {
            ActiveErrorsChanged?.Invoke(sender, errors, channelNumber);
        }
        void OnOccuredErrorsChanged(object sender, string errors, int channelNumber)
        {
            OccuredErrorsChanged?.Invoke(sender, errors, channelNumber);
        }
        void OnDigitalIOChanged(object sender, byte[] data)
        {
            DigitalIOChanged?.Invoke(sender, data);
        }
        void OnCommunicationLogChanged(object sender, string data)
        {
            CommunicationLogChanged?.Invoke(sender, communicationLog);
        }
    }
}
