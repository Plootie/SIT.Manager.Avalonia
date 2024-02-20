using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIT.Manager.Avalonia.Interfaces;
using SIT.Manager.Avalonia.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.ViewModels
{
    public partial class LocationEditorViewModel : ViewModelBase
    {
        private readonly IBarNotificationService _barNotificationService;
        private readonly IPickerDialogService _pickerDialogService;

        [ObservableProperty]
        private BaseLocation? _location;

        [ObservableProperty]
        private string _loadedLocation = string.Empty;

        public IAsyncRelayCommand LoadCommand { get; }
        public IAsyncRelayCommand SaveCommand { get; }

        public LocationEditorViewModel(IBarNotificationService barNotificationService, IPickerDialogService pickerDialogService) {
            _barNotificationService = barNotificationService;
            _pickerDialogService = pickerDialogService;

            LoadCommand = new AsyncRelayCommand(Load);
        }

        private async Task Load() {
            IStorageFile? file = await _pickerDialogService.GetFileFromPickerAsync(new List<FilePickerFileType>() { FilePickerFileTypes.All });
            if (file == null) {
                return;
            }

            if (File.Exists(file.Path.LocalPath)) {
                BaseLocation? location = JsonSerializer.Deserialize<BaseLocation>(await File.ReadAllTextAsync(file.Path.LocalPath));
                if (location == null) {
                    _barNotificationService.ShowError("Load Error", "There was an error saving the file.");
                    return;
                }

                for (int i = 0; i < location.waves.Count; i++) {
                    location.waves[i].Name = i + 1;
                }

                for (int i = 0; i < location.BossLocationSpawn.Count; i++) {
                    location.BossLocationSpawn[i].Name = i + 1;
                }

                switch (location.Scene.path) {
                    case "maps/factory_day_preset.bundle":
                        LoadedLocation = "Factory (Day)";
                        break;
                    case "maps/factory_night_preset.bundle":
                        LoadedLocation = "Factory (Night)";
                        break;
                    case "maps/woods_preset.bundle":
                        LoadedLocation = "Woods";
                        break;
                    case "maps/customs_preset.bundle":
                        LoadedLocation = "Customs";
                        break;
                    case "maps/shopping_mall.bundle":
                        LoadedLocation = "Interchange";
                        break;
                    case "maps/rezerv_base_preset.bundle":
                        LoadedLocation = "Reserve";
                        break;
                    case "maps/shoreline_preset.bundle":
                        LoadedLocation = "Shoreline";
                        break;
                    case "maps/laboratory_preset.bundle":
                        LoadedLocation = "Labs";
                        break;
                    case "maps/lighthouse_preset.bundle":
                        LoadedLocation = "Lighthouse";
                        break;
                    case "maps/city_preset.bundle":
                        LoadedLocation = "Streets";
                        break;
                    default:
                        break;
                }

                Location = location;

                if (location.waves.Count > 0) {
                    // TODO WaveList.SelectedIndex = 0;
                }

                if (location.BossLocationSpawn.Count > 0) {
                    // TODO BossList.SelectedIndex = 0;
                }

                _barNotificationService.ShowSuccess("Load Location", $"Loaded location {LoadedLocation} successfully.");
            }
        }
    }

    /* TODO
    public sealed partial class LocationEditor : Page
    {
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker filePicker = new()
            {
                DefaultFileExtension = ".json",
                SuggestedFileName = "base.json",
                FileTypeChoices = { { "JSON", new List<string>() { ".json" } } }
            };

            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);

            StorageFile file = await filePicker.PickSaveFileAsync();

            if (file == null)
                return;

            if (File.Exists(file.Path))
            {
                File.Copy(file.Path, file.Path.Replace(".json", ".BAK"), true);
            }

            BaseLocation baseLocation = (BaseLocation)DataContext;
            if (baseLocation == null)
            {
                Utils.ShowInfoBar("Save Error", "There was an error saving the file.", InfoBarSeverity.Error);
                return;
            }
            var json = JsonSerializer.Serialize(baseLocation, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(file.Path, json);
            Utils.ShowInfoBar("Save", $"Successfully saved the file to: {file.Path}", InfoBarSeverity.Success);
        }

        private void AddWaveButton_Click(object sender, RoutedEventArgs e)
        {
            BaseLocation location = (BaseLocation)DataContext;

            if (location != null)
            {
                location.waves.Add(new Wave());

                for (int i = 0; i < location.waves.Count; i++)
                {
                    location.waves[i].Name = i + 1;
                }

                if (location.waves.Count > 0)
                {
                    WaveList.SelectedIndex = 0;
                }
            }
        }

        private void RemoveWaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (WaveList.SelectedIndex == -1)
                return;

            BaseLocation location = (BaseLocation)DataContext;

            if (location != null)
            {
                location.waves.RemoveAt(WaveList.SelectedIndex);

                for (int i = 0; i < location.waves.Count; i++)
                {
                    location.waves[i].Name = i + 1;
                }

                if (location.waves.Count > 0)
                {
                    WaveList.SelectedIndex = 0;
                }
            }
        }

        private void AddBossButton_Click(object sender, RoutedEventArgs e)
        {
            BaseLocation location = (BaseLocation)DataContext;

            if (location != null)
            {
                location.BossLocationSpawn.Add(new BossLocationSpawn());

                for (int i = 0; i < location.BossLocationSpawn.Count; i++)
                {
                    location.BossLocationSpawn[i].Name = i + 1;
                }

                if (location.BossLocationSpawn.Count > 0)
                {
                    BossList.SelectedIndex = 0;
                }
            }
        }

        private void RemoveBossButton_Click(object sender, RoutedEventArgs e)
        {
            if (BossList.SelectedIndex == -1)
                return;

            BaseLocation location = (BaseLocation)DataContext;

            if (location != null)
            {
                location.BossLocationSpawn.RemoveAt(BossList.SelectedIndex);

                for (int i = 0; i < location.BossLocationSpawn.Count; i++)
                {
                    location.BossLocationSpawn[i].Name = i + 1;
                }

                if (location.BossLocationSpawn.Count > 0)
                {
                    BossList.SelectedIndex = 0;
                }
            }
        }
    }
    */
}
