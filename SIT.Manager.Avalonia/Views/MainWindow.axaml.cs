using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using SIT.Manager.Avalonia.Services;
using System;

namespace SIT.Manager.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow() {
        InitializeComponent();
        // This feature doesn't work on linux
        if (OperatingSystem.IsLinux()) {
            CustomTitleBarGrid.IsVisible = false;
        }
    }

    private void Window_Closed(object? sender, EventArgs e) {
        IAkiServerService? akiServerService = App.Current.Services.GetService<IAkiServerService>();
        IManagerConfigService? managerConfigService = App.Current.Services.GetService<IManagerConfigService>();
        if (akiServerService != null && akiServerService.State == AkiServerService.RunningState.Running)
        {
            //This logic looks a little funky but its just so that if the config services is null we default close it anyway
            if (managerConfigService != null && (!managerConfigService.Config.CloseServerOnClose || managerConfigService.Config.CloseAfterLaunch))
                return;
            akiServerService.Stop();
        }
    }
}
