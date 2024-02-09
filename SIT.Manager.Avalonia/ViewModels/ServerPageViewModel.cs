using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIT.Manager.Avalonia.Services;
using System.Collections.ObjectModel;
using System.Drawing;

namespace SIT.Manager.Avalonia.ViewModels
{
    /// <summary>
    /// ServerPageViewModel for handling SPT-AKI Server execution and console output.
    /// </summary>
    public partial class ServerPageViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _consoleFontFamily;

        [ObservableProperty]
        private Color _consoleFontColor;

        public ObservableCollection<string> ConsoleOutput { get; } = [];

        public ServerPageViewModel(IManagerConfigService configService, IAkiServerService akiServerService) {
            // TODO AkiServer.OutputDataReceived += AkiServer_OutputDataReceived;
            // TODO AkiServer.RunningStateChanged += AkiServer_RunningStateChanged;
        }

        [RelayCommand]
        private void StartServer() {
            /* TODO
            if (AkiServer.State == AkiServer.RunningState.NOT_RUNNING)
            {
                if (AkiServer.IsUnhandledInstanceRunning())
                {
                    AddConsole("SPT-AKI is currently running. Please close any running instance of SPT-AKI.");

                    return;
                }

                if (!File.Exists(AkiServer.FilePath))
                {
                    AddConsole("SPT-AKI not found. Please configure the SPT-AKI path in Settings tab before starting the server.");
                    return;
                }

                AddConsole("Starting server...");

                try
                {
                    AkiServer.Start();
                }
                catch (Exception ex)
                {
                    AddConsole(ex.Message);
                }
            }
            else
            {
                AddConsole("Stopping server...");

                try
                {
                    AkiServer.Stop();
                }
                catch (Exception ex)
                {
                    AddConsole(ex.Message);
                }
            }
            */
        }

        /* TODO
        private void AddConsole(string text)
        {
            if (text == null)
                return;

            Paragraph paragraph = new();
            Run run = new();

            //[32m, [2J, [0;0f,
            run.Text = Regex.Replace(text, @"(\[\d{1,2}m)|(\[\d{1}[a-zA-Z])|(\[\d{1};\d{1}[a-zA-Z])", "");

            paragraph.Inlines.Add(run);
            ConsoleLog.Blocks.Add(paragraph);
        }

        private void AkiServer_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            window.DispatcherQueue.TryEnqueue(() => AddConsole(e.Data));
        }

        private void AkiServer_RunningStateChanged(AkiServer.RunningState runningState)
        {
            window.DispatcherQueue.TryEnqueue(() =>
            {
                switch (runningState)
                {
                    case AkiServer.RunningState.RUNNING:
                        {
                            AddConsole("Server started!");
                            StartServerButtonSymbolIcon.Symbol = Symbol.Stop;
                            StartServerButtonTextBlock.Text = "Stop Server";
                        }
                        break;
                    case AkiServer.RunningState.NOT_RUNNING:
                        {
                            AddConsole("Server stopped!");
                            StartServerButtonSymbolIcon.Symbol = Symbol.Play;
                            StartServerButtonTextBlock.Text = "Start Server";
                        }
                        break;
                    case AkiServer.RunningState.STOPPED_UNEXPECTEDLY:
                        {
                            AddConsole("Server stopped unexpectedly! Check console for errors.");
                            StartServerButtonSymbolIcon.Symbol = Symbol.Play;
                            StartServerButtonTextBlock.Text = "Start Server";
                        }
                        break;
                }
            });
        }
        */
    }
}
