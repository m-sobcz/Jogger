using Jogger.Drivers;
using Jogger.IO;
using Jogger.Models;
using Jogger.Receive;
using Jogger.Sequence;
using Jogger.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using static Jogger.Receive.Receiver;

namespace Jogger.Valves
{
    public class ValveManager : IValveManager
    {
        int actualProcessedChannel = 0;
        int previousProcessedChannel = 0;
        public List<Valve> valves = new List<Valve>();
        readonly static string namespacePrefix;
        private IDriver driver;
        IDigitalIO digitalIO;
        private TestSettings testSettings;
        public bool IsTestingDone { get; set; } = false;
        public event CommunicationLogEventHandler CommunicationLogChanged;
        public delegate void CommunicationLogEventHandler(object sender, string log);
        public event ErrorsEventHandler ActiveErrorsChanged;
        public event ErrorsEventHandler OccuredErrorsChanged;
        public event ResultEventHandler ResultChanged;
        public delegate void ResultEventHandler(object sender, Result result, int channelNumber);
        static ValveManager()
        {
            namespacePrefix = System.Reflection.Assembly.GetExecutingAssembly().EntryPoint.DeclaringType.Namespace;
            namespacePrefix += ".";
        }
        public ValveManager(IDigitalIO digitalIO, IDriver driver)
        {
            this.driver = driver;
            this.digitalIO = digitalIO;
            digitalIO.InputsRead += DigitalIO_InputsRead;
        }
        public ActionStatus Initialize(int channelsCount)
        {
            for (int i = 0; i < channelsCount; i++)
            {
                valves.Add(new Valve(driver));//TOD
                valves[i].OccuredErrorsChanged += Receiver_OccuredErrorsChanged;
                valves[i].ActiveErrorsChanged += Receiver_ActiveErrorsChanged;
            }
            return ActionStatus.OK;
        }
        public ActionStatus Start(TestSettings testSettings, string valveType)
        {
            ActionStatus actionStatus = ActionStatus.OK;
            this.testSettings = testSettings;
            Valve.testSettings = testSettings;
            foreach (Valve valve in valves)
            {
                valve.Start();
            }
            IsTestingDone = false;
            return actionStatus;
        }
        public void Stop()
        {
            foreach (Valve valve in valves)
            {
                valve.IsStopRequested = true;
            }
        }
        public async Task SendData()
        {
            if (valves[actualProcessedChannel].IsStarted)
            {
                string dataToDriver = await valves[actualProcessedChannel].ExecuteStep(driver.MasterMask[actualProcessedChannel]);
                if (Valve.testSettings.IsLogOutDataSelected)
                {
                    CommunicationLogChanged?.Invoke(this, dataToDriver);
                }
            }
        }
        public async Task ReceiveData()
        {
            string dataFromDriver = await valves[actualProcessedChannel].Receive();
            if (testSettings.IsLogInDataSelected)
            {
                CommunicationLogChanged?.Invoke(this, dataFromDriver);
            }
        }
        public void SetNextProcessedChannel()
        {
            previousProcessedChannel = actualProcessedChannel;
            while (true)
            {
                actualProcessedChannel++;
                if (actualProcessedChannel >= valves.Count)
                {
                    actualProcessedChannel = 0;
                }
                if (valves[actualProcessedChannel].IsStarted |
                    actualProcessedChannel == previousProcessedChannel)
                {
                    break;
                }
            }
        }
        private void Receiver_ActiveErrorsChanged(object sender, string errors, int channelNumber)
        {
            ActiveErrorsChanged?.Invoke(sender, errors, channelNumber);
        }
        private void Receiver_OccuredErrorsChanged(object sender, string errors, int channelNumber)
        {
            OccuredErrorsChanged?.Invoke(sender, errors, channelNumber);
        }
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
