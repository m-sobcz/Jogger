using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Jogger.IO
{

    public class DigitalIOStub : IDigitalIO
    {
        private readonly ActionStatus actionStatus;
        readonly string readInputs;
        byte[] result;
            

        public DigitalIOStub(ActionStatus actionStatus, string readInputs, byte[] result )
        {
            this.actionStatus = actionStatus;
            this.readInputs = readInputs;
            this.result = result;
        }
        public void Dispose()
        {
            Trace.WriteLine(nameof(Dispose));
        }

        public Task<(string, byte[])> ReadInputs()
        {
            (string readInputs, byte[] result) tuple = (readInputs, result);
            return Task.FromResult(tuple);
        }

        public ActionStatus Initialize()
        {
            Trace.WriteLine(nameof(Initialize));
            return actionStatus;
        }
    }
}
