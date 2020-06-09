using Jogger.Drivers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Valves
{
    public class ValveDecoder : IValveDecoder
    {
        private IDriver driver;

        public ValveDecoder(IDriver driver)
        {
            this.driver = driver;
        }

    }
}
