using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Valve
{
    public class Valve
    {
        readonly static string[] valveTypes = { "", "2Up" };
        readonly static string namespacePrefix;
        public ISequencer Sequencer { get; set; }
        public IReceiver Receiver { get; set; }
        public int ChannelNumber { get; set; }
        static int count;
        public bool IsDeflateSensorOn
        {
            get { return Sequencer.IsDeflated; }
            set { Sequencer.IsDeflated = value; }
        }
        public bool IsInflateSensorOn
        {
            get { return Sequencer.IsInflated; }
            set { Sequencer.IsInflated = value; }
        }
        public Result result = Result.Idle;
        static Valve()
        {
            namespacePrefix = System.Reflection.Assembly.GetExecutingAssembly().EntryPoint.DeclaringType.Namespace;
            namespacePrefix += ".";
        }
        public void SetSensorsState(byte[] ioResult)
        {
            IsInflateSensorOn = (ioResult[0] & (1 << ChannelNumber * 2)) != 0;
            IsDeflateSensorOn = (ioResult[0] & (1 << ChannelNumber * 2 + 1)) != 0;
        }
        public Valve()
        {
            this.ChannelNumber = count;
            count++;
        }

    }
}
