using SIT.Manager.Avalonia.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static SIT.Manager.Avalonia.Services.AkiServerService;

namespace SIT.Manager.Avalonia.Services
{
    public interface IAkiServerService
    {
        string ServerFilePath { get; }
        RunningState State { get; }

        event EventHandler<DataReceivedEventArgs>? OutputDataReceived;
        event EventHandler<RunningState>? RunningStateChanged;

        /// <summary>
        /// Clear the server's cache
        /// </summary>
        void ClearCache();
        /// <summary>
        /// Installs the selected SPT Server version
        /// </summary>
        /// <param name="selectedVersion">The <see cref="GithubRelease"/> to install</param>
        Task Install(GithubRelease selectedVersion);
        bool IsUnhandledInstanceRunning();
        void Start();
        void Stop();
    }
}
