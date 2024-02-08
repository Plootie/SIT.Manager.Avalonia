using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SIT.Manager.Avalonia.Services
{
    public partial class VersionService : IVersionService
    {
        [GeneratedRegex("[0]{1,}\\.[0-9]{1,2}\\.[0-9]{1,2}\\.[0-9]{1,2}\\-[0-9]{1,5}")]
        private static partial Regex EFTVersionRegex();

        [GeneratedRegex("[1]{1,}\\.[0-9]{1,2}\\.[0-9]{1,5}\\.[0-9]{1,5}")]
        private static partial Regex SITVersionRegex();

        public string GetEFTVersion(string path) {
            string filePath = Path.Combine(path, "EscapeFromTarkov.exe");
            if (File.Exists(filePath)) {
                string fileVersion = FileVersionInfo.GetVersionInfo(filePath).ProductVersion ?? string.Empty;
                fileVersion = EFTVersionRegex().Match(fileVersion).Value.Replace("-", ".");
                // TODO Loggy.LogToFile("EFT Version is now: " + fileVersion);
                return fileVersion;
            }
            else {
                // TODO Loggy.LogToFile("CheckEFTVersion: File did not exist at " + filePath);
            }
            return string.Empty;
        }

        public string GetSITVersion(string path) {
            string filePath = Path.Combine(path, "BepInEx", "plugins", "StayInTarkov.dll");
            if (File.Exists(filePath)) {
                string fileVersion = FileVersionInfo.GetVersionInfo(filePath).ProductVersion ?? string.Empty;
                fileVersion = SITVersionRegex().Match(fileVersion).Value.ToString();
                // TODO Loggy.LogToFile("SIT Version is now: " + fileVersion);
                return fileVersion;
            }
            else {
                // TODO Loggy.LogToFile("CheckSITVersion: File did not exist at " + filePath);
            }
            return string.Empty;
        }
    }
}
