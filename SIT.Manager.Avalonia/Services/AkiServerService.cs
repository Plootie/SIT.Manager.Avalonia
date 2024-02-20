using SIT.Manager.Avalonia.Interfaces;
using SIT.Manager.Avalonia.ManagedProcess;
using SIT.Manager.Avalonia.Models;
using SIT.Manager.Avalonia.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Services
{
    public class AkiServerService(IBarNotificationService barNotificationService,
                                  IManagerConfigService configService) : ManagedProcess.ManagedProcess(barNotificationService, configService), IAkiServerService
    {
        private const string SERVER_EXE = "Aki.Server.exe";
        protected override string EXECUTABLE_NAME => SERVER_EXE;
        public override string ExecutableDirectory => !string.IsNullOrEmpty(_configService.Config.AkiServerPath) ? _configService.Config.AkiServerPath : string.Empty;
        public event EventHandler<DataReceivedEventArgs>? OutputDataReceived;
        public event EventHandler? ServerStarted;
        public bool IsStarted { get; private set; } = false;

        public override void ClearCache() {
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

        public override async Task Install(GithubRelease selectedVersion) {
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
            startedEventHandler = new DataReceivedEventHandler((sender, e) => {
                if (ServerPageViewModel.ConsoleTextRemoveANSIFilterRegex()
                .Replace(e.Data ?? string.Empty, "")
                .Equals("Server is running, do not close while playing SPT, Happy playing!!", StringComparison.InvariantCultureIgnoreCase)) {
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
