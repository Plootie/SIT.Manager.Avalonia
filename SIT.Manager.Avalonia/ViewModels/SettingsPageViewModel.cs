using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIT.Manager.Avalonia.Models;
using SIT.Manager.Avalonia.Services;
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
    private readonly IFolderPickerService _folderPickerService;
    private readonly IBarNotificationService _barNotificationService;
    private readonly IVersionService _versionService;

    private readonly bool _isLoaded = false;

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

    public SettingsPageViewModel(IManagerConfigService configService,
                                 IFolderPickerService folderPickerService,
                                 IBarNotificationService notificationService,
                                 IVersionService versionService) {
        _configsService = configService;
        _folderPickerService = folderPickerService;
        _barNotificationService = notificationService;
        _versionService = versionService;

        CloseAfterLaunch = _configsService.Config.CloseAfterLaunch;
        LookForUpdates = _configsService.Config.LookForUpdates;
        InstallPath = _configsService.Config.InstallPath;
        TarkovVersion = _configsService.Config.TarkovVersion;
        SitVersion = _configsService.Config.SitVersion;
        AkiServerPath = _configsService.Config.AkiServerPath;
        ConsoleFontFamily = _configsService.Config.ConsoleFontFamily;
        ConsoleFontColor = _configsService.Config.ConsoleFontColor;

        InstalledFonts = [.. FontManager.Current.SystemFonts];
        SelectedConsoleFontFamily = InstalledFonts.First(x => x.Name == ConsoleFontFamily);

        ManagerVersionString = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "N/A";

        _isLoaded = true;
    }

    [RelayCommand]
    private async Task ChangeInstallLocation() {
        IStorageFolder? folderSelected = await _folderPickerService.OpenFolderAsync();
        if (folderSelected != null && File.Exists(Path.Combine(folderSelected.Path.AbsolutePath, "EscapeFromTarkov.exe"))) {
            InstallPath = folderSelected.Path.AbsolutePath;
            TarkovVersion = _versionService.GetEFTVersion(folderSelected.Path.AbsolutePath);
            SitVersion = _versionService.GetSITVersion(folderSelected.Path.AbsolutePath);
            _barNotificationService.ShowInformational("Config", $"EFT installation path set to '{folderSelected.Path.AbsolutePath}'");
        }
        else {
            _barNotificationService.ShowError("Error", $"The selected folder was invalid. Make sure it's a proper EFT game folder.");
        }
    }

    [RelayCommand]
    private async Task ChangeAkiServerLocation() {
        IStorageFolder? folderSelected = await _folderPickerService.OpenFolderAsync();
        if (folderSelected != null && File.Exists(Path.Combine(folderSelected.Path.AbsolutePath, "Aki.Server.exe"))) {
            AkiServerPath = folderSelected.Path.AbsolutePath;
            _barNotificationService.ShowInformational("Config", $"SPT-AKI installation path set to '{folderSelected.Path.AbsolutePath}'");
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

        if (_isLoaded) {
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
}
