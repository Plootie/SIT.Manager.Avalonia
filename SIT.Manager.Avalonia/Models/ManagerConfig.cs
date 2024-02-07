using Avalonia.Media;
using System.Collections.Generic;

namespace SIT.Manager.Avalonia.Models
{
    public class ManagerConfig
    {
        public string LastServer { get; set; } = "http://127.0.0.1:6969";
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string InstallPath { get; set; } = string.Empty;
        public string AkiServerPath { get; set; } = string.Empty;
        public bool RememberLogin { get; set; } = false;
        public bool CloseAfterLaunch { get; set; } = false;
        public string TarkovVersion { get; set; } = string.Empty;
        public string SitVersion { get; set; } = string.Empty;
        public bool LookForUpdates { get; set; } = true;
        public bool AcceptedModsDisclaimer { get; set; } = false;
        public string ModCollectionVersion { get; set; } = string.Empty;
        public Dictionary<string, string> InstalledMods { get; set; } = [];
        public string ConsoleFontColor { get; set; } = Colors.LightBlue.ToString();
        public string ConsoleFontFamily { get; set; } = "Consolas";
    }
}
