using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using SIT.Manager.Avalonia.Classes;
using SIT.Manager.Avalonia.ManagedProcess;
using SIT.Manager.Avalonia.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
        private readonly HttpClient _httpClient;
        private readonly HttpClientHandler _httpClientHandler;
        private readonly ITarkovClientService _tarkovClientService;
        public PlayPageViewModel(IManagerConfigService configService, HttpClient httpClient, HttpClientHandler httpClientHandler, ITarkovClientService tarkovClientService)
        {
            _configService = configService;
            //TODO: Check that this is the best way to implement DI for the TarkovRequesting. Prettysure service provider would be better
            _httpClient = httpClient;
            _httpClientHandler = httpClientHandler;
            _tarkovClientService = tarkovClientService;

            _lastServer = _configService.Config.LastServer;
            _username = _configService.Config.Username;
            _password = _configService.Config.Password;
            _rememberMe = _configService.Config.RememberLogin;
        }

        private async Task<string> LoginToServerAsync(Uri address)
        {
            try
            {
                TarkovRequesting requesting = new(address, _httpClient, _httpClientHandler);
                TarkovLoginInfo loginInfo = new()
                {
                    Username = Username,
                    Password = Password,
                    BackendUrl = address.AbsoluteUri.Trim(['/', '\\'])
                };

                string SessionID = await requesting.PostJson("/launcher/profile/login", JsonSerializer.Serialize(loginInfo));
                
                if (SessionID.Equals("failed", StringComparison.InvariantCultureIgnoreCase))
                {
                    string connectionData = await requesting.PostJson("/launcher/server/connect", JsonSerializer.Serialize(new object()));
                    //TODO: Give this connection data its own object to deserialize into and pull both the editions and the descriptions

                }
                else if(SessionID.Equals("invalid_password", StringComparison.InvariantCultureIgnoreCase))
                {
                    //TODO: Utils.ShowInfoBar("Connect", $"Invalid password!", InfoBarSeverity.Error);
                    //TODO: throw tarkov error
                    return "error";
                }

                return SessionID;
            }
            catch(Exception ex)
            {
                //TODO: Add correct error handling here
                return string.Empty;
            }
        }

        private static Uri? GetUriFromAddress(string addressString)
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
            string token = await LoginToServerAsync(serverAddress);

            //Launch game
            string launchArguments = $"-token={token} -config={{\"BackendUrl\":\"{serverAddress.AbsoluteUri}\",\"Version\":\"live\"}}";
            _tarkovClientService.Start(launchArguments);
        }
    }
}
