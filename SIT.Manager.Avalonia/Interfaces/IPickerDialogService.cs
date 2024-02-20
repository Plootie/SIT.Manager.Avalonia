using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Interfaces
{
    public interface IPickerDialogService
    {
        /// <summary>
        /// Get a directory from the user using the directory picker dialog
        /// </summary>
        /// <returns>IStorageFolder the user selected or null</returns>
        Task<IStorageFolder?> GetDirectoryFromPickerAsync();
        /// <summary>
        /// Get a file from the user using the file picker dialog
        /// </summary>
        /// <returns>IStorageFile the user selected or null</returns>
        Task<IStorageFile?> GetFileFromPickerAsync(List<FilePickerFileType>? fileTypeFiler = null);
    }
}
