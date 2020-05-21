using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Models
{
    public class ConfigurationSettings
    {
        public int HardwareChannelCount { get; set; } = 3;
        public int Baudrate { get; set; } = 19200;
    }

}
