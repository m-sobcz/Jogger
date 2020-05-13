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
        private IDriver driver;
        public event ResultEventHandler ResultChanged;
        public delegate void ResultEventHandler(object sender, Result result, int channelNumber);
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
        private Result result = Result.Idle;
        public Result Result
        {
            get => result;
            set
            {
                result = value;
                ResultChanged?.Invoke(this, result, ChannelNumber);
            }
        }
        public Valve(IDriver driver)
        {
            this.driver = driver;
            ChannelNumber = count;
            count++;
        }
    }
}
