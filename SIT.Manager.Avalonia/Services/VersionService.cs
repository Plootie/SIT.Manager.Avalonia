﻿using PeNet;
using PeNet.Header.Resource;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SIT.Manager.Avalonia.Services
{
    public partial class VersionService : IVersionService
    {
        [GeneratedRegex("[0]{1,}\\.[0-9]{1,2}\\.[0-9]{1,2}\\.[0-9]{1,2}\\-[0-9]{1,5}")]
        private static partial Regex EFTVersionRegex();

        [GeneratedRegex("[1]{1,}\\.[0-9]{1,2}\\.[0-9]{1,5}\\.[0-9]{1,5}")]
        private static partial Regex SITVersionRegex();

        private static string GetFileProductVersionString(string filePath) {
            if (!File.Exists(filePath)) {
                return string.Empty;
            }

            // Use the first traditional / recommended method first
            string fileVersion = FileVersionInfo.GetVersionInfo(filePath).ProductVersion ?? string.Empty;

            // If the above doesn't return anything attempt to read the executable itself
            if (string.IsNullOrEmpty(fileVersion)) {
                PeFile peHeader = new(filePath);
                StringFileInfo? stringFileInfo = peHeader.Resources?.VsVersionInfo?.StringFileInfo;
                if (stringFileInfo != null) {
                    StringTable? fileinfoTable = stringFileInfo.StringTable.Any() ? stringFileInfo.StringTable[0] : null;
                    fileVersion = fileinfoTable?.ProductVersion ?? string.Empty;
                }
            }

            return fileVersion;
        }

        public string GetEFTVersion(string path) {
            string filePath = Path.Combine(path, "EscapeFromTarkov.exe");
            string fileVersion = GetFileProductVersionString(filePath);
            if (string.IsNullOrEmpty(fileVersion)) {
                // TODO Loggy.LogToFile("CheckEFTVersion: File did not exist at " + filePath);
            }
            else {
                fileVersion = EFTVersionRegex().Match(fileVersion).Value.Replace("-", ".");
                // TODO Loggy.LogToFile("EFT Version is now: " + fileVersion);
            }
            return fileVersion;
        }

        public string GetSITVersion(string path) {
            string filePath = Path.Combine(path, "BepInEx", "plugins", "StayInTarkov.dll");
            string fileVersion = GetFileProductVersionString(filePath);
            if (string.IsNullOrEmpty(fileVersion)) {
                // TODO Loggy.LogToFile("CheckSITVersion: File did not exist at " + filePath);
            }
            else {
                fileVersion = SITVersionRegex().Match(fileVersion).Value.ToString();
                // TODO Loggy.LogToFile("SIT Version is now: " + fileVersion);
            }
            return fileVersion;
        }
    }
}
