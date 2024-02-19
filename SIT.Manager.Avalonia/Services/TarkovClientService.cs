using System;
using System.IO;

namespace SIT.Manager.Avalonia.Services
{
    public class TarkovClientService : ITarkovClientService
    {
        private readonly IBarNotificationService _barNotificationService;
        private readonly IManagerConfigService _configService;

        public TarkovClientService(IBarNotificationService barNotificationService, IManagerConfigService configService) {
            _barNotificationService = barNotificationService;
            _configService = configService;
        }

        private void ClearModCache() {
            string cachePath = _configService.Config.InstallPath;
            if (!string.IsNullOrEmpty(cachePath) && Directory.Exists(cachePath)) {
                // Combine the installPath with the additional subpath.
                cachePath = Path.Combine(cachePath, "BepInEx", "cache");

                // Clear both EFT local cache and additional path.
                foreach (string file in Directory.GetFiles(cachePath)) {
                    File.Delete(file);
                }

                foreach (string subDirectory in Directory.GetDirectories(cachePath)) {
                    Directory.Delete(subDirectory, true);
                }

                // Optionally, display a success message or perform additional actions.
                _barNotificationService.ShowInformational("Cache Cleared", "Everything cleared successfully!");
            }
            else {
                // Handle the case where InstallPath is not found or empty.
                _barNotificationService.ShowError("Cache Clear Error", "InstallPath not found in settings");
            }
        }

        public void ClearCache() {
            ClearLocalCache();
            ClearModCache();
        }

        public void ClearLocalCache() {
            string eftCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp", "Battlestate Games", "EscapeFromTarkov");

            // Check if the directory exists.
            if (Directory.Exists(eftCachePath)) {
                // Delete all files within the directory.
                foreach (string file in Directory.GetFiles(eftCachePath)) {
                    File.Delete(file);
                }

                // Delete all subdirectories and their contents.
                foreach (string subDirectory in Directory.GetDirectories(eftCachePath)) {
                    Directory.Delete(subDirectory, true);
                }

                // Optionally, display a success message or perform additional actions.
                _barNotificationService.ShowInformational("Cache Cleared", "EFT local cache cleared successfully!");
            }
            else {
                // Handle the case where the cache directory does not exist.
                _barNotificationService.ShowWarning("Cache Clear Error", $"EFT local cache directory not found at: {eftCachePath}");
            }
        }
    }
}
