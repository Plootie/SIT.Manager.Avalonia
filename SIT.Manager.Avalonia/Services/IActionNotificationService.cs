using SIT.Manager.Avalonia.Models;
using System;

namespace SIT.Manager.Avalonia.Services
{
    public interface IActionNotificationService
    {
        event EventHandler<ActionNotification>? ActionNotificationReceived;

        void StartActionNotification();
        void StopActionNotification();
        void UpdateActionNotification(ActionNotification notification);
    }
}
