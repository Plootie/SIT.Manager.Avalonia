using Avalonia.Controls;
using Avalonia.Styling;
using FluentAvalonia.UI.Controls;
using SIT.Manager.Avalonia.Models;
using SIT.Manager.Avalonia.ViewModels;
using System;

namespace SIT.Manager.Avalonia
{
    public partial class SelectEditionDialog : ContentDialog, IStyleable
    {
        Type IStyleable.StyleKey => typeof(ContentDialog);
        public SelectEditionDialog(TarkovEdition[] editions)
        {
            InitializeComponent();
            this.DataContext = new SelectEditionDialogViewModel(editions);
            this.ApplyTemplate();
        }
    }
}
