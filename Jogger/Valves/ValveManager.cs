using Jogger.Drivers;
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

        public List<IValve> valves = new List<IValve>();
        readonly Func<IValve> getValve;
        public TestSettings TestSettings { get; set; }
        public int PreviousProcessedValve { get; set; }
        public int ActualProcessedValve { get; set; }

        public int GetNumberOfValves() => valves.Count;
        public event CommunicationLogEventHandler CommunicationLogChanged;
        public event ErrorsEventHandler ActiveErrorsChanged;
        public event ErrorsEventHandler OccuredErrorsChanged;
        public event ResultEventHandler ResultChanged;
        public event EventHandler TestingFinished;
        public ValveManager(TestSettings testSettings, Func<IValve> getValve)
        {
            this.getValve = getValve;
            this.TestSettings = testSettings;
        }
        public ActionStatus Initialize(int channelsCount)
        {
            for (int i = 0; i < channelsCount; i++)
            {
                valves.Add(getValve());
                valves[i].ResultChanged += (object sender, Result result, int channelNumber) => CheckTestingDone();
                //TODO - move out - direct wiring
                valves[i].OccuredErrorsChanged +=
    (object sender, string errors, int channelNumber) => OccuredErrorsChanged?.Invoke(sender, errors, channelNumber);
                valves[i].ActiveErrorsChanged +=
                    (object sender, string errors, int channelNumber) => ActiveErrorsChanged?.Invoke(sender, errors, channelNumber);
                valves[i].ResultChanged +=
                   (object sender, Result result, int channelNumber) => ResultChanged?.Invoke(sender, result, channelNumber);
            }
            return ActionStatus.OK;
        }

        public ActionStatus Start(TestSettings testSettings, string valveTypeTxt)
        {
            if (valveTypeTxt is null) return ActionStatus.Error;
            this.TestSettings = testSettings;
            foreach (IValve valve in valves)
            {
                IValveType valveType = GetValveType(valveTypeTxt);
                if (valveType is null) return ActionStatus.Error;
                valve.Start(valveType);
            }
            return ActionStatus.OK;
        }

        public ActionStatus Stop()
        {
            foreach (IValve valve in valves)
            {
                valve.IsStopRequested = true;
            }
            return ActionStatus.OK;
        }
        public async Task<bool> Send()
        {
            string dataToDriver = await valves[ActualProcessedValve].ExecuteStep();
            if (TestSettings.IsLogOutDataSelected & dataToDriver != null)
            {
                CommunicationLogChanged?.Invoke(this, dataToDriver + "\n");
            }
            return true;
        }
        public async Task<bool> Receive()
        {
            string dataFromDriver = await valves[ActualProcessedValve].Receive();
            if (TestSettings.IsLogInDataSelected & (TestSettings.IsLogTimeoutSelected | !dataFromDriver.Equals("Timeout")))
            {
                CommunicationLogChanged?.Invoke(this, dataFromDriver + "\n");
            }
            return true;
        }
        public void SetNextProcessedValve()
        {
            PreviousProcessedValve = ActualProcessedValve;
            while (true)
            {
                ActualProcessedValve++;
                if (ActualProcessedValve >= valves.Count)
                {
                    ActualProcessedValve = 0;
                }
                if (valves[ActualProcessedValve].IsStarted)
                {
                    break;
                }
                if (ActualProcessedValve == PreviousProcessedValve)
                {
                    TestingFinished?.Invoke(this, EventArgs.Empty);
                    break;
                }
            }
            valves[ActualProcessedValve].QueryFinished = false;
        }
        public bool SetValveSensorsState(int valveNumber, bool isInflated, bool isDeflated)
        {
            if (valveNumber < valves.Count)
            {
                valves[valveNumber].IsInflated = isInflated;
                valves[valveNumber].IsDeflated = isDeflated;
                return true;
            }
            else return false;
        }
        IValveType GetValveType(string valveTypeTxt)
        {
            string namespacePrefix = System.Reflection.Assembly.GetExecutingAssembly().EntryPoint.DeclaringType.Namespace;
            Type valveType = Type.GetType(namespacePrefix + ".ValveTypes.ValveType" + valveTypeTxt, false);
            if (valveType is null) return null;
            return Activator.CreateInstance(valveType) as IValveType;
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
        
        
    }
}
