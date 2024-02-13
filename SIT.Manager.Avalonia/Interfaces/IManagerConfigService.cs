using SIT.Manager.Avalonia.Models;

namespace SIT.Manager.Avalonia.Interfaces
{
    public interface IManagerConfigService
    {
        ManagerConfig Config { get; }

        void Save(bool SaveAccount = false);
        void UpdateConfig(ManagerConfig config);
    }
}
