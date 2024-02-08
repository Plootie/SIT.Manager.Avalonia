using SIT.Manager.Avalonia.Models;

namespace SIT.Manager.Avalonia.Services
{
    public interface IManagerConfigService
    {
        public ManagerConfig Config { get; }

        public void Save(bool SaveAccount = false);
        public void UpdateConfig(ManagerConfig config);
    }
}
