using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using static Jogger.IO.IDigitalIO;

namespace Jogger.IO
{

    public class DigitalIOStub : IDigitalIO
    {
        public ActionStatus Status { get; set; }
        public string ReadData { get; set; }
        public byte[] ResultData { get; set; }

        public event InputsReadEventHandler InputsRead;
        public event CommunicationLogEventHandler CommunicationLogChanged;

        public void Dispose()
        {
        }

        public Task<(string, byte[])> ReadInputs()
        {
            (string readInputs, byte[] result) tuple = (ReadData, ResultData);
            InputsRead?.Invoke(this, ReadData, ResultData);
            return Task.FromResult(tuple);
        }

        public ActionStatus Initialize()
        {
            return Status;
        }
    }
}
