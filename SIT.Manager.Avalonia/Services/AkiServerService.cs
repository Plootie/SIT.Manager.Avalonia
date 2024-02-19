using SIT.Manager.Avalonia.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

        private readonly IBarNotificationService _barNotificationService;
        private readonly IManagerConfigService _configService;

        private Process? _serverProcess;
        private bool _stopRequest = false;

        private string ServerDirectory => !string.IsNullOrEmpty(_configService.Config.AkiServerPath) ? _configService.Config.AkiServerPath : string.Empty;

        public string ServerFilePath => !string.IsNullOrEmpty(ServerDirectory) ? Path.Combine(ServerDirectory, SERVER_EXE) : string.Empty;

        public RunningState State { get; private set; } = RunningState.NotRunning;

        public event EventHandler<DataReceivedEventArgs>? OutputDataReceived;
        public event EventHandler<RunningState>? RunningStateChanged;

        public AkiServerService(IBarNotificationService barNotificationService, IManagerConfigService configService) {
            _barNotificationService = barNotificationService;
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

        public void ClearCache() {
            string serverPath = _configService.Config.AkiServerPath;
            if (!string.IsNullOrEmpty(serverPath)) {
                // Combine the serverPath with the additional subpath.
                string serverCachePath = Path.Combine(serverPath, "user", "cache");

                foreach (string file in Directory.GetFiles(serverCachePath)) {
                    File.Delete(file);
                }

                foreach (string subDirectory in Directory.GetDirectories(serverCachePath)) {
                    Directory.Delete(subDirectory, true);
                }
            }
        }

        public async Task Install(GithubRelease selectedVersion) {
            if (string.IsNullOrEmpty(_configService.Config.InstallPath)) {
                _barNotificationService.ShowError("Error", "Install Path is not set. Configure it in Settings.");
                return;
            }

            if (selectedVersion == null) {
                // TODO Loggy.LogToFile("Install Server: selectVersion is 'null'");
                return;
            }

            try {
                bool patcherResult = true;
                if (_configService.Config.TarkovVersion != selectedVersion.body) {
                    // TODO patcherResult = await DownloadAndRunPatcher(selectedVersion.body);
                    // TODO CheckEFTVersion(App.ManagerConfig.InstallPath);
                }

                // We don't use index as they might be different from version to version
                string? releaseZipUrl = selectedVersion.assets.Find(q => q.name == "SPT-AKI-with-SITCoop.zip")?.browser_download_url;

                // Navigate one level up from InstallPath
                string baseDirectory = Directory.GetParent(_configService.Config.InstallPath)?.FullName ?? string.Empty;

                // Define the target directory for SIT-Server within the parent directory
                string sitServerDirectory = Path.Combine(baseDirectory, "SIT-Server");

                Directory.CreateDirectory(sitServerDirectory);

                // Define the paths for download and extraction based on the SIT-Server directory
                string downloadLocation = Path.Combine(sitServerDirectory, "SPT-AKI-with-SITCoop.zip");
                string extractionPath = sitServerDirectory;

                // Download and extract the file in SIT-Server directory
                // TODO await DownloadFile("SPT-AKI-with-SITCoop.zip", sitServerDirectory, releaseZipUrl, true);
                // TODO ExtractArchive(downloadLocation, extractionPath);

                // Remove the downloaded SIT-Server after extraction
                File.Delete(downloadLocation);

                // TODO CheckSITVersion(App.ManagerConfig.InstallPath);

                // Attempt to automatically set the AKI Server Path after successful installation
                if (!string.IsNullOrEmpty(sitServerDirectory)) {
                    var config = _configService.Config;
                    config.AkiServerPath = sitServerDirectory;
                    _configService.UpdateConfig(config);
                    _configService.Save();
                    _barNotificationService.ShowSuccess("Config", $"Server installation path automatically set to '{sitServerDirectory}'");
                }
                else {
                    // Optional: Notify user that automatic path detection failed and manual setting is needed
                    _barNotificationService.ShowWarning("Notice", "Automatic Server path detection failed. Please set it manually.");
                }

                _barNotificationService.ShowSuccess("Install", "Installation of Server was succesful.");
            }
            catch (Exception ex) {
                // TODO ShowInfoBarWithLogButton("Install Error", "Encountered an error during installation.", InfoBarSeverity.Error, 10);
                // TODO Loggy.LogToFile("Install Server: " + ex.Message + "\n" + ex);
            }
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
            if (!clsMsgSent)
                _serverProcess.Kill();

            _serverProcess.WaitForExit();
            _serverProcess.Close();
        }
    }
}
