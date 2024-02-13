using SIT.Manager.Avalonia.Interfaces;
using SIT.Manager.Avalonia.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IBarNotificationService _barNotificationService;

    public ObservableCollection<BarNotification> BarNotifications { get; } = [];

    public MainViewModel(IBarNotificationService barNotificationService) {
        _barNotificationService = barNotificationService;

        _barNotificationService.BarNotificationReceived += BarNotificationService_BarNotificationReceived;
    }

    private async void BarNotificationService_BarNotificationReceived(object? sender, BarNotification e) {
        BarNotifications.Add(e);
        if (e.Delay > 0) {
            await Task.Delay(TimeSpan.FromSeconds(e.Delay));
            BarNotifications.Remove(e);
        }
    }
}
