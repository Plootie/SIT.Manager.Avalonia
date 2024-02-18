using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using SIT.Manager.Avalonia.Classes;
using SIT.Manager.Avalonia.Classes.Exceptions;
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
        private readonly IAkiServerService _akiServerService;
        public PlayPageViewModel(
            IManagerConfigService configService,
            HttpClient httpClient,
            HttpClientHandler httpClientHandler,
            ITarkovClientService tarkovClientService,
            IAkiServerService akiServerService)
        {
            _configService = configService;
            //TODO: Check that this is the best way to implement DI for the TarkovRequesting. Prettysure service provider would be better
            _httpClient = httpClient;
            _httpClientHandler = httpClientHandler;
            _tarkovClientService = tarkovClientService;
            _akiServerService = akiServerService;

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
                    AkiServerConnectionResponse? serverResponse =
                        JsonSerializer.Deserialize<AkiServerConnectionResponse>(connectionData) ?? throw new IncorrectServerPasswordException();
                    TarkovEdition[] editions = new TarkovEdition[serverResponse.Editions.Length];
                    for(int i = 0; i  < editions.Length; i++)
                    {
                        string editionStr = serverResponse.Editions[i];
                        string descriptionStr = serverResponse.Descriptions[editionStr];
                        editions[i] = new TarkovEdition(editionStr, descriptionStr);
                        Debug.WriteLine($"New TarkovEdition: {editionStr}, {descriptionStr}");
                    }
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
            //TODO: Change this to a serialized object
            string launchArguments = $"-token={token} -config={{\"BackendUrl\":\"{serverAddress.AbsoluteUri}\",\"Version\":\"live\"}}";
            _tarkovClientService.Start(launchArguments);

            if (_configService.Config.CloseAfterLaunch)
            {
                IApplicationLifetime? lifetime = App.Current.ApplicationLifetime;
                if (lifetime != null && lifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
                {
                    desktopLifetime.Shutdown();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}
