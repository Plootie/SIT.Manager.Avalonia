using Avalonia.Controls;
using System;

namespace SIT.Manager.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        //This feature doesn't work on linux
        if (OperatingSystem.IsLinux())
        {
            CustomTitleBarGrid.IsVisible = false;
        }
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        //TODO: Shutdown aki server here once implemented
    }
}
