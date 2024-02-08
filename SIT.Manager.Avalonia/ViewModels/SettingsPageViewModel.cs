using CommunityToolkit.Mvvm.ComponentModel;
using SIT.Manager.Avalonia.Models;
using SIT.Manager.Avalonia.Services;
using System.ComponentModel;
using System.Reflection;

namespace SIT.Manager.Avalonia.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    private readonly IManagerConfigService _configsService;

    private readonly bool _isLoaded = false;

    [ObservableProperty]
    private bool _closeAfterLaunch;

    [ObservableProperty]
    private bool _lookForUpdates;

    [ObservableProperty]
    private string _managerVersionString;

    public SettingsPageViewModel(IManagerConfigService configService) {
        _configsService = configService;

        CloseAfterLaunch = _configsService.Config.CloseAfterLaunch;
        LookForUpdates = _configsService.Config.LookForUpdates;

        ManagerVersionString = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "N/A";

        _isLoaded = true;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
        base.OnPropertyChanged(e);

        if (_isLoaded) {
            ManagerConfig config = _configsService.Config;
            config.CloseAfterLaunch = CloseAfterLaunch;
            config.LookForUpdates = LookForUpdates;

            _configsService.UpdateConfig(config);
            _configsService.Save();
        }
    }
}
