using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Text;
using System.Threading.Tasks;
using static Jogger.IO.IDigitalIO;

namespace Jogger.IO
{

    public class DigitalIOStub : IDigitalIO
    {
        public ActionStatus initializationStatus=ActionStatus.OK;

        public ActionStatus Status { get; set; }
        public bool isDisposed = false;

        public event InputsReadEventHandler InputsRead;
        public event CommunicationLogEventHandler CommunicationLogChanged;

        public void Dispose()
        {
            isDisposed = true;        
        }

        public ActionStatus Initialize()
        {
            return initializationStatus; 
        }

        public Task<(string, byte[])> ReadInputs()
        {
            throw new NotImplementedException();
        }
    }
}
