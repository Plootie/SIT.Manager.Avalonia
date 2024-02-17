using SIT.Manager.Avalonia.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using SIT.Manager.Avalonia.Models;
using SIT.Manager.Avalonia.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IActionNotificationService _actionNotificationService;
    private readonly IBarNotificationService _barNotificationService;

    [ObservableProperty]
    private ActionNotification? _actionPanelNotification = new ActionNotification(string.Empty, 0, false);

    public ObservableCollection<BarNotification> BarNotifications { get; } = [];

    public MainViewModel(IActionNotificationService actionNotificationService, IBarNotificationService barNotificationService) {
        _actionNotificationService = actionNotificationService;
        _barNotificationService = barNotificationService;

        _actionNotificationService.ActionNotificationReceived += ActionNotificationService_ActionNotificationReceived;
        _barNotificationService.BarNotificationReceived += BarNotificationService_BarNotificationReceived;
    }

    private void ActionNotificationService_ActionNotificationReceived(object? sender, ActionNotification e) {
        ActionPanelNotification = e;
    }

    private async void BarNotificationService_BarNotificationReceived(object? sender, BarNotification e) {
        BarNotifications.Add(e);
        if (e.Delay > 0) {
            await Task.Delay(TimeSpan.FromSeconds(e.Delay));
            BarNotifications.Remove(e);
        }
    }
}
