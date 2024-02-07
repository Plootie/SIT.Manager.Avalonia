using SIT.Manager.Avalonia.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace SIT.Manager.Avalonia.Services
{
    internal sealed class ManagerConfigService : IManagerConfigService
    {
        private ManagerConfig _config = new();

        public ManagerConfigService() { }

        public void Load() {
            try {
                string currentDir = AppContext.BaseDirectory;
                if (File.Exists(currentDir + @"\ManagerConfig.json")) {
                    _config = JsonSerializer.Deserialize<ManagerConfig>(File.ReadAllText(currentDir + @"\ManagerConfig.json")) ?? new();
                }
            }
            catch (Exception ex) {
                // TODO Loggy.LogToFile("ManagerConfig.Load: " + ex.Message);
            }
        }

        public void Save(bool SaveAccount = false) {
            string currentDir = AppContext.BaseDirectory;
            Debug.WriteLine(currentDir);

            if (SaveAccount == false) {
                ManagerConfig newLauncherConfig = _config;
                newLauncherConfig.Username = string.Empty;
                newLauncherConfig.Password = string.Empty;

                File.WriteAllText(currentDir + "ManagerConfig.json", JsonSerializer.Serialize(newLauncherConfig, new JsonSerializerOptions { WriteIndented = true }));
            }
            else {
                File.WriteAllText(currentDir + "ManagerConfig.json", JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true }));
            }
        }
    }
}
