using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using SIT.Manager.Avalonia.ViewModels;

namespace SIT.Manager.Avalonia.Views
{
    public partial class ServerPage : ReactiveUserControl<ServerPageViewModel>
    {
        private readonly ScrollViewer? _consoleLogScroller;

        public ServerPage() {
            this.DataContext = App.Current.Services.GetService<ServerPageViewModel>();
            // ServerPageViewModel's WhenActivated block will also get called.
            this.WhenActivated(disposables => { /* Handle view activation etc. */ });
            InitializeComponent();

            _consoleLogScroller = this.FindControl<ScrollViewer>("ConsoleLogScroller");
        }

        public void ConsoleLogItemsRepeater_SizeChanged(object? sender, SizeChangedEventArgs e) {
            _consoleLogScroller?.ScrollToEnd();
        }
    }
}