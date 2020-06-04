using Jogger.Valves;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.IO
{
    class ValvesWiring : IWiringIO
    {
        private IDigitalIO digitalIO;
        private IValveManager valveManager;
        public ValvesWiring(IDigitalIO digitalIO, IValveManager valveManager)
        {
            this.digitalIO = digitalIO;
            this.valveManager = valveManager;
            digitalIO.InputsRead += DigitalIO_InputsRead;
        }
        private void DigitalIO_InputsRead(object sender, string errorCode, byte[] buffer)
        {
            for (int i = 0; i < (valveManager.GetNumberOfValves()); i++)
            {
                bool isInflated = (buffer[0] & (1 << i * 2)) != 0;
                bool isDeflated = (buffer[0] & (1 << i * 2)) != 0;
                valveManager.SetValveSensorsState(i, isInflated, isDeflated);
            }
        }
    }
}
