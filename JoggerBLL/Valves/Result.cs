using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Valves
{
    public enum Result
    {
        Unused,
        Idle,
        Testing,
        DoneOk,
        DoneErrorCriticalCode,
        DoneErrorConnection,
        DoneErrorTimeout,
        Stopped
    }
}
