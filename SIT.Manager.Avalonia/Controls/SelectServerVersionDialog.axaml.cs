using Avalonia.Styling;
using FluentAvalonia.UI.Controls;
using SIT.Manager.Avalonia.Models;
using SIT.Manager.Avalonia.ViewModels.Dialogs;
using System;

namespace SIT.Manager.Avalonia.Controls
{
    public partial class SelectServerVersionDialog : ContentDialog, IStyleable
    {
        private readonly SelectServerVersionDialogViewModel dc;

        Type IStyleable.StyleKey => typeof(ContentDialog);

        public SelectServerVersionDialog() {
            dc = new SelectServerVersionDialogViewModel();
            this.DataContext = dc;
            InitializeComponent();
        }

        public GithubRelease? GetSelectedGithubRelease() {
            return dc.SelectedRelease;
        }
    }
}