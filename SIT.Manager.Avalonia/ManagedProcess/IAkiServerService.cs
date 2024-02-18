using SIT.Manager.Avalonia.Classes;
using System;
using System.Diagnostics;
using static SIT.Manager.Avalonia.Services.AkiServerService;

namespace SIT.Manager.Avalonia.ManagedProcess
{
    public interface IAkiServerService : IManagedProcess
    {
        TarkovEdition[] TarkovEditions { get; }
        event EventHandler<DataReceivedEventArgs>? OutputDataReceived;
        bool IsUnhandledInstanceRunning();
    }
}
