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
}
