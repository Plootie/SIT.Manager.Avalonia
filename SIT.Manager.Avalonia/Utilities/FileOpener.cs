using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Utilities
{
    public static class FileOpener
    {
        public static async Task OpenAtLocation(string path) {
            // On Linux try using dbus first, if that fails then we use the default fallback method
            if (OperatingSystem.IsLinux()) {
                using Process dbusShowItemsProcess = new() {
                    StartInfo = new ProcessStartInfo {
                        FileName = "dbus-send",
                        Arguments = $"--print-reply --dest=org.freedesktop.FileManager1 /org/freedesktop/FileManager1 org.freedesktop.FileManager1.ShowItems array:string:\"file://{path}\" string:\"\"",
                        UseShellExecute = true
                    }
                };
                dbusShowItemsProcess.Start();
                await dbusShowItemsProcess.WaitForExitAsync();

                if (dbusShowItemsProcess.ExitCode == 0) {
                    // The dbus invocation can fail for a variety of reasons:
                    // - dbus is not available
                    // - no programs implement the service,
                    // - ...
                    return;
                }
            }

            using (Process opener = new()) {
                if (OperatingSystem.IsWindows()) {
                    opener.StartInfo.FileName = "explorer";
                    opener.StartInfo.Arguments = $"/select,{path}\"";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                    opener.StartInfo.FileName = "explorer";
                    opener.StartInfo.Arguments = $"-R {path}";
                }
                else {
                    opener.StartInfo.FileName = path;
                    opener.StartInfo.UseShellExecute = true;
                }
                opener.Start();
                await opener.WaitForExitAsync();
            }

        }
    }
}
