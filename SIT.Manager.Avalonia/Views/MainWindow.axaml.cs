using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using SIT.Manager.Avalonia.Interfaces;
using SIT.Manager.Avalonia.Services;
using System;

namespace SIT.Manager.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow() {
        InitializeComponent();
        //This feature doesn't work on linux
        if (OperatingSystem.IsLinux()) {
            CustomTitleBarGrid.IsVisible = false;
        }
    }

    private void Window_Closed(object? sender, EventArgs e) {
        IAkiServerService? akiServerService = App.Current.Services.GetService<IAkiServerService>();
        if (akiServerService != null && akiServerService.State == AkiServerService.RunningState.Running) {
            akiServerService.Stop();
        }
    }
}
