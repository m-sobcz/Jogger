﻿
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
        public string ValveType { get; set; } = "";
        public ProgramState State
        {
            get { return state; }
            set
            {
                state = value;
                Trace.WriteLine($"New program state: {state}");
                ProgramStateChanged?.Invoke(this, state);
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
            State = ProgramState.Initializing;
            List<ActionStatus> actionList = new List<ActionStatus>();
            actionList.Add(digitalIO.Initialize());//show error to user
            actionList.Add(driver.Initialize(configurationSettings.HardwareChannelCount));
            actionList.Add(valveManager.Initialize(configurationSettings.HardwareChannelCount)); 
            actionStatus = ActionListStatusToActionStatus(actionList);
            State = (actionStatus == ActionStatus.Error) ? ProgramState.Error : ProgramState.Initialized;
            return actionStatus;
        }
        public ActionStatus Start(TestSettings testSettings)
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
    }
}
