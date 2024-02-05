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

    private void UpdateButton_Click(object? sender, RoutedEventArgs e)
    {

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
            NavigateToPage(item);
        }
    }

    private bool NavigateToPage(NavigationViewItem navItem)
        => NavigateToPage(navItem.Tag?.ToString() ?? string.Empty);
    private bool NavigateToPage(string tagName)
    {
        if (NavMenuLookup.TryGetValue(tagName, out Type? page))
        {
            if (page == currentPage)
                return false;
            else
            {
                currentPage = page;
                return ContentFrame.Navigate(page);
            }
        }
        else
            return false;
    }
}
