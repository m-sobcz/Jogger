using Jogger.Receive;
using Jogger.Sequence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Text;

namespace Jogger.Valves
{
    public class ValveManager
    {
        public List<Valve> valves = new List<Valve>();
        readonly static string namespacePrefix;
        static ValveManager()
        {
            namespacePrefix = System.Reflection.Assembly.GetExecutingAssembly().EntryPoint.DeclaringType.Namespace;
            namespacePrefix += ".";
        }
        public void Add(Valve valve) 
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
    }
}
