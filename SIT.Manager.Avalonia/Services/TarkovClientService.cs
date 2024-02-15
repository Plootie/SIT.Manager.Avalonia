using Avalonia.Controls.ApplicationLifetimes;
using SIT.Manager.Avalonia.ManagedProcess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Services
{
    public class TarkovClientService(IManagerConfigService configService) : ITarkovClientService
    {
        private const string CLIENT_EXE = "EscapeFromTarkov.exe";
        private readonly IManagerConfigService _configService = configService;
        private Process? _gameProcess;
        private bool _stopRequest = false;

        public string GameDirectory => !string.IsNullOrEmpty(_configService.Config.InstallPath) ? _configService.Config.InstallPath : string.Empty;
        public string ExecutableFilePath => !string.IsNullOrEmpty(GameDirectory) ? Path.Combine(GameDirectory, CLIENT_EXE) : string.Empty;

        public RunningState State { get; private set; } = RunningState.NotRunning;

        public event EventHandler<RunningState>? RunningStateChanged;
        private void ExitedEvent(object? sender, EventArgs e)
        {
            if (State == RunningState.Running && !_stopRequest)
            {
                State = RunningState.StoppedUnexpectedly;
            }
            else
            {
                State = RunningState.NotRunning;
            }

            _stopRequest = false;
            RunningStateChanged?.Invoke(this, State);
        }

        public void Start(string token, string address)
        {
            _gameProcess = new Process()
            {
                StartInfo = new(ExecutableFilePath)
                {
                    WorkingDirectory = GameDirectory,
                    UseShellExecute = true,
                    ArgumentList =
                    {
                        $"-token={token}",
                        //TODO: Replace this with a struct that gets serialized
                        $"-config={{\"BackendUrl\":\"{address}\",\"Version\":\"live\"}}"
                    }
                },
                EnableRaisingEvents = true,
            };
            _gameProcess.Exited += new EventHandler((sender, e) => ExitedEvent(sender, e));

            if (_configService.Config.CloseAfterLaunch)
            {
                IApplicationLifetime? lifetime = App.Current?.ApplicationLifetime;
                if (lifetime != null && lifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
                {
                    desktopLifetime.Shutdown();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }

        public void Stop()
        {
            if (State == RunningState.NotRunning || _gameProcess == null || _gameProcess.HasExited)
            {
                return;
            }

            _stopRequest = true;

            // Stop the server process
            bool clsMsgSent = _gameProcess.CloseMainWindow();
            if (!clsMsgSent)
                _gameProcess.Kill();

            _gameProcess.WaitForExit();
            _gameProcess.Close();
        }
    }
}
