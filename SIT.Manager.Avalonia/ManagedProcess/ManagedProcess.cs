﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.ManagedProcess
{
    public abstract class ManagedProcess(IManagerConfigService configService) : IManagedProcess
    {
        protected abstract string EXECUTABLE_NAME { get; }
        protected readonly IManagerConfigService _configService = configService;
        protected Process? _process;
        protected bool _stopRequest = false;
        public abstract string ExecutableDirectory {get;}
        public string ExecutableFilePath => !string.IsNullOrEmpty(ExecutableDirectory) ? Path.Combine(ExecutableDirectory, EXECUTABLE_NAME) : string.Empty;

        public RunningState State { get; protected set; } = RunningState.NotRunning;

        public event EventHandler<RunningState>? RunningStateChanged;
        protected virtual void ExitedEvent(object? sender, EventArgs e)
        {
            RunningState newState = (State == RunningState.Running && !_stopRequest) ? RunningState.StoppedUnexpectedly : RunningState.NotRunning;
            _stopRequest = false;
            UpdateRunningState(newState);
        }

        protected void UpdateRunningState(RunningState newState)
        {
            State = newState;
            //It's 3am, this probably sucks, idk anymore
            RunningStateChanged?.Invoke(this, State);
        }

        public abstract void Start(string? arguments);

        public virtual void Stop()
        {
            if (State == RunningState.NotRunning || _process == null || _process.HasExited)
            {
                return;
            }

            _stopRequest = true;

            // Stop the server process
            bool clsMsgSent = _process.CloseMainWindow();
            if (!clsMsgSent)
                _process.Kill();

            _process.WaitForExit();
            _process.Close();
        }
    }
}