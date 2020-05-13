using Jogger.IO;
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

namespace Jogger.Valves
{
    public class ValveManager : IValveManager
    {
        public List<Valve> valves = new List<Valve>();
        readonly static string namespacePrefix;
        IDigitalIO digitalIO;
        static ValveManager()
        {
            namespacePrefix = System.Reflection.Assembly.GetExecutingAssembly().EntryPoint.DeclaringType.Namespace;
            namespacePrefix += ".";
        }
        public ValveManager(IDigitalIO digitalIO)
        {
            this.digitalIO = digitalIO;
            digitalIO.InputsRead += DigitalIO_InputsRead;
        }
        public ActionStatus Initialize(int channelsCount) 
        {
            for (int i = 0; i < channelsCount; i++)
            {
                valves.Add(App.ServiceProvider.GetRequiredService<Valve>());
                valves[i].Result= Result.Idle;
            }
            return ActionStatus.OK;
        }
        void Add(Valve valve)
        {
            valves.Add(valve);
        }
        public void SetSequencerType(string valveType)
        {
            try
            {
                Type sequencerType = Type.GetType(namespacePrefix + "Sequence." + nameof(Jogger.Sequence.Sequencer) + valveType, true);
                foreach (Valve valve in valves)
                {
                    valve.Sequencer = Activator.CreateInstance(sequencerType) as ISequencer;
                    valve.Sequencer.AddAllTestQueries();
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("SetSequencerType: " + e.Message);
            }
        }
        public void SetReceiverType(string valveType)
        {
            try
            {
                Type receiverType = Type.GetType(namespacePrefix + "Receive." + nameof(Jogger.Receive.Receiver) + valveType, true);
                foreach (Valve valve in valves)
                {
                    valve.Receiver = Activator.CreateInstance(receiverType) as IReceiver;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("SetReceiverType: " + e.Message);
            }
        }
        public void SetSensorsState(byte[] ioResult)
        {
            foreach (Valve valve in valves)
            {
                valve.IsInflateSensorOn = (ioResult[0] & (1 << valve.ChannelNumber * 2)) != 0;
                valve.IsDeflateSensorOn = (ioResult[0] & (1 << valve.ChannelNumber * 2 + 1)) != 0;
            }
        }
        private void DigitalIO_InputsRead(object sender, string errorCode, byte[] buffer)
        {
            foreach (Valve valve in valves)
            {
                valve.IsInflateSensorOn = (buffer[0] & (1 << valve.ChannelNumber * 2)) != 0;
                valve.IsDeflateSensorOn = (buffer[0] & (1 << valve.ChannelNumber * 2 + 1)) != 0;
            }
        }
    }
}
