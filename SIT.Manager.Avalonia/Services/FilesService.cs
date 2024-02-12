using CG.Web.MegaApiClient;
using SIT.Manager.Avalonia.Extentions;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Services
{
    public class FileService : IFileService
    {
        private readonly IActionNotificationService _actionNotificationService;
        private readonly IManagerConfigService _configService;

        public FileService(IActionNotificationService actionNotificationService, IManagerConfigService configService) {
            _actionNotificationService = actionNotificationService;
            _configService = configService;
        }

        private async Task<bool> DownloadMegaFile(string fileName, string fileUrl) {
            // TODO Loggy.LogToFile("Attempting to use Mega API.");
            try {
                MegaApiClient megaApiClient = new();
                await megaApiClient.LoginAnonymousAsync();

                // Todo: Add proper error handling below
                if (!megaApiClient.IsLoggedIn) {
                    return false;
                }

                // TODO Loggy.LogToFile($"Starting download of '{fileName}' from '{fileUrl}'");

                Progress<double> progress = new Progress<double>((prog) => {
                    // TODO mainQueue.TryEnqueue(() => { window.actionProgressBar.Value = (int) Math.Floor(prog); });
                });

                Uri fileLink = new(fileUrl);
                INode fileNode = await megaApiClient.GetNodeFromLinkAsync(fileLink);

                string targetPath = Path.Combine(_configService.Config.InstallPath, fileName);
                await megaApiClient.DownloadFileAsync(fileNode, targetPath, progress);

                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// Downloads a file and shows a progress bar if enabled
        /// </summary>
        /// <param name="fileName">The name of the file to be downloaded.</param>
        /// <param name="filePath">The path (not including the filename) to download to.</param>
        /// <param name="fileUrl">The URL to download from.</param>
        /// <param name="showProgress">If a progress bar should show the status.</param>
        /// <returns></returns>
        public async Task<bool> DownloadFile(string fileName, string filePath, string fileUrl, bool showProgress = false) {
            // TODO var window = App.m_window as MainWindow;
            // TODO DispatcherQueue mainQueue = window.DispatcherQueue;

            /* TODO
            if (showProgress == true)
                mainQueue.TryEnqueue(() =>
                {
                    window.actionPanel.Visibility = Visibility.Visible;
                    window.actionProgressRing.Visibility = Visibility.Visible;
                    window.actionTextBlock.Text = $"Downloading '{fileName}'";
                });
            */

            bool result = false;
            if (fileUrl.Contains("mega.nz")) {
                result = await DownloadMegaFile(fileName, fileUrl);
            }
            else {
                // TODO  Loggy.LogToFile($"Starting download of '{fileName}' from '{fileUrl}'");
                filePath = Path.Combine(filePath, fileName);
                if (File.Exists(filePath)) {
                    File.Delete(filePath);
                }

                var progress = new Progress<float>((prog) => {
                    // TODO mainQueue.TryEnqueue(() => { window.actionProgressBar.Value = (int) Math.Floor(prog); });
                });

                try {
                    using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None)) {
                        using (HttpClient httpClient = new() {
                            Timeout = TimeSpan.FromSeconds(15),
                            DefaultRequestHeaders = {
                            { "X-GitHub-Api-Version", "2022-11-28" },
                            { "User-Agent", "request" }
                        }
                        }) {
                            await httpClient.DownloadDataAsync(fileUrl, file, progress);
                        }
                    }
                    result = true;
                }
                catch (Exception ex) {
                    // TODO Loggy.LogToFile("DownloadFile: " + ex.Message);
                }
            }

            /* TODO
if (showProgress == true)
        mainQueue.TryEnqueue(() =>
        {
            window.actionPanel.Visibility = Visibility.Collapsed;
            window.actionProgressRing.Visibility = Visibility.Collapsed;
            window.actionTextBlock.Text = "";
        });
*/

            return result;
        }

        public void ExtractArchive(string filePath, string destination) {
            // TODO var window = App.m_window as MainWindow;
            // TODO DispatcherQueue mainQueue = window.DispatcherQueue;

            // Ensures that the last character on the extraction path is the directory separator char.
            // Without this, a malicious zip file could try to traverse outside of the expected extraction path.
            if (!destination.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)) {
                destination += Path.DirectorySeparatorChar;
            }
            destination = Path.GetFullPath(destination);

            try {
                using (ZipArchive archive = ZipFile.OpenRead(filePath)) {
                    int totalFiles = archive.Entries.Count;
                    int completed = 0;

                    /* TODO
                    // Show Action Panel
                mainQueue.TryEnqueue(() =>
                {
                    window.actionPanel.Visibility = Visibility.Visible;
                    window.actionProgressRing.Visibility = Visibility.Visible;
                });

                                    var progress = new Progress<float>((prog) => { mainQueue.TryEnqueue(() => { window.actionProgressBar.Value = (int)Math.Floor(prog); }); });
                IProgress<float> progressBar = progress;
                    */

                    foreach (ZipArchiveEntry entry in archive.Entries) {
                        // Gets the full path to ensure that relative segments are removed.
                        string destinationPath = Path.GetFullPath(Path.Combine(destination, entry.FullName));

                        // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                        // are case-insensitive.
                        if (destinationPath.StartsWith(destination, StringComparison.Ordinal)) {
                            if (Path.EndsInDirectorySeparator(destinationPath)) {
                                Directory.CreateDirectory(destinationPath);
                            }
                            else {
                                entry.ExtractToFile(destinationPath, true);
                            }
                        }
                        completed++;

                        // TODO progressBar.Report(((float) completed / totalFiles.Count()) * 100);
                        // TODO mainQueue.TryEnqueue(() => { window.actionTextBlock.Text = $"Extracting file {file.Key.Split("/").Last()} ({completed}/{totalFiles.Count()})"; });
                    }
                }
            }
            catch (Exception ex) {
                // TODO Loggy.LogToFile("ExtractFile: Error when opening Archive: " + ex.Message + "\n" + ex);
            }

            /* TODO
            mainQueue.TryEnqueue(() =>
                {
                    window.actionPanel.Visibility = Visibility.Collapsed;
                    window.actionProgressRing.Visibility = Visibility.Collapsed;
                });
            */
        }
    }
}
