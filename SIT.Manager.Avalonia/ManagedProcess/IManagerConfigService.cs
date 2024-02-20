using SIT.Manager.Avalonia.Models;
using System;

namespace SIT.Manager.Avalonia.ManagedProcess
{
    public interface IManagerConfigService
    {
        ManagerConfig Config { get; }

        void Save(bool SaveAccount = false);
        void UpdateConfig(ManagerConfig config);
        event EventHandler<ManagerConfig>? ConfigChanged;
    }
}
