using SIT.Manager.Avalonia.Models;
using System;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.ManagedProcess
{
    public interface IManagedProcess
    {
        string ExecutableDirectory { get; }
        string ExecutableFilePath { get; }
        RunningState State { get; }
        event EventHandler<RunningState>? RunningStateChanged;
        /// <summary>
        /// Clear the cache for the process.
        /// </summary>
        void ClearCache();
        /// <summary>
        /// Installs the selected release for this managed process.
        /// </summary>
        /// <param name="selectedVersion">The <see cref="GithubRelease"/> to install</param>
        Task Install(GithubRelease selectedVersion);
        void Stop();
        void Start(string? arguments = null);
    }

    public enum RunningState
    {
        NotRunning,
        Running,
        StoppedUnexpectedly
    }
}
