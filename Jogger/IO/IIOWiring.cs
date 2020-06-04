using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Jogger.IO
{
    public interface IIOWiring
    {
        bool GetInflateInput(int channelNumber, byte [] data);
        bool GetDeflateInput(int channelNumber, byte[] data);
    }
}
