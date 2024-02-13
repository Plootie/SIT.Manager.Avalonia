using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using SIT.Manager.Avalonia.Interfaces;
using SIT.Manager.Avalonia.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.ViewModels
{
    public partial class PlayPageViewModel : ViewModelBase
    {
        private readonly IManagerConfigService _configService;

        [ObservableProperty]
        private string _lastServer;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private bool _rememberMe;
        public PlayPageViewModel(IManagerConfigService configService)
        {
            _configService = configService;

            _lastServer = _configService.Config.LastServer;
            _username = _configService.Config.Username;
            _password = _configService.Config.Password;
            _rememberMe = _configService.Config.RememberLogin;
        }

        [RelayCommand]
        private async Task ConnectToServer()
        {
            //TODO: Move this to its own function to be reused
            try
            {
                UriBuilder addressBuilder = new(LastServer);
                addressBuilder.Port = addressBuilder.Port == 80 ? 6969 : addressBuilder.Port;
                LastServer = addressBuilder.ToString();
            }
            catch (UriFormatException)
            {
                await new ContentDialog()
                {
                    Title = "Invalid server address",
                    Content = "Please fix your server address and try again",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            

            //Update config

            //Connect to server

            //Launch game
        }
    }
}
