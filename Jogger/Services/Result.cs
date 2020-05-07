using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Services
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
