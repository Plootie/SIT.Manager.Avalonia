using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using SIT.Manager.Avalonia.ManagedProcess;
using SIT.Manager.Avalonia.Models;
using SIT.Manager.Avalonia.ViewModels;

namespace SIT.Manager.Avalonia.Services
{
    public class AkiServerService(IManagerConfigService configService) : ManagedProcess.ManagedProcess(configService), IAkiServerService
    {
        private const string SERVER_EXE = "Aki.Server.exe";
        protected override string EXECUTABLE_NAME => SERVER_EXE;
        public override string ExecutableDirectory => !string.IsNullOrEmpty(_configService.Config.AkiServerPath) ? _configService.Config.AkiServerPath : string.Empty;
        public event EventHandler<DataReceivedEventArgs>? OutputDataReceived;
        public event EventHandler? ServerStarted;
        public bool IsStarted { get; private set; } = false;
        public TarkovEdition[] TarkovEditions { get; private set; } = [];
        public bool IsUnhandledInstanceRunning() {
            Process[] akiServerProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(SERVER_EXE));

            if (akiServerProcesses.Length > 0) {
                if (_process == null || _process.HasExited) {
                    return true;
                }

                foreach (Process akiServerProcess in akiServerProcesses) {
                    if (_process.Id != akiServerProcess.Id) {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void Start(string? arguments = null) {
            if (State == RunningState.Running) {
                return;
            }

            _process = new Process() {
                StartInfo = new ProcessStartInfo() {
                    FileName = ExecutableFilePath,
                    WorkingDirectory = ExecutableDirectory,
                    UseShellExecute = false,
                    StandardOutputEncoding = Encoding.UTF8,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            _process.OutputDataReceived += new DataReceivedEventHandler((sender, e) => OutputDataReceived?.Invoke(sender, e));
            DataReceivedEventHandler? startedEventHandler = null;
            startedEventHandler = new DataReceivedEventHandler((sender, e) =>
            {
                if (ServerPageViewModel.ConsoleTextRemoveANSIFilterRegex()
                .Replace(e.Data ?? string.Empty, "")
                .Equals("Server is running, do not close while playing SPT, Happy playing!!", StringComparison.InvariantCultureIgnoreCase))
                {
                    IsStarted = true;
                    ServerStarted?.Invoke(sender, e);
                    _process.OutputDataReceived -= startedEventHandler;
                }
            });
            _process.OutputDataReceived += startedEventHandler;
            _process.Exited += new EventHandler((sender, e) => ExitedEvent(sender, e));

            _process.Start();
            _process.BeginOutputReadLine();

            UpdateRunningState(RunningState.Running);
        }
    }
}
