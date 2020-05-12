using Jogger.Drivers;
using Jogger.Receive;
using Jogger.Sequence;
using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Channels;

namespace Jogger.Valves
{
    public class Valve
    {
        static int count = 0;
        public ISequencer Sequencer { get; set; }
        public IReceiver Receiver { get; set; }
        private object driver;
        public int ChannelNumber { get; set; }
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
        public Valve(IDriver driver)
        {
            this.driver = driver;
            ChannelNumber = count;
            count++;
        }
    }
}
