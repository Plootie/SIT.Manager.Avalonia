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
                                 IFolderPickerService folderPickerService,
                                 IBarNotificationService notificationService,
                                 IVersionService versionService) {
        _configsService = configService;
        _folderPickerService = folderPickerService;
        _barNotificationService = notificationService;
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
    /// Parse the directory path we get from the folder picker 
    /// </summary>
    /// <param name="path">The folder picker path</param>
    /// <returns>A cleaned ready to use path</returns>
    private static string ParseFolderPickerPath(string path) {
        // For some reason the abolute path returns spaces as the %20 escape code
        string filteredPath = path.Replace("%20", " ");
        return filteredPath;
    }

    /// <summary>
    /// Gets the path containing the required filename based on the folder picker selection from a user
    /// </summary>
    /// <param name="filename">The filename to look for in the user specified directory</param>
    /// <returns>The path if the file exists, otherwise an empty string</returns>
    private async Task<string> GetPathLocation(string filename) {
        IStorageFolder? folderSelected = await _folderPickerService.OpenFolderAsync();
        if (folderSelected != null) {
            string directoryPath = ParseFolderPickerPath(folderSelected.Path.AbsolutePath);
            if (File.Exists(Path.Combine(directoryPath, filename))) {
                return directoryPath;
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
