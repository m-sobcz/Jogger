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
        public ActionStatus Status { get; set; }
        public string ReadData { get; set; }
        public byte[] ResultData { get; set; }
        public void Dispose()
        {
        }

        public Task<(string, byte[])> ReadInputs()
        {
            (string readInputs, byte[] result) tuple = (ReadData, ResultData);
            return Task.FromResult(tuple);
        }

        public ActionStatus Initialize()
        {
            return Status;
        }
    }
}
