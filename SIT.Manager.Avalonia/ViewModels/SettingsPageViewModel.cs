using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIT.Manager.Avalonia.Interfaces;
using SIT.Manager.Avalonia.ManagedProcess;
using SIT.Manager.Avalonia.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    private readonly IManagerConfigService _configsService;
    private readonly IBarNotificationService _barNotificationService;
    private readonly IDirectoryService _directoryService;
    private readonly IVersionService _versionService;

    [ObservableProperty]
    private bool _closeAfterLaunch;

    [ObservableProperty]
    private bool _lookForUpdates;

    [ObservableProperty]
    private string _managerVersionString;

    [ObservableProperty]
    private string _installPath;

    [ObservableProperty]
    private string _tarkovVersion;

    [ObservableProperty]
    private string _sitVersion;

    [ObservableProperty]
    private string _akiServerPath;

    [ObservableProperty]
    private string _consoleFontFamily;

    [ObservableProperty]
    private Color _consoleFontColor;

    [ObservableProperty]
    private FontFamily _selectedConsoleFontFamily;

    [ObservableProperty]
    private List<FontFamily> _installedFonts;

    public IAsyncRelayCommand ChangeInstallLocationCommand { get; }

    public IAsyncRelayCommand ChangeAkiServerLocationCommand { get; }

    public SettingsPageViewModel(IManagerConfigService configService,
                                 IBarNotificationService barNotificationService,
                                 IDirectoryService directoryService,
                                 IVersionService versionService) {
        _configsService = configService;
        _directoryService = directoryService;
        _barNotificationService = barNotificationService;
        _versionService = versionService;

        _closeAfterLaunch = _configsService.Config.CloseAfterLaunch;
        _lookForUpdates = _configsService.Config.LookForUpdates;
        _installPath = _configsService.Config.InstallPath;
        _tarkovVersion = _configsService.Config.TarkovVersion;
        _sitVersion = _configsService.Config.SitVersion;
        _akiServerPath = _configsService.Config.AkiServerPath;
        _consoleFontFamily = _configsService.Config.ConsoleFontFamily;
        _consoleFontColor = _configsService.Config.ConsoleFontColor;

        List<FontFamily> installedFonts = [.. FontManager.Current.SystemFonts];
        installedFonts.Add(FontFamily.Parse("Bender"));
        _installedFonts = [.. installedFonts.OrderBy(x => x.Name)];

        _selectedConsoleFontFamily = InstalledFonts.FirstOrDefault(x => x.Name == ConsoleFontFamily, FontFamily.Parse("Bender"));

        _managerVersionString = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "N/A";

        ChangeInstallLocationCommand = new AsyncRelayCommand(ChangeInstallLocation);
        ChangeAkiServerLocationCommand = new AsyncRelayCommand(ChangeAkiServerLocation);
    }

    /// <summary>
    /// Gets the path containing the required filename based on the folder picker selection from a user
    /// </summary>
    /// <param name="filename">The filename to look for in the user specified directory</param>
    /// <returns>The path if the file exists, otherwise an empty string</returns>
    private async Task<string> GetPathLocation(string filename) {
        string? directorySelectedPath = await _directoryService.GetDirectoryFromPickerAsync();
        if (!string.IsNullOrEmpty(directorySelectedPath)) {
            if (File.Exists(Path.Combine(directorySelectedPath, filename))) {
                return directorySelectedPath;
            }
        }
        return string.Empty;
    }

    private async Task ChangeInstallLocation() {
        string targetPath = await GetPathLocation("EscapeFromTarkov.exe");
        if (!string.IsNullOrEmpty(targetPath)) {
            InstallPath = targetPath;
            TarkovVersion = _versionService.GetEFTVersion(targetPath);
            SitVersion = _versionService.GetSITVersion(targetPath);
            _barNotificationService.ShowInformational("Config", $"EFT installation path set to '{targetPath}'");
        }
        else {
            _barNotificationService.ShowError("Error", $"The selected folder was invalid. Make sure it's a proper EFT game folder.");
        }
    }

    private async Task ChangeAkiServerLocation() {
        string targetPath = await GetPathLocation("Aki.Server.exe");
        if (!string.IsNullOrEmpty(targetPath)) {
            AkiServerPath = targetPath;
            _barNotificationService.ShowInformational("Config", $"SPT-AKI installation path set to '{targetPath}'");
        }
        else {
            _barNotificationService.ShowError("Error", "The selected folder was invalid. Make sure it's a proper SPT-AKI server folder.");
        }
    }

    partial void OnSelectedConsoleFontFamilyChanged(FontFamily value) {
        ConsoleFontFamily = value.Name;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        base.OnPropertyChanged(e);

        ManagerConfig config = _configsService.Config;
        config.CloseAfterLaunch = CloseAfterLaunch;
        config.LookForUpdates = LookForUpdates;
        config.InstallPath = InstallPath;
        config.TarkovVersion = TarkovVersion;
        config.SitVersion = SitVersion;
        config.AkiServerPath = AkiServerPath;
        config.ConsoleFontFamily = ConsoleFontFamily;
        config.ConsoleFontColor = ConsoleFontColor;

        _configsService.UpdateConfig(config);
        _configsService.Save();
    }
}
