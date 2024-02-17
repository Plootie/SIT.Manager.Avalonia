using Avalonia.Controls;
using Avalonia.Platform.Storage;
using SIT.Manager.Avalonia.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly Window _target;

        public DirectoryService(Window target) {
            _target = target;
        }

        /// <summary>
        /// Parse the directory path we get from the folder picker 
        /// </summary>
        /// <param name="path">The folder picker path</param>
        /// <returns>A cleaned ready to use path</returns>
        private static string ParseFolderPickerPath(string path) {
            // For some reason the abolute path returns spaces as the %20 escape code
            string filteredPath = path.Replace("%20", " ");
            return filteredPath;
        }

        public async Task<string> GetDirectoryFromPickerAsync() {
            IReadOnlyList<IStorageFolder> folders = await _target.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions() {
                AllowMultiple = false
            });

            if (folders.Count != 0) {
                IStorageFolder selectedFolder = folders[0];
                return ParseFolderPickerPath(selectedFolder.Path.AbsolutePath);
            }
            return string.Empty;
        }

        public async Task OpenDirectoryAsync(string path) {
            string dirPath = Path.GetDirectoryName(path) ?? string.Empty;
            if (!Directory.Exists(dirPath)) {
                // Directory doesn't exist so return early.
                return;
            }
            await FileOpener.OpenAtLocation(dirPath);
        }
    }
}
