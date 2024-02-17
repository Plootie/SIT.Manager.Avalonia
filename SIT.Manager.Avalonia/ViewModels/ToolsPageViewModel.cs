using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using SIT.Manager.Avalonia.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.ViewModels
{
    public class ToolsPageViewModel : ViewModelBase
    {
        private readonly IManagerConfigService _configService;
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;

        public IAsyncRelayCommand InstallSITCommand { get; }

        public IAsyncRelayCommand OpenEFTFolderCommand { get; }

        public IAsyncRelayCommand OpenBepInExFolderCommand { get; }

        public IAsyncRelayCommand OpenSITConfigCommand { get; }

        public IAsyncRelayCommand InstallServerCommand { get; }

        public IAsyncRelayCommand OpenEFTLogCommand { get; }

        public IAsyncRelayCommand OpenLocationEditorCommand { get; }

        public IAsyncRelayCommand ClearCacheCommand { get; }

        public ToolsPageViewModel(IManagerConfigService configService, IDirectoryService directoryService, IFileService fileService) {
            _configService = configService;
            _directoryService = directoryService;
            _fileService = fileService;

            OpenEFTFolderCommand = new AsyncRelayCommand(OpenETFFolder);
            OpenBepInExFolderCommand = new AsyncRelayCommand(OpenBepInExFolder);
            OpenSITConfigCommand = new AsyncRelayCommand(OpenSITConfig);
            OpenEFTLogCommand = new AsyncRelayCommand(OpenEFTLog);
        }

        private async Task OpenETFFolder() {
            if (string.IsNullOrEmpty(_configService.Config.InstallPath)) {
                ContentDialog contentDialog = new() {
                    Title = "Config Error",
                    Content = "'Install Path' is not configured. Go to settings to configure the installation path.",
                    CloseButtonText = "Ok"
                };
                await contentDialog.ShowAsync();
            }
            else {
                await _directoryService.OpenDirectoryAsync(_configService.Config.InstallPath);
            }
        }

        private async Task OpenBepInExFolder() {
            if (string.IsNullOrEmpty(_configService.Config.InstallPath)) {
                ContentDialog contentDialog = new() {
                    Title = "Config Error",
                    Content = "'Install Path' is not configured. Go to settings to configure the installation path.",
                    CloseButtonText = "Ok"
                };
                await contentDialog.ShowAsync();
            }
            else {
                string dirPath = Path.Combine(_configService.Config.InstallPath, "BepInEx", "plugins");
                if (Directory.Exists(dirPath)) {
                    await _directoryService.OpenDirectoryAsync(dirPath);
                }
                else {
                    ContentDialog contentDialog = new() {
                        Title = "Config Error",
                        Content = $"Could not find the given path. Is BepInEx installed?\nPath: {dirPath}",
                        CloseButtonText = "Ok"
                    };
                    await contentDialog.ShowAsync();
                }
            }
        }

        private async Task OpenSITConfig() {
            string path = Path.Combine(_configService.Config.InstallPath, "BepInEx", "config");
            string sitCfg = @"SIT.Core.cfg";

            // Different versions of SIT has different names
            if (!File.Exists(Path.Combine(path, sitCfg))) {
                sitCfg = "com.sit.core.cfg";
            }

            if (!File.Exists(Path.Combine(path, sitCfg))) {
                ContentDialog contentDialog = new() {
                    Title = "Config Error",
                    Content = $"Could not find '{sitCfg}'. Make sure SIT is installed and that you have started the game once.",
                    CloseButtonText = "Ok"
                };
                await contentDialog.ShowAsync();
            }
            else {
                await _fileService.OpenFileAsync(Path.Combine(path, sitCfg));
            }
        }

        private async Task OpenEFTLog() {
            // TODO fix this for linux :)
            string logPath = @"%userprofile%\AppData\LocalLow\Battlestate Games\EscapeFromTarkov\Player.log";
            logPath = Environment.ExpandEnvironmentVariables(logPath);
            if (File.Exists(logPath)) {
                await _fileService.OpenFileAsync(logPath);
            }
            else {
                ContentDialog contentDialog = new() {
                    Title = "Config Error",
                    Content = "Log file could not be found.",
                    CloseButtonText = "Ok"
                };
                await contentDialog.ShowAsync();
            }
        }
    }
}
