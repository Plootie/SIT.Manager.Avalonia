using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
using System;
using Avalonia.Media;
using Avalonia;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Threading;
using FluentAvalonia.UI.Media.Animation;
using System.IO;
using Avalonia.Controls.ApplicationLifetimes;

namespace SIT.Manager.Avalonia.Views;

public partial class MainView : UserControl
{
    private Type? currentPage;
    public MainView()
    {
        InitializeComponent();
    }
    private static readonly Dictionary<string, Type> NavMenuLookup = new()
    {
        { "Play", typeof(PlayPage) },
        { "Tools", typeof(ToolsPage) },
        { "Server", typeof(ServerPage) },
        { "Mods", typeof(ModsPage) },
        { "Settings", typeof(SettingsPage) },
    };

    //TODO: Add a confirmation to this. Not happy with it just starting the updater on click
    private void UpdateButton_Click(object? sender, RoutedEventArgs e)
    {
        //TODO: Add a way to update for linux users
        if (OperatingSystem.IsWindows())
        {
            //TODO: Change this to use a const
            string updaterPath = Path.Combine(AppContext.BaseDirectory, "SIT.Manager.Updater.exe");
            if (File.Exists(updaterPath))
            {
                Process.Start(updaterPath);
                IApplicationLifetime? lifetime = Application.Current?.ApplicationLifetime;
                if(lifetime != null && lifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
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

    //I hate this so much, Please if someone knows of a better way to do this make a pull request. Even microsoft docs recommend this heathenry
    private void NavView_ItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
        => NavigateToPage(e.InvokedItem.ToString() ?? string.Empty);

    //Used to set the FontFamily of the setting tab of the nav view since it's not exposed in the XAML
    private void NavView_Loaded(object? sender, RoutedEventArgs e)
    {
        if (sender is not NavigationView navView || Application.Current?.Resources["BenderFont"] is not FontFamily fontFamily)
            return;
        navView.SettingsItem.FontFamily = fontFamily;

        if(navView.MenuItems.FirstOrDefault() is NavigationViewItem item){
            navView.SelectedItem = item;
            NavigateToPage(item, true);
        }
    }

    private bool NavigateToPage(NavigationViewItem navItem, bool suppressTransition = false)
        => NavigateToPage(navItem.Tag?.ToString() ?? string.Empty, suppressTransition);
    private bool NavigateToPage(string tagName, bool suppressTransition = false)
    {
        if (NavMenuLookup.TryGetValue(tagName, out Type? page))
        {
            if (page == currentPage)
                return false;
            else
            {
                currentPage = page;

                return ContentFrame.Navigate(page, null, suppressTransition ? new SuppressNavigationTransitionInfo() : null);
            }
        }
        else
            return false;
    }
}
