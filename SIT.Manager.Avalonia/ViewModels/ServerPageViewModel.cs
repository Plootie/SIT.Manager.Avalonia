using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using SIT.Manager.Avalonia.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive.Disposables;
using System.Text.RegularExpressions;
using static SIT.Manager.Avalonia.Services.AkiServerService;

namespace SIT.Manager.Avalonia.ViewModels
{
    /// <summary>
    /// ServerPageViewModel for handling SPT-AKI Server execution and console output.
    /// </summary>
    public partial class ServerPageViewModel : ViewModelBase
    {
        private const int CONSOLE_LINE_LIMIT = 50;

        [GeneratedRegex("(\\[\\d{1,2}m)|(\\[\\d{1}[a-zA-Z])|(\\[\\d{1};\\d{1}[a-zA-Z])")]
        private static partial Regex ConsoleTextOutputFilterRegex();

        private readonly IAkiServerService _akiServerService;
        private readonly IManagerConfigService _configService;

        [ObservableProperty]
        private Symbol _startServerButtonSymbolIcon = Symbol.Play;

        [ObservableProperty]
        private string _startServerButtonTextBlock = "Start Server";

        // TODO load the console font family and color into the item repeater from settings somehow
        [ObservableProperty]
        private string _consoleFontFamily;

        [ObservableProperty]
        private Color _consoleFontColor;

        public ObservableCollection<string> ConsoleOutput { get; } = [];

        public ServerPageViewModel(IManagerConfigService configService, IAkiServerService akiServerService) {
            this._configService = configService;
            this._akiServerService = akiServerService;

            ConsoleFontFamily = _configService.Config.ConsoleFontFamily;
            ConsoleFontColor = _configService.Config.ConsoleFontColor;

            this.WhenActivated((CompositeDisposable disposables) => {
                // Handle Activation

                _akiServerService.OutputDataReceived += AkiServer_OutputDataReceived;
                _akiServerService.RunningStateChanged += AkiServer_RunningStateChanged;

                Disposable.Create(() => {
                    // Handle Deactivation

                    _akiServerService.OutputDataReceived -= AkiServer_OutputDataReceived;
                    _akiServerService.RunningStateChanged -= AkiServer_RunningStateChanged;

                }).DisposeWith(disposables);
            });
        }

        private void AddConsole(string text) {
            if (string.IsNullOrEmpty(text)) {
                return;
            }

            if (ConsoleOutput.Count > CONSOLE_LINE_LIMIT) {
                ConsoleOutput.RemoveAt(0);
            }

            //[32m, [2J, [0;0f,
            text = ConsoleTextOutputFilterRegex().Replace(text, "");

            ConsoleOutput.Add(text);
        }

        private void AkiServer_OutputDataReceived(object? sender, DataReceivedEventArgs e) {
            Dispatcher.UIThread.Post(() => AddConsole(e.Data ?? "\n"));
        }

        private void AkiServer_RunningStateChanged(object? sender, RunningState runningState) {
            Dispatcher.UIThread.Post(() => {
                switch (runningState) {
                    case RunningState.Running: {
                            AddConsole("Server started!");
                            StartServerButtonSymbolIcon = Symbol.Stop;
                            StartServerButtonTextBlock = "Stop Server";
                            break;
                        }
                    case RunningState.NotRunning: {
                            AddConsole("Server stopped!");
                            StartServerButtonSymbolIcon = Symbol.Play;
                            StartServerButtonTextBlock = "Start Server";
                            break;
                        }
                    case RunningState.StoppedUnexpectedly: {
                            AddConsole("Server stopped unexpectedly! Check console for errors.");
                            StartServerButtonSymbolIcon = Symbol.Play;
                            StartServerButtonTextBlock = "Start Server";
                            break;
                        }
                }
            });
        }

        [RelayCommand]
        private void EditServerConfig() {
            // TODO here so VS picks it up as it doesn't in XAML -- this literally does nothing and as far as I can see was never implemented in the current manager either
        }

        [RelayCommand]
        private void StartServer() {
            if (_akiServerService.State == RunningState.NotRunning) {
                if (_akiServerService.IsUnhandledInstanceRunning()) {
                    AddConsole("SPT-AKI is currently running. Please close any running instance of SPT-AKI.");
                    return;
                }

                if (!File.Exists(_akiServerService.ServerFilePath)) {
                    AddConsole("SPT-AKI not found. Please configure the SPT-AKI path in Settings tab before starting the server.");
                    return;
                }

                AddConsole("Starting server...");
                try {
                    _akiServerService.Start();
                }
                catch (Exception ex) {
                    AddConsole(ex.Message);
                }
            }
            else {
                AddConsole("Stopping server...");
                try {
                    _akiServerService.Stop();
                }
                catch (Exception ex) {
                    AddConsole(ex.Message);
                }
            }
        }
    }
}
