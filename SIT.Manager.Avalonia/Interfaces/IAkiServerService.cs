using System;
using System.Diagnostics;
using static SIT.Manager.Avalonia.Services.AkiServerService;

namespace SIT.Manager.Avalonia.Interfaces
{
    public interface IAkiServerService : IManagedProcess
    {
        event EventHandler<DataReceivedEventArgs>? OutputDataReceived;
        bool IsUnhandledInstanceRunning();
        void Start();
    }
}
