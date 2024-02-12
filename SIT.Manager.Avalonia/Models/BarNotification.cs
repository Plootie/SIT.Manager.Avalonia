using FluentAvalonia.UI.Controls;

namespace SIT.Manager.Avalonia.Models
{
    public class BarNotification
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public InfoBarSeverity Severity { get; set; } = InfoBarSeverity.Informational;
        public int Delay { get; set; } = 5;
    }
}
