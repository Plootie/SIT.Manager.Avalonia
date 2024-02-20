using Avalonia.Controls.ApplicationLifetimes;
using SIT.Manager.Avalonia.Interfaces;
using SIT.Manager.Avalonia.ManagedProcess;
using SIT.Manager.Avalonia.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Services
{
    public class TarkovClientService(IBarNotificationService barNotificationService,
                                     IManagerConfigService configService) : ManagedProcess.ManagedProcess(barNotificationService, configService), ITarkovClientService
    {
        private const string TARKOV_EXE = "EscapeFromTarkov.exe";
        public override string ExecutableDirectory => !string.IsNullOrEmpty(_configService.Config.InstallPath) ? _configService.Config.InstallPath : string.Empty;

        protected override string EXECUTABLE_NAME => TARKOV_EXE;

        private void ClearModCache() {
            string cachePath = _configService.Config.InstallPath;
            if (!string.IsNullOrEmpty(cachePath) && Directory.Exists(cachePath)) {
                // Combine the installPath with the additional subpath.
                cachePath = Path.Combine(cachePath, "BepInEx", "cache");

                // Clear both EFT local cache and additional path.
                foreach (string file in Directory.GetFiles(cachePath)) {
                    File.Delete(file);
                }

                foreach (string subDirectory in Directory.GetDirectories(cachePath)) {
                    Directory.Delete(subDirectory, true);
                }

                // Optionally, display a success message or perform additional actions.
                _barNotificationService.ShowInformational("Cache Cleared", "Everything cleared successfully!");
            }
            else {
                // Handle the case where InstallPath is not found or empty.
                _barNotificationService.ShowError("Cache Clear Error", "InstallPath not found in settings");
            }
        }


        public override void ClearCache() {
            ClearLocalCache();
            ClearModCache();
        }

        public void ClearLocalCache() {
            string eftCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp", "Battlestate Games", "EscapeFromTarkov");

            // Check if the directory exists.
            if (Directory.Exists(eftCachePath)) {
                // Delete all files within the directory.
                foreach (string file in Directory.GetFiles(eftCachePath)) {
                    File.Delete(file);
                }

                // Delete all subdirectories and their contents.
                foreach (string subDirectory in Directory.GetDirectories(eftCachePath)) {
                    Directory.Delete(subDirectory, true);
                }

                // Optionally, display a success message or perform additional actions.
                _barNotificationService.ShowInformational("Cache Cleared", "EFT local cache cleared successfully!");
            }
            else {
                // Handle the case where the cache directory does not exist.
                _barNotificationService.ShowWarning("Cache Clear Error", $"EFT local cache directory not found at: {eftCachePath}");
            }
        }

        public override async Task Install(GithubRelease selectedVersion) {
            /* TODO
             var window = App.m_window as MainWindow;
            DispatcherQueue mainQueue = window.DispatcherQueue;

            if (string.IsNullOrEmpty(App.ManagerConfig.InstallPath))
            {
                Utils.ShowInfoBar("Error", "Install Path is not set. Configure it in Settings.", InfoBarSeverity.Error);
                return;
            }

            try
            {
                if (selectedVersion == null)
                {
                    Loggy.LogToFile("InstallSIT: selectVersion is 'null'");
                    return;
                }

                if (File.Exists(App.ManagerConfig.InstallPath + @"\EscapeFromTarkov_BE.exe"))
                {
                    await CleanUpEFTDirectory();
                }

                if (File.Exists(App.ManagerConfig.InstallPath + @"\SITLauncher\CoreFiles\StayInTarkov-Release.zip"))
                    File.Delete(App.ManagerConfig.InstallPath + @"\SITLauncher\CoreFiles\StayInTarkov-Release.zip");


                if (App.ManagerConfig.TarkovVersion != selectedVersion.body)
                {
                    await Task.Run(() => DownloadAndRunPatcher(selectedVersion.body));
                    CheckEFTVersion(App.ManagerConfig.InstallPath);
                }

                if (!Directory.Exists(App.ManagerConfig.InstallPath + @"\SITLauncher\CoreFiles"))
                    Directory.CreateDirectory(App.ManagerConfig.InstallPath + @"\SITLauncher\CoreFiles");

                if (!Directory.Exists(App.ManagerConfig.InstallPath + @"\SITLauncher\Backup\CoreFiles"))
                    Directory.CreateDirectory(App.ManagerConfig.InstallPath + @"\SITLauncher\Backup\CoreFiles");

                if (!Directory.Exists(App.ManagerConfig.InstallPath + @"\BepInEx\plugins"))
                {
                    await DownloadFile("BepInEx5.zip", App.ManagerConfig.InstallPath + @"\SITLauncher", "https://github.com/BepInEx/BepInEx/releases/download/v5.4.22/BepInEx_x64_5.4.22.0.zip", true);
                    ExtractArchive(App.ManagerConfig.InstallPath + @"\SITLauncher\BepInEx5.zip", App.ManagerConfig.InstallPath);
                    Directory.CreateDirectory(App.ManagerConfig.InstallPath + @"\BepInEx\plugins");
                }

                //We don't use index as they might be different from version to version
                string releaseZipUrl = selectedVersion.assets.Find(q => q.name == "StayInTarkov-Release.zip").browser_download_url;

                await DownloadFile("StayInTarkov-Release.zip", App.ManagerConfig.InstallPath + @"\SITLauncher\CoreFiles", releaseZipUrl, true);

                ExtractArchive(App.ManagerConfig.InstallPath + @"\SITLauncher\CoreFiles\StayInTarkov-Release.zip", App.ManagerConfig.InstallPath + @"\SITLauncher\CoreFiles\");

                if (File.Exists(App.ManagerConfig.InstallPath + @"\EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll"))
                    File.Copy(App.ManagerConfig.InstallPath + @"\EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll", App.ManagerConfig.InstallPath + @"\SITLauncher\Backup\CoreFiles\Assembly-CSharp.dll", true);
                File.Copy(App.ManagerConfig.InstallPath + @"\SITLauncher\CoreFiles\StayInTarkov-Release\Assembly-CSharp.dll", App.ManagerConfig.InstallPath + @"\EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll", true);

                File.Copy(App.ManagerConfig.InstallPath + @"\SITLauncher\CoreFiles\StayInTarkov-Release\StayInTarkov.dll", App.ManagerConfig.InstallPath + @"\BepInEx\plugins\StayInTarkov.dll", true);

                using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("SIT.Manager.Resources.Aki.Common.dll"))
                {
                    using (var file = new FileStream(App.ManagerConfig.InstallPath + @"\EscapeFromTarkov_Data\Managed\Aki.Common.dll", FileMode.Create, FileAccess.Write))
                    {
                        resource.CopyTo(file);
                    }
                }

                using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("SIT.Manager.Resources.Aki.Reflection.dll"))
                {
                    using (var file = new FileStream(App.ManagerConfig.InstallPath + @"\EscapeFromTarkov_Data\Managed\Aki.Reflection.dll", FileMode.Create, FileAccess.Write))
                    {
                        resource.CopyTo(file);
                    }
                }

                // Run on UI thread to prevent System.InvalidCastException, WinUI bug yikes
                mainQueue.TryEnqueue(() =>
                {
                    CheckSITVersion(App.ManagerConfig.InstallPath);
                });

                ShowInfoBar("Install", "Installation of SIT was succesful.", InfoBarSeverity.Success);
            }
            catch (Exception ex)
            {
                ShowInfoBarWithLogButton("Install Error", "Encountered an error during installation.", InfoBarSeverity.Error, 10);

                Loggy.LogToFile("Install SIT: " + ex.Message + "\n" + ex);

                return;
            }
             */
        }

        public override void Start(string? arguments) {
            _process = new Process() {
                StartInfo = new(ExecutableFilePath) {
                    WorkingDirectory = ExecutableDirectory,
                    UseShellExecute = true,
                    Arguments = arguments
                },
                EnableRaisingEvents = true,
            };
            _process.Exited += new EventHandler((sender, e) => ExitedEvent(sender, e));
            _process.Start();

            if (_configService.Config.CloseAfterLaunch) {
                IApplicationLifetime? lifetime = App.Current?.ApplicationLifetime;
                if (lifetime != null && lifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime) {
                    desktopLifetime.Shutdown();
                }
                else {
                    Environment.Exit(0);
                }
            }
            else {
                UpdateRunningState(RunningState.Running);
            }
        }
    }
}
