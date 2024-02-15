using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.ManagedProcess
{
    public interface IManagedProcess
    {
        string ExecutableFilePath { get; }
        RunningState State { get; }
        event EventHandler<RunningState>? RunningStateChanged;
        void Stop();
    }

    public enum RunningState
    {
        NotRunning,
        Running,
        StoppedUnexpectedly
    }
}
