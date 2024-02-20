using Avalonia.Controls;
using Avalonia.Platform.Storage;
using SIT.Manager.Avalonia.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Services
{
    public class FolderPickerService : IFolderPickerService
    {
        private readonly Window _target;

        public FolderPickerService(Window target) {
            _target = target;
        }

        public async Task<IStorageFolder?> OpenFolderAsync() {
            IReadOnlyList<IStorageFolder> folders = await _target.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions() {
                AllowMultiple = false
            });

            if (folders.Count == 0) {
                return null;
            }
            return folders[0];
        }
    }
}
