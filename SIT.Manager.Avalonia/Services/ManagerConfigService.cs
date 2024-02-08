using SIT.Manager.Avalonia.Converters;
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
        public ManagerConfig Config {
            get => _config;
            private set { _config = value; }
        }

        public ManagerConfigService() {
            Load();
        }

        private void Load() {
            var options = new JsonSerializerOptions() {
                Converters = {
                    new ColorJsonConverter()
                }
            };

            try {
                string managerConfigPath = Path.Combine(AppContext.BaseDirectory, "ManagerConfig.json");
                if (File.Exists(managerConfigPath)) {
                    string json = File.ReadAllText(managerConfigPath);
                    _config = JsonSerializer.Deserialize<ManagerConfig>(json, options) ?? new();
                }
            }
            catch (Exception ex) {
                // TODO Loggy.LogToFile("ManagerConfig.Load: " + ex.Message);
            }
        }

        public void Save(bool SaveAccount = false) {
            var options = new JsonSerializerOptions() {
                Converters = {
                    new ColorJsonConverter()
                },
                WriteIndented = true
            };

            string managerConfigPath = Path.Combine(AppContext.BaseDirectory, "ManagerConfig.json");
            Debug.WriteLine(managerConfigPath);

            if (SaveAccount == false) {
                ManagerConfig newLauncherConfig = _config;
                newLauncherConfig.Username = string.Empty;
                newLauncherConfig.Password = string.Empty;

                string json = JsonSerializer.Serialize(newLauncherConfig, options);
                File.WriteAllText(managerConfigPath, json);
            }
            else {
                File.WriteAllText(managerConfigPath, JsonSerializer.Serialize(_config, options));
            }
        }

        public void UpdateConfig(ManagerConfig config) {
            _config = config;
        }
    }
}
