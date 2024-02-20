using Avalonia.Controls.ApplicationLifetimes;
using SIT.Manager.Avalonia.ManagedProcess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Services
{
    public class TarkovClientService(IManagerConfigService configService) : ManagedProcess.ManagedProcess(configService), ITarkovClientService
    {
        private const string TARKOV_EXE = "EscapeFromTarkov.exe";
        public override string ExecutableDirectory => !string.IsNullOrEmpty(_configService.Config.InstallPath) ? _configService.Config.InstallPath : string.Empty;

        protected override string EXECUTABLE_NAME => TARKOV_EXE;

        public override void Start(string? arguments)
        {
            _process = new Process()
            {
                StartInfo = new(ExecutableFilePath)
                {
                    WorkingDirectory = ExecutableDirectory,
                    UseShellExecute = true,
                    Arguments = arguments
                },
                EnableRaisingEvents = true,
            };
            _process.Exited += new EventHandler((sender, e) => ExitedEvent(sender, e));
            _process.Start();

            if (_configService.Config.CloseAfterLaunch)
            {
                IApplicationLifetime? lifetime = App.Current?.ApplicationLifetime;
                if (lifetime != null && lifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
                {
                    desktopLifetime.Shutdown();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            else
            {
                UpdateRunningState(RunningState.Running);
            }
        }
    }
}
