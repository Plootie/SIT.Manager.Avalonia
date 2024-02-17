using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Services
{
    public interface IDirectoryService
    {
        /// <summary>
        /// Get a directory from the user using the directory picker dialog
        /// </summary>
        /// <returns>Path the user selected or and empty string</returns>
        Task<string> GetDirectoryFromPickerAsync();
        /// <summary>
        /// Open the system file manager at the path requested, if the directory doesn't exist then do nothing
        /// </summary>
        /// <param name="path">Path to open file manager at</param>
        Task OpenDirectoryAsync(string path);
    }
}
