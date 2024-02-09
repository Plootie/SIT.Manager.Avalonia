using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using SIT.Manager.Avalonia.ViewModels;

namespace SIT.Manager.Avalonia.Views
{
    public partial class ServerPage : ReactiveUserControl<ServerPageViewModel>
    {
        public ServerPage() {
            this.DataContext = App.Current.Services.GetService<ServerPageViewModel>();
            // ServerPageViewModel's WhenActivated block will also get called.
            this.WhenActivated(disposables => { /* Handle view activation etc. */ });
            InitializeComponent();
        }
    }
}