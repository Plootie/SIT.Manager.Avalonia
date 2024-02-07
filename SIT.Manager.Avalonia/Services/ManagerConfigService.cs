﻿using SIT.Manager.Avalonia.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace SIT.Manager.Avalonia.Services
{
    internal sealed class ManagerConfigService : IManagerConfigService
    {
        private ManagerConfig _config = new();
        public ManagerConfig Config {
            get => _config;
            private set { _config = value; }
        }

        public ManagerConfigService() {
            Load();
        }

        private void Load() {
            try {
                string currentDir = AppContext.BaseDirectory;
                if (File.Exists(currentDir + @"\ManagerConfig.json")) {
                    string json = File.ReadAllText(currentDir + @"\ManagerConfig.json");
                    _config = JsonSerializer.Deserialize<ManagerConfig>(json) ?? new();
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

                string json = JsonSerializer.Serialize(newLauncherConfig, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(currentDir + "ManagerConfig.json", json);
            }
            else {
                File.WriteAllText(currentDir + "ManagerConfig.json", JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        public void UpdateConfig(ManagerConfig config) {
            _config = config;
        }
    }
}
