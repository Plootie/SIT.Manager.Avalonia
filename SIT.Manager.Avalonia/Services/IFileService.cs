using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Downloads a file and show a progress bar if enabled
        /// </summary>
        /// <param name="fileName">The name of the file to be downloaded.</param>
        /// <param name="filePath">The path (not including the filename) to download to.</param>
        /// <param name="fileUrl">The URL to download from.</param>
        /// <param name="showProgress">If a progress bar should show the status.</param>
        /// <returns></returns>
        Task<bool> DownloadFile(string fileName, string filePath, string fileUrl, bool showProgress = false);
        /// <summary>
        /// Extracts a Zip archive
        /// </summary>
        /// <param name="filePath">The file to extract</param>
        /// <param name="destination">The destination to extract to</param>
        /// <returns></returns>
        Task ExtractArchive(string filePath, string destination);
    }
}
