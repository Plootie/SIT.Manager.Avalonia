using SIT.Manager.Avalonia.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Services
{
    public class ModService : IModService
    {
        private const string MOD_COLLECTION_URL = "https://github.com/stayintarkov/SIT-Mod-Ports/releases/latest/download/SIT.Mod.Ports.Collection.zip";

        private readonly IFileService _filesService;
        private readonly IManagerConfigService _configService;

        public ModService(IFileService filesService, IManagerConfigService configService) {
            _filesService = filesService;
            _configService = configService;
        }

        public async Task DownloadModsCollection() {
            string modsDirectory = Path.Combine(_configService.Config.InstallPath, "SITLauncher", "Mods");
            if (!Directory.Exists(modsDirectory)) {
                Directory.CreateDirectory(modsDirectory);
            }

            string[] subDirs = Directory.GetDirectories(modsDirectory);
            foreach (string subDir in subDirs) {
                Directory.Delete(subDir, true);
            }
            Directory.CreateDirectory(Path.Combine(modsDirectory, "Extracted"));

            await _filesService.DownloadFile("SIT.Mod.Ports.Collection.zip", modsDirectory, MOD_COLLECTION_URL, true);
            _filesService.ExtractArchive(Path.Combine(modsDirectory, "SIT.Mod.Ports.Collection.zip"), Path.Combine(modsDirectory, "Extracted"));
        }

        public async Task AutoUpdate(List<ModInfo> outdatedMods) {
            /* TODO
            // As this is being run on another thread than the UI we need to fetch the MainWindow
            MainWindow window = App.m_window as MainWindow;

            List<string> outdatedNames = new();
            outdatedNames.AddRange(from ModInfo mod in outdatedMods
                                   select mod.Name);

            string outdatedString = string.Join("\n", outdatedNames);

            ScrollView scrollView = new() {
                Content = new TextBlock() {
                    Text = $"You have {outdatedMods.Count} outdated mods. Would you like to automatically update them?\nOutdated Mods:\n\n{outdatedString}"
                }
            };

            ContentDialog contentDialog = new() {
                XamlRoot = window.Content.XamlRoot,
                Title = "Outdated Mods Found",
                Content = scrollView,
                CloseButtonText = "No",
                IsPrimaryButtonEnabled = true,
                PrimaryButtonText = "Yes"
            };

            ContentDialogResult result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary) {
                foreach (ModInfo mod in outdatedMods) {
                    App.ManagerConfig.InstalledMods.Remove(mod.Name);
                    InstallMod(mod, true);
                }
            }
            else {
                return;
            }

            Utils.ShowInfoBar("Updated Mods", $"Updated {outdatedMods.Count} mods.", InfoBarSeverity.Success);
            */
        }


        public async Task InstallMod(ModInfo mod, bool suppressNotification = false) {
            /* TODO
            if (string.IsNullOrEmpty(App.ManagerConfig.InstallPath)) {
                Utils.ShowInfoBar("Install Mod", "Install Path is not set. Configure it in Settings.", InfoBarSeverity.Error);
                return;
            }

            try {
                if (mod.SupportedVersion != App.ManagerConfig.SitVersion) {
                    ContentDialog contentDialog = new ContentDialog() {
                        XamlRoot = XamlRoot,
                        Title = "Warning",
                        Content = $"The mod you are trying to install is not compatible with your currently installed version of SIT.\n\nSupported SIT Version: {mod.SupportedVersion}\nInstalled SIT Version: {(string.IsNullOrEmpty(App.ManagerConfig.SitVersion) ? "Unknown" : App.ManagerConfig.SitVersion)}\n\nContinue anyway?",
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        IsPrimaryButtonEnabled = true,
                        PrimaryButtonText = "Yes",
                        CloseButtonText = "No"
                    };

                    ContentDialogResult response = await contentDialog.ShowAsync();

                    if (response != ContentDialogResult.Primary)
                        return;
                }

                InstallButton.IsEnabled = false;
                if (mod == null || string.IsNullOrEmpty(App.ManagerConfig.InstallPath))
                    return;

                string installPath = App.ManagerConfig.InstallPath;
                string gamePluginsPath = installPath + @"\BepInEx\plugins\";
                string gameConfigPath = installPath + @"\BepInEx\config\";

                foreach (string pluginFile in mod.PluginFiles) {
                    File.Copy(installPath + @"\SITLauncher\Mods\Extracted\plugins\" + pluginFile, gamePluginsPath + pluginFile, true);
                }

                foreach (var configFile in mod.ConfigFiles) {
                    File.Copy(installPath + @"\SITLauncher\Mods\Extracted\config\" + configFile, gameConfigPath + configFile, true);
                }

                App.ManagerConfig.InstalledMods.Add(mod.Name, mod.PortVersion);
                ManagerConfig.Save();

                if (suppressNotification == false)
                    Utils.ShowInfoBar("Install Mod", $"{mod.Name} was successfully installed.", InfoBarSeverity.Success);
                UninstallButton.IsEnabled = true;
            }
            catch (Exception ex) {
                Loggy.LogToFile("InstallMod: " + ex.Message);
                InstallButton.IsEnabled = true;
                Utils.ShowInfoBar("Install Mod", $"{mod.Name} failed to install. Check your Launcher.log", InfoBarSeverity.Error);
                return;
            }
            */
        }

        public async Task UninstallMod(ModInfo mod) {
            /* TODO
            try {
                UninstallButton.IsEnabled = false;

                if (mod == null || string.IsNullOrEmpty(App.ManagerConfig.InstallPath))
                    return;

                string installPath = App.ManagerConfig.InstallPath;
                string gamePluginsPath = installPath + @"\BepInEx\plugins\";
                string gameConfigPath = installPath + @"\BepInEx\config\";

                foreach (string pluginFile in mod.PluginFiles) {
                    if (File.Exists(gamePluginsPath + pluginFile)) {
                        File.Delete(gamePluginsPath + pluginFile);
                    }
                    else {
                        ContentDialog dialog = new() {
                            XamlRoot = XamlRoot,
                            Title = "Error Uninstalling Mod",
                            Content = $"A file was missing from the mod {mod.Name}: '{pluginFile}'\n\nForce remove the mod from the list of installed mods anyway?",
                            CloseButtonText = "No",
                            IsPrimaryButtonEnabled = true,
                            PrimaryButtonText = "Yes"
                        };

                        ContentDialogResult result = await dialog.ShowAsync();

                        if (result != ContentDialogResult.Primary) {
                            throw new FileNotFoundException($"A file was missing from the mod {mod.Name}: '{pluginFile}'");
                        }
                    }
                }

                foreach (var configFile in mod.ConfigFiles) {
                    if (File.Exists(gameConfigPath + configFile)) {
                        File.Delete(gameConfigPath + configFile);
                    }
                    else {
                        ContentDialog dialog = new() {
                            XamlRoot = XamlRoot,
                            Title = "Error Uninstalling Mod",
                            Content = $"A file was missing from the mod {mod.Name}: '{configFile}'\n\nForce remove the mod from the list of installed mods anyway?",
                            CloseButtonText = "No",
                            IsPrimaryButtonEnabled = true,
                            PrimaryButtonText = "Yes"
                        };

                        ContentDialogResult result = await dialog.ShowAsync();

                        if (result != ContentDialogResult.Primary) {
                            throw new FileNotFoundException($"A file was missing from the mod {mod.Name}: '{configFile}'");
                        }
                    }
                }

                App.ManagerConfig.InstalledMods.Remove(mod.Name);
                ManagerConfig.Save();

                Utils.ShowInfoBar("Uninstall Mod", $"{mod.Name} was successfully uninstalled.", InfoBarSeverity.Success);
                InstallButton.IsEnabled = true;
            }
            catch (Exception ex) {
                Loggy.LogToFile("UninstallMod: " + ex.Message);
                UninstallButton.IsEnabled = true;
                Utils.ShowInfoBar("Uninstall Mod", $"{mod.Name} failed to uninstall. Check your Launcher.log", InfoBarSeverity.Error);
                return;
            }
            */
        }
    }
}
