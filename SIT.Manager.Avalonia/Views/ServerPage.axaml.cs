using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using SIT.Manager.Avalonia.ViewModels;

namespace SIT.Manager.Avalonia.Views
{
    public partial class ServerPage : UserControl
    {
        private readonly ScrollViewer? _consoleLogScroller;

        public ServerPage() {
            this.DataContext = App.Current.Services.GetService<ServerPageViewModel>();
            InitializeComponent();
            _consoleLogScroller = this.FindControl<ScrollViewer>("ConsoleLogScroller");
        }

        public void ConsoleLogItemsControl_SizeChanged(object? sender, SizeChangedEventArgs e) {
            _consoleLogScroller?.ScrollToEnd();
        }
    }
}