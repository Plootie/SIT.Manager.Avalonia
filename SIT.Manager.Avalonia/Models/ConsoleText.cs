using Avalonia.Media;

namespace SIT.Manager.Avalonia.Models
{
    public class ConsoleText
    {
        public SolidColorBrush TextColor { get; set; } = new SolidColorBrush(Colors.White);
        public FontFamily TextFont { get; set; } = FontFamily.Default;
        public string Messagge { get; set; } = string.Empty;
    }
}
