using SIT.Manager.Avalonia.Services;

namespace SIT.Manager.Avalonia.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private IManagerConfigService _configsService;

        public SettingsPageViewModel(IManagerConfigService configService) {
            _configsService = configService;
        }
    }
}
