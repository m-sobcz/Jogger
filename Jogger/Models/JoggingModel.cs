using Jogger.Services;
using Jogger.Valves;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Models
{
    class JoggingModel : ObservedObject
    {
        public Result[] results;
        public string[] activeErrors;
        public string[] occuredErrors;
        public string communicationLog;

        public JoggingModel()
        {
            results = new Result[4];
            activeErrors = new string[4];
            occuredErrors = new string[4];
            communicationLog = "";
        }

    }
}
