using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using SIT.Manager.Avalonia.Models;
using SIT.Manager.Avalonia.Models.Messages;
using SIT.Manager.Avalonia.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase, IRecipient<PageNavigationMessage>
{
    private readonly IActionNotificationService _actionNotificationService;
    private readonly IBarNotificationService _barNotificationService;

    private Frame? contentFrame;

    [ObservableProperty]
    private ActionNotification? _actionPanelNotification = new(string.Empty, 0, false);

    public ObservableCollection<BarNotification> BarNotifications { get; } = [];

    public MainViewModel(IActionNotificationService actionNotificationService, IBarNotificationService barNotificationService) {
        _actionNotificationService = actionNotificationService;
        _barNotificationService = barNotificationService;

        _actionNotificationService.ActionNotificationReceived += ActionNotificationService_ActionNotificationReceived;
        _barNotificationService.BarNotificationReceived += BarNotificationService_BarNotificationReceived;

        WeakReferenceMessenger.Default.Register(this);
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

    private bool NavigateToPage(Type page, bool suppressTransition = false) {
        object? currentPage = contentFrame?.Content;
        if (page == currentPage?.GetType()) {
            return false;
        }
        return contentFrame?.Navigate(page, null, suppressTransition ? new SuppressNavigationTransitionInfo() : null) ?? false;
    }

    public void RegisterContentFrame(Frame frame) {
        contentFrame = frame;
    }

    public void Receive(PageNavigationMessage message) {
        NavigateToPage(message.Value.TargetPage, message.Value.SuppressTransition);
    }
}
