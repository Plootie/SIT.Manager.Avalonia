using System;
using System.Diagnostics;
using static SIT.Manager.Avalonia.Services.AkiServerService;

namespace SIT.Manager.Avalonia.Services
{
    public interface IAkiServerService
    {
        string ServerFilePath { get; }
        RunningState State { get; }

        event EventHandler<DataReceivedEventArgs>? OutputDataReceived;
        event EventHandler<RunningState>? RunningStateChanged;

        bool IsUnhandledInstanceRunning();
        void Start();
        void Stop();
    }
}
