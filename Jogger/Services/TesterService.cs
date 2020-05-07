﻿
using Jogger.Communication;
using Jogger.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Jogger.Services
{
    public class TesterService : IDisposable
    {
        #region fields
        ProgramState state = ProgramState.NotInitialized;
        public Result[] results = new Result[4];
        string communicationLog = "";
        ICommunication communication;
        IDigitalIO digitalIO;
        private TestSettings testSettings = new TestSettings();//TODO

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
        #endregion
        #region properties
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
        #endregion
        public TesterService(ICommunication communication, IDigitalIO digitalIO)
        {
            this.communication = communication;
            this.digitalIO = digitalIO;
        }

        public ActionStatus Initialize(ConfigurationSettings configurationSettings, Func<Task> afterInitializationTask)
        {
            OnProgramStateChanged(ProgramState.Initializing);
            ActionStatus actionStatus = digitalIO.Initialize();
            //driver = new LinDriver(configurationSettings.HardwareChannelCount);
            //linCommunication = new LinCommunication(digitalIO, driver);
            //communication.OccuredErrorsChanged += OnOccuredErrorsChanged;
            //communication.ActiveErrorsChanged += OnActiveErrorsChanged;
            //communication.ResultChanged += OnResultChanged;
            //communication.CommunicationLogChanged += OnCommunicationLogChanged;
            //driver.CommunicationLogChanged += OnCommunicationLogChanged;
            //bool isInitializationSuccessful = driver.Initialize();
            //if (isInitializationSuccessful)
            //{
            //    communication.Initialize(configurationSettings.HardwareChannelCount);
            //    OnProgramStateChanged(ProgramState.Initialized);
            //}
            //else
            //{
            //    errorCode = ErrorCode.Error;
            //    OnProgramStateChanged(ProgramState.Error);
            //}
            if (actionStatus == ActionStatus.OK) State = (ProgramState.Initialized);
            else State = ProgramState.Error;
            _ = afterInitializationTask.Invoke();
            return actionStatus;
        }
        public ActionStatus Start(TestSettings testSettings)
        {
            this.testSettings = testSettings;
            State = (ProgramState.Starting);
            ActionStatus actionStatus = communication.Start(testSettings, ValveType);
            if (actionStatus == ActionStatus.OK) State = (ProgramState.Started);
            else State = ProgramState.Error;
            return actionStatus;
        }
        public async Task CommunicationLoop()
        {
            communication.SetSequencerAndReceiver(ValveType);
            byte[] ioData;
            while (true)
            {
                ioData = await communication.ReadIO();
                OnDigitalIOChanged(this, ioData);
                await communication.SendData();
                await Task.WhenAny(communication.ReceiveData(), Task.Delay(5000));
            }
        }
        public ActionStatus Stop()
        {
            OnProgramStateChanged(ProgramState.Stopping);
            communication.StopAll();
            OnProgramStateChanged(ProgramState.Idle);
            return ActionStatus.OK;
        }
        public void Dispose()
        {
            communication?.Dispose();
            digitalIO?.Dispose();
        }
        void OnProgramStateChanged(ProgramState programState)
        {
            ProgramStateChanged?.Invoke(this, state);
        }
        void OnResultChanged(object sender, Result result, int channelNumber)
        {
            ResultChanged?.Invoke(this, result, channelNumber);
            if (communication.IsTestingDone)
            {
                OnProgramStateChanged(ProgramState.Done);
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
            CommunicationLogChanged?.Invoke(this, communicationLog);
        }
    }




}