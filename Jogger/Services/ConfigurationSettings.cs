using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Services
{
    public class ConfigurationSettings
    {
        static ConfigurationSettings actualSettings;
        public int HardwareChannelCount { get; set; } = 3;
        public static ConfigurationSettings GetActual()
        {
            if (actualSettings is null)
            {
                actualSettings = new ConfigurationSettings();
            }
            return actualSettings;
        }
    }

}
