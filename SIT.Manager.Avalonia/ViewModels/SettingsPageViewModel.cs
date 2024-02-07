using CommunityToolkit.Mvvm.ComponentModel;
using SIT.Manager.Avalonia.Models;
using SIT.Manager.Avalonia.Services;
using System.ComponentModel;

namespace SIT.Manager.Avalonia.ViewModels
{
    public partial class SettingsPageViewModel : ViewModelBase
    {
        private IManagerConfigService _configsService;

        private bool _isLoaded = false;

        [ObservableProperty]
        private bool _closeAfterLaunch;

        [ObservableProperty]
        private bool _lookForUpdates;

        public SettingsPageViewModel(IManagerConfigService configService) {
            _configsService = configService;

            CloseAfterLaunch = _configsService.Config.CloseAfterLaunch;
            LookForUpdates = _configsService.Config.LookForUpdates;

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
}
