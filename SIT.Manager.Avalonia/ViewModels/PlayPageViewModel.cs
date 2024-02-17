using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using SIT.Manager.Avalonia.ManagedProcess;
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

        private async Task<string> LoginToServerAsync(Uri address)
        {
            try
            {

            }
            catch
            {

            }
        }

        private Uri? GetUriFromAddress(string addressString)
        {
            try
            {
                UriBuilder addressBuilder = new(addressString);
                addressBuilder.Port = addressBuilder.Port == 80 ? 6969 : addressBuilder.Port;
                return addressBuilder.Uri;
            }
            catch (UriFormatException)
            {
                return null;
            }
            catch(Exception ex)
            {
                //Something BAAAAD has happened here
                //TODO: Loggy & content dialog
                return null;
            }
        }

        [RelayCommand]
        private async Task ConnectToServer()
        {
            Uri? serverAddress = GetUriFromAddress(LastServer);
            if(serverAddress == null)
            {
                await new ContentDialog()
                {
                    Title = "Invalid server address",
                    Content = "Please fix your server address and try again",
                    CloseButtonText = "Ok"
                }.ShowAsync();
                return;
            }

            //TODO: Check install path and SIT installation
            //TODO: Save the config (and my sanity)

            //Connect to server

            //Launch game
        }
    }
}
