using SIT.Manager.Avalonia.Interfaces;
using SIT.Manager.Avalonia.ManagedProcess;
using SIT.Manager.Avalonia.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Services
{
    public class InstallerService : IInstallerService
    {
        private readonly IBarNotificationService _barNotificationService;
        private readonly IManagerConfigService _configService;
        private readonly IVersionService _versionService;

        public InstallerService(IBarNotificationService barNotificationService, IManagerConfigService configService, IVersionService versionService) {
            _barNotificationService = barNotificationService;
            _configService = configService;
        }


        public async Task InstallServer(GithubRelease selectedVersion) {
            if (string.IsNullOrEmpty(_configService.Config.InstallPath)) {
                _barNotificationService.ShowError("Error", "Install Path is not set. Configure it in Settings.");
                return;
            }

            if (selectedVersion == null) {
                // TODO Loggy.LogToFile("Install Server: selectVersion is 'null'");
                return;
            }

            ManagerConfig config;
            try {
                bool patcherResult = true;
                if (_configService.Config.TarkovVersion != selectedVersion.body) {
                    // TODO patcherResult = await DownloadAndRunPatcher(selectedVersion.body);

                    string tarkovVersion = _versionService.GetEFTVersion(_configService.Config.InstallPath);

                    config = _configService.Config;
                    config.TarkovVersion = tarkovVersion;
                    _configService.UpdateConfig(config);
                    _configService.Save();
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

                config = _configService.Config;

                string sitVersion = _versionService.GetSITVersion(_configService.Config.InstallPath);
                config.SitVersion = sitVersion;

                // Attempt to automatically set the AKI Server Path after successful installation
                if (!string.IsNullOrEmpty(sitServerDirectory)) {
                    config.AkiServerPath = sitServerDirectory;
                    _barNotificationService.ShowSuccess("Config", $"Server installation path automatically set to '{sitServerDirectory}'");
                }
                else {
                    // Optional: Notify user that automatic path detection failed and manual setting is needed
                    _barNotificationService.ShowWarning("Notice", "Automatic Server path detection failed. Please set it manually.");
                }

                _configService.UpdateConfig(config);
                _configService.Save();

                _barNotificationService.ShowSuccess("Install", "Installation of Server was succesful.");
            }
            catch (Exception ex) {
                // TODO ShowInfoBarWithLogButton("Install Error", "Encountered an error during installation.", InfoBarSeverity.Error, 10);
                // TODO Loggy.LogToFile("Install Server: " + ex.Message + "\n" + ex);
            }
        }


        public async Task InstallSIT(GithubRelease selectedVersion) {
            /* TODO
            var window = App.m_window as MainWindow;
            DispatcherQueue mainQueue = window.DispatcherQueue;

            if (string.IsNullOrEmpty(App.ManagerConfig.InstallPath)) {
                Utils.ShowInfoBar("Error", "Install Path is not set. Configure it in Settings.", InfoBarSeverity.Error);
                return;
            }

            try {
                if (selectedVersion == null) {
                    Loggy.LogToFile("InstallSIT: selectVersion is 'null'");
                    return;
                }

                if (File.Exists(App.ManagerConfig.InstallPath + @"\EscapeFromTarkov_BE.exe")) {
                    await CleanUpEFTDirectory();
                }

                if (File.Exists(App.ManagerConfig.InstallPath + @"\SITLauncher\CoreFiles\StayInTarkov-Release.zip"))
                    File.Delete(App.ManagerConfig.InstallPath + @"\SITLauncher\CoreFiles\StayInTarkov-Release.zip");


                if (App.ManagerConfig.TarkovVersion != selectedVersion.body) {
                    await Task.Run(() => DownloadAndRunPatcher(selectedVersion.body));
                    CheckEFTVersion(App.ManagerConfig.InstallPath);
                }

                if (!Directory.Exists(App.ManagerConfig.InstallPath + @"\SITLauncher\CoreFiles"))
                    Directory.CreateDirectory(App.ManagerConfig.InstallPath + @"\SITLauncher\CoreFiles");

                if (!Directory.Exists(App.ManagerConfig.InstallPath + @"\SITLauncher\Backup\CoreFiles"))
                    Directory.CreateDirectory(App.ManagerConfig.InstallPath + @"\SITLauncher\Backup\CoreFiles");

                if (!Directory.Exists(App.ManagerConfig.InstallPath + @"\BepInEx\plugins")) {
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

                using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("SIT.Manager.Resources.Aki.Common.dll")) {
                    using (var file = new FileStream(App.ManagerConfig.InstallPath + @"\EscapeFromTarkov_Data\Managed\Aki.Common.dll", FileMode.Create, FileAccess.Write)) {
                        resource.CopyTo(file);
                    }
                }

                using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("SIT.Manager.Resources.Aki.Reflection.dll")) {
                    using (var file = new FileStream(App.ManagerConfig.InstallPath + @"\EscapeFromTarkov_Data\Managed\Aki.Reflection.dll", FileMode.Create, FileAccess.Write)) {
                        resource.CopyTo(file);
                    }
                }

                // Run on UI thread to prevent System.InvalidCastException, WinUI bug yikes
                mainQueue.TryEnqueue(() => {
                    CheckSITVersion(App.ManagerConfig.InstallPath);
                });

                ShowInfoBar("Install", "Installation of SIT was succesful.", InfoBarSeverity.Success);
            }
            catch (Exception ex) {
                ShowInfoBarWithLogButton("Install Error", "Encountered an error during installation.", InfoBarSeverity.Error, 10);

                Loggy.LogToFile("Install SIT: " + ex.Message + "\n" + ex);

                return;
            }
            */
        }
    }
}
