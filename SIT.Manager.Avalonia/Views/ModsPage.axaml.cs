using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace SIT.Manager.Avalonia.Views
{
    public partial class ModsPage : UserControl
    {
        public ModsPage()
        {
            InitializeComponent();
        }

        private void IUnderstandButton_Click(object? sender, RoutedEventArgs e)
        {
            DisclaimerGrid.IsVisible = false;
            ModGrid.IsVisible = true;
        }

        private void ModsList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
