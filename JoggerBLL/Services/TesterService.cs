﻿
using Jogger.Drivers;
using Jogger.IO;
using Jogger.Models;
using Jogger.Valves;
using Jogger.ValveTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Jogger.Services
{
    public class TesterService : IDisposable, ITesterService
    {
        private readonly IValveManager valveManager;
        readonly IDigitalIO digitalIO;
        readonly IDriver driver;
        ActionStatus actionStatus;
        ProgramState state = ProgramState.NotInitialized;
        public Result[] results = new Result[4];
        public event Action<ProgramState> ProgramStateChanged;
        public string ValveType { get; set; } = "";
        public ProgramState State
        {
            get { return state; }
            set
            {
                state = value;
                Trace.WriteLine($"New program state: {state}");
                ProgramStateChanged?.Invoke(state);
            }
        }
        public TesterService(IDigitalIO digitalIO, IDriver driver, IValveManager valveManager)
        {
            this.valveManager = valveManager;
            this.digitalIO = digitalIO;
            this.driver = driver;
            valveManager.TestingFinished += ValveManager_TestingFinished;
        }
        public ActionStatus Initialize(int hardwareChannelCount = 4)
        {
            State = ProgramState.Initializing;
            actionStatus = ActionListStatusToActionStatus(GetInitializeActionList(hardwareChannelCount));
            State = (actionStatus == ActionStatus.Error) ? ProgramState.Error : ProgramState.Initialized;
            return actionStatus;
        }
        public ActionStatus Start(TestSettings testSettings, string valveType)
        {
            actionStatus = valveManager.Start(testSettings, valveType);
            State = (actionStatus == ActionStatus.OK) ? ProgramState.Started : ProgramState.Idle;
            return actionStatus;
        }
        public async Task CommunicationLoop()
        {
            while (true)
            {
                await digitalIO.ReadInputs();
                await valveManager.Send();
                await valveManager.Receive();
                valveManager.SetNextProcessedValve();
            }
        }
        public ActionStatus Stop()
        {
            valveManager.Stop();
            State = ProgramState.Stopping;
            return ActionStatus.OK;
        }
        public void Dispose()
        {
            digitalIO?.Dispose();
            driver?.Dispose();
        }
        List<ActionStatus> GetInitializeActionList(int hardwareChannelCount)
        {
            List<ActionStatus> actionList = new List<ActionStatus>
            {
                digitalIO.Initialize(),
                driver.Initialize(hardwareChannelCount),
                valveManager.Initialize(hardwareChannelCount)
            };
            return actionList;
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
        private void ValveManager_TestingFinished()
        {
            if (State == ProgramState.Started) State = ProgramState.Done;
            else if (State == ProgramState.Stopping) State = ProgramState.Idle;
        }
    }

}
