using Jogger.Drivers;
using Jogger.IO;
using Jogger.Services;
using Jogger.Valves;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Jogger.Communication
{
    class Communication
    {
        readonly IDriver driver;
        readonly IDigitalIO digitalIO;
        ValveManager valveManager;
        public bool IsTestingDone { get; set; } = false;
        int actualProcessedChannel = 0;
        int previousProcessedChannel = 0;
        private TestSettings testSettings;

        public event ErrorsEventHandler ActiveErrorsChanged;
        public event ErrorsEventHandler OccuredErrorsChanged;
        public delegate void ErrorsEventHandler(object sender, string errors, int channelNumber);
        public event ResultEventHandler ResultChanged;
        public delegate void ResultEventHandler(object sender, Result result, int channelNumber);
        public event CommunicationLogEventHandler CommunicationLogChanged;
        public delegate void CommunicationLogEventHandler(object sender, string log);

        public Communication(IDigitalIO digitalIO, LinDriver driver, ValveManager valveManager)
        {
            this.valveManager = valveManager;
            this.driver = driver;
            this.digitalIO = digitalIO;
        }
        public async Task<byte[]> ReadIO()
        {
            string ioError;
            byte[] ioResult;
            (ioError, ioResult) = await digitalIO.ReadInputs();
            valveManager.SetSensorsState(ioResult);
            return ioResult;
        }
        public BitArray GetDigitalInputs()
        {
            BitArray bitArray = new BitArray(valveManager.valves.Count * 2);
            //for (int i = 0; i < valves.Count; i++)
            //{
            //    bitArray[i * 2] = valves[i].IsInflateSensorOn;
            //    bitArray[i * 2 + 1] = valves[i].IsDeflateSensorOn;
            //}
            return bitArray;
        }

        public void Initialize(int hardwareChannelCount)
        {
            for (int i = 0; i < hardwareChannelCount; i++)
            {
                Valve valve = App.ServiceProvider.GetRequiredService<Valve>();           
                valve.result = Result.Idle;
                valveManager.Add(valve);
                OnResultChanged(this, Result.Idle, i);
            }
            Trace.WriteLine($"Valves added, valves count {valveManager.valves.Count}");
        }
        public ActionStatus Start(TestSettings testSettings, string valveType)
        {
            this.testSettings = testSettings;
            ActionStatus actionStatus = ActionStatus.OK;
            valveManager.SetSequencerType(valveType);
            valveManager.SetReceiverType(valveType);
            InitializeReceivers();
            SetRepetitionsAndValveActivationTime();
            StartSequencers();
            return actionStatus;
        }

        public Result CheckResult()
        {
            Result result;
            if (valveManager.valves[actualProcessedChannel].Sequencer.IsStopRequested)
            {
                result = Result.Stopped;
            }
            else if (!(valveManager.valves[actualProcessedChannel].Receiver.HasReceivedAnyMessage))
            {
                result = Result.DoneErrorConnection;
            }
            else if (valveManager.valves[actualProcessedChannel].Sequencer.isUntimelyDone)
            {
                result = Result.DoneErrorTimeout;
            }
            else if (valveManager.valves[actualProcessedChannel].Receiver.HasCriticalError)
            {
                result = Result.DoneErrorCriticalCode;
            }
            else
            {
                result = Result.DoneOk;
            }
            valveManager.valves[actualProcessedChannel].Sequencer.IsDone = false;

            return result;
        }
        public void SetNextProcessedChannel()
        {
            previousProcessedChannel = actualProcessedChannel;
            while (true)
            {
                actualProcessedChannel++;
                if (actualProcessedChannel >= valveManager.valves.Count)
                {
                    actualProcessedChannel = 0;
                }
                if (valveManager.valves[actualProcessedChannel].Sequencer.IsStarted |
                    actualProcessedChannel == previousProcessedChannel)
                {
                    //Trace.WriteLine($"changed at actualProcessedChannel {actualProcessedChannel}");
                    break;
                }
            }
        }
        public async Task SendData()
        {
            if (valveManager.valves[actualProcessedChannel].Sequencer.IsStarted)
            {
                string dataToDriver = await valveManager.valves[actualProcessedChannel].Sequencer.ExecuteStep(driver.MasterMask[actualProcessedChannel]);
                if (testSettings.IsLogOutDataSelected)
                {
                    OnCommunicationLogChanged(this, dataToDriver);
                }
            }
        }
        public async Task ReceiveData()
        {
            string dataFromDriver = await valveManager.valves[actualProcessedChannel].Receiver.Receive();
            if (testSettings.IsLogInDataSelected)
            {
                OnCommunicationLogChanged(this, dataFromDriver);
            }
            if (valveManager.valves[actualProcessedChannel].Receiver.isNewActiveListAvailable)
            {
                string errors = "";
                foreach (string s in valveManager.valves[actualProcessedChannel].Receiver.ActiveErrorList)
                {
                    errors += s + "\n";
                }
                OnActiveErrorsChanged(this, errors, actualProcessedChannel);
            }
            if (valveManager.valves[actualProcessedChannel].Receiver.isNewOccuredListAvailable)
            {
                string errors = "";
                foreach (string s in valveManager.valves[actualProcessedChannel].Receiver.OccuredErrorList)
                {
                    errors += s + "\n";
                }
                OnOccuredErrorsChanged(this, errors, actualProcessedChannel);
            }
            if (valveManager.valves[actualProcessedChannel].Sequencer.IsDone)
            {
                OnResultChanged(this, CheckResult(), actualProcessedChannel);
            }
            SetNextProcessedChannel();
        }
        public void InitializeReceivers()
        {
            foreach (Valve valve in valveManager.valves)
            {
                valve.Receiver.Initialize();
            }
        }
        public void StartSequencers()
        {
            IsTestingDone = false;
            foreach (Valve valve in valveManager.valves)
            {
                valve.Sequencer.Start();
                valve.result = Result.Testing;
                OnResultChanged(this, Result.Testing, valve.ChannelNumber);
                OnActiveErrorsChanged(this, "...", valve.ChannelNumber);
                OnOccuredErrorsChanged(this, "...", valve.ChannelNumber); ;
            }
        }

        public void SetRepetitionsAndValveActivationTime()
        {
            foreach (Valve valve in valveManager.valves)
            {

                valve.Sequencer.valveMinInflateTime = testSettings.ValveMinInflateTime;
                valve.Sequencer.valveMinDeflateTime = testSettings.ValveMinDeflateTime;
                valve.Sequencer.valveMaxInflateTime = testSettings.ValveMaxInflateTime;
                valve.Sequencer.valveMaxDeflateTime = testSettings.ValveMaxDeflateTime;
                valve.Sequencer.Repetitions = testSettings.Repetitions;
            }
        }
        public void Dispose()
        {
            driver?.Close();
        }
        public void OnActiveErrorsChanged(object sender, string errors, int channelNumber)
        {
            ActiveErrorsChanged?.Invoke(this, errors, channelNumber);
        }
        public void OnOccuredErrorsChanged(object sender, string errors, int channelNumber)
        {
            OccuredErrorsChanged?.Invoke(this, errors, channelNumber);
        }
        public void OnCommunicationLogChanged(object sender, string log)
        {
            CommunicationLogChanged?.Invoke(this, log);
        }
        public void OnResultChanged(object sender, Result result, int channelNumber)
        {
            valveManager.valves[channelNumber].result = result;
            bool testDoneCheck = true;
            for (int i = 0; i < valveManager.valves.Count; i++)
            {
                if (valveManager.valves[i].result == Result.Idle | valveManager.valves[i].result == Result.Testing) testDoneCheck = false;
            }
            IsTestingDone = testDoneCheck;
            ResultChanged?.Invoke(this, result, channelNumber);
        }
        public void StopAll()
        {
            foreach (Valve valve in valveManager.valves)
            {
                valve.Sequencer.IsStopRequested = true;
            }
        }
    }
}
