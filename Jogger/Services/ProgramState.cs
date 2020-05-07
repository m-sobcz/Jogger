using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Services
{
    public enum ProgramState
    {
        NotInitialized,
        Initializing,
        Initialized,
        Idle,
        Starting,
        Started,
        Stopping,
        Done,
        Error
    }
}
