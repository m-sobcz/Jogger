﻿using Jogger.Drivers;
using Jogger.IO;
using Jogger.Models;
using Jogger.Services;
using Jogger.ValveTypes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using static Jogger.Valves.IValveManager;

namespace Jogger.Valves
{
    public class ValveManager : IValveManager
    {
        int actualProcessedChannel = 0;
        int previousProcessedChannel = 0;
        public List<IValve> valves = new List<IValve>();
        readonly Func<IValve> getValve;
        //private IDriver driver;
        readonly IDigitalIO digitalIO;
        private TestSettings testSettings;
        public event CommunicationLogEventHandler CommunicationLogChanged;
        public event ErrorsEventHandler ActiveErrorsChanged;
        public event ErrorsEventHandler OccuredErrorsChanged;
        public event ResultEventHandler ResultChanged;
        public event EventHandler TestingFinished;
        public ValveManager(IDigitalIO digitalIO, TestSettings testSettings, Func<IValve> getValve)
        {
            this.getValve = getValve;
            this.testSettings = testSettings;
            this.digitalIO = digitalIO;
            digitalIO.InputsRead += DigitalIO_InputsRead;
        }
        public void SetTestSettings(TestSettings testSettings)
        {
            this.testSettings = testSettings;
        }
        public ActionStatus Initialize(int channelsCount)
        {
            for (int i = 0; i < channelsCount; i++)
            {
                valves.Add(getValve());
                valves[i].OccuredErrorsChanged +=
                    (object sender, string errors, int channelNumber) => OccuredErrorsChanged?.Invoke(sender, errors, channelNumber);
                valves[i].ActiveErrorsChanged +=
                    (object sender, string errors, int channelNumber) => ActiveErrorsChanged?.Invoke(sender, errors, channelNumber);
                valves[i].ResultChanged += ValveManager_ResultChanged;
            }
            return ActionStatus.OK;
        }

        public ActionStatus Start(TestSettings testSettings, string valveTypeTxt)
        {
            ActionStatus actionStatus = ActionStatus.OK;
            this.testSettings = testSettings;
            Valve.testSettings = testSettings;
            foreach (Valve valve in valves)
            {
                ValveType valveType = GetValveType(valveTypeTxt);
                if (valveType is null) actionStatus = ActionStatus.Error;
                else
                {
                    valve.Queries = valveType.queryList;
                    valve.errorCodes = valveType.errorCodes;
                    valve.Start();
                }
            }
            return actionStatus;
        }
        ValveType GetValveType(string valveTypeTxt)
        {
            string namespacePrefix = System.Reflection.Assembly.GetExecutingAssembly().EntryPoint.DeclaringType.Namespace;

            Type valveType = Type.GetType(namespacePrefix + ".ValveTypes.ValveType" + valveTypeTxt, false);
            if (valveType is null)
            {
                Trace.WriteLine("ValveType could not be created !");
                return null;
            }
            else
            {
                return Activator.CreateInstance(valveType) as ValveType;
            }
        }
        public ActionStatus Stop()
        {
            foreach (Valve valve in valves)
            {
                valve.IsStopRequested = true;
            }
            return ActionStatus.OK;
        }
        public async Task Send()
        {
            if (valves[actualProcessedChannel].IsStarted) // If query done go to next channel
            {
                string dataToDriver = await valves[actualProcessedChannel].ExecuteStep();
                if (Valve.testSettings.IsLogOutDataSelected)
                {
                    CommunicationLogChanged?.Invoke(this, dataToDriver + "\n");
                }
            }
        }
        public async Task Receive()
        {
            string dataFromDriver = await valves[actualProcessedChannel].Receive();
            if (testSettings.IsLogInDataSelected & (testSettings.IsLogTimeoutSelected | !dataFromDriver.Equals("Timeout")))
            {
                CommunicationLogChanged?.Invoke(this, dataFromDriver + "\n");
            }
            SetNextProcessedChannel();
        }
        void ValveManager_ResultChanged(object sender, Result result, int channelNumber)
        {
            CheckTestingDone();
            ResultChanged?.Invoke(sender, result, channelNumber);
        }
        void CheckTestingDone()
        {
            bool isTestingDone = true;
            foreach (Valve valve in valves)
            {
                if (valve.Result == Result.Testing) isTestingDone = false;
            }
            if (isTestingDone) TestingFinished?.Invoke(this, EventArgs.Empty);
        }
        void SetNextProcessedChannel()
        {
            previousProcessedChannel = actualProcessedChannel;
            while (true)
            {
                actualProcessedChannel++;
                if (actualProcessedChannel >= valves.Count)
                {
                    actualProcessedChannel = 0;
                }
                if (valves[actualProcessedChannel].IsStarted)
                {
                    break;
                }
                if (actualProcessedChannel == previousProcessedChannel)
                {
                    TestingFinished?.Invoke(this, EventArgs.Empty);
                    break;
                }
            }
            valves[actualProcessedChannel].QueryFinished = false;
        }//
        private void DigitalIO_InputsRead(object sender, string errorCode, byte[] buffer)
        {
            foreach (Valve valve in valves)
            {
                valve.IsInflated = (buffer[0] & (1 << valve.ChannelNumber * 2)) != 0;
                valve.IsDeflated = (buffer[0] & (1 << valve.ChannelNumber * 2 + 1)) != 0;
            }
        }
    }
}
