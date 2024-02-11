using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SIT.Manager.Avalonia.Services
{
    public class AkiServerService : IAkiServerService
    {
        private const string SERVER_EXE = "Aki.Server.exe";

        public enum RunningState
        {
            NotRunning,
            Running,
            StoppedUnexpectedly
        }

        private readonly IManagerConfigService _configService;

        private Process? _serverProcess;
        private bool _stopRequest = false;

        private string ServerDirectory => !string.IsNullOrEmpty(_configService.Config.AkiServerPath) ? _configService.Config.AkiServerPath : string.Empty;

        public string ServerFilePath => !string.IsNullOrEmpty(ServerDirectory) ? Path.Combine(ServerDirectory, SERVER_EXE) : string.Empty;

        public RunningState State { get; private set; } = RunningState.NotRunning;

        public event EventHandler<DataReceivedEventArgs>? OutputDataReceived;
        public event EventHandler<RunningState>? RunningStateChanged;

        public AkiServerService(IManagerConfigService configService) {
            _configService = configService;
        }

        private void ExitedEvent(object? sender, EventArgs e) {
            if (State == RunningState.Running && !_stopRequest) {
                State = RunningState.StoppedUnexpectedly;
            }
            else {
                State = RunningState.NotRunning;
            }

            _stopRequest = false;
            RunningStateChanged?.Invoke(this, State);
        }

        public bool IsUnhandledInstanceRunning() {
            Process[] akiServerProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(SERVER_EXE));

            if (akiServerProcesses.Length > 0) {
                if (_serverProcess == null || _serverProcess.HasExited) {
                    return true;
                }

                foreach (Process akiServerProcess in akiServerProcesses) {
                    if (_serverProcess.Id != akiServerProcess.Id) {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Start() {
            if (State == RunningState.Running) {
                return;
            }

            _serverProcess = new Process() {
                StartInfo = new ProcessStartInfo() {
                    FileName = ServerFilePath,
                    WorkingDirectory = ServerDirectory,
                    UseShellExecute = false,
                    StandardOutputEncoding = Encoding.UTF8,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            _serverProcess.OutputDataReceived += new DataReceivedEventHandler((sender, e) => OutputDataReceived?.Invoke(sender, e));
            _serverProcess.Exited += new EventHandler((sender, e) => ExitedEvent(sender, e));

            _serverProcess.Start();
            _serverProcess.BeginOutputReadLine();

            State = RunningState.Running;

            RunningStateChanged?.Invoke(this, State);
        }

        public void Stop() {
            if (State == RunningState.NotRunning || _serverProcess == null || _serverProcess.HasExited) {
                return;
            }

            _stopRequest = true;

            // Stop the server process
            bool clsMsgSent = _serverProcess.CloseMainWindow();
            if (clsMsgSent)
                _serverProcess.Kill();

            _serverProcess.WaitForExit();
            _serverProcess.Close();
        }
    }
}
