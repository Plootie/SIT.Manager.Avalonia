using SIT.Manager.Avalonia.Models;
using System;
using System.Diagnostics;
using static SIT.Manager.Avalonia.Services.AkiServerService;

namespace SIT.Manager.Avalonia.ManagedProcess
{
    public interface IAkiServerService : IManagedProcess
    {
        TarkovEdition[] TarkovEditions { get; }
        event EventHandler<DataReceivedEventArgs>? OutputDataReceived;
        event EventHandler? ServerStarted;
        public bool IsStarted { get; }
        bool IsUnhandledInstanceRunning();
    }
}
