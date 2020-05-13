using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Models
{
    public class TestSettings
    {

        public bool IsLogInDataSelected { get; set; } = false;
        public bool IsLogOutDataSelected { get; set; } = false;
        public bool IsLogTimeoutSelected { get; set; } = false;

        public int Repetitions { get; set; } = 3;
        public int ValveMinInflateTime { get; set; } = 200;
        public int ValveMinDeflateTime { get; set; } = 200;
        public int ValveMaxDeflateTime { get; set; } = 3000;
        public int ValveMaxInflateTime { get; set; } = 3000;
        


    }
}
