using System.Diagnostics;

namespace FastForwardLibrary
{
    public class FastForward
    {
        public FastForward(string binary,string workingDirectory)
        {
            if (!File.Exists(binary))
            {
                throw new FastForwardException($"Cannot find ffmpeg binary {binary}");
            }

            if(!Directory.Exists(workingDirectory))
            {
                throw new FastForwardException($"Cannot find working directory {workingDirectory}");
            }

            Binary = binary;
            WorkingDirectory = workingDirectory;
            State = new FastForwardState(WorkingDirectory);
        }

        /// <summary>
        /// Will capture all log messages outputed by ffmpeg
        /// </summary>
        public event EventHandler<string>? Log;

        /// <summary>
        /// Called when process has exited
        /// </summary>
        public event EventHandler? Exited;

        /// <summary>
        /// path to ffmpeg
        /// </summary>
        public string Binary { get; private set; }

        /// <summary>
        /// path to ffmpeg
        /// </summary>
        public string WorkingDirectory { get; private set; }

        /// <summary>
        /// returns the last known exitcode.
        ///
        /// In windows
        /// -1 means the application was terminated/killed.
        /// 0 means ffmpeg finished it jobs successfully
        /// +1 means ffmpeg crashed.
        /// </summary>
        public int ExitCode { get; private set; }

        public bool IsRunning
        {
            get
            {
                Process?.Refresh();
                return Process != null && !Process.HasExited;
            }
        }

        /// <summary>
        /// The Process ID for the ffmpeg process in windows/linux
        /// </summary>
        public int ProcessId { get; private set; }

        /// <summary>
        /// The last known statistics off the encoding.
        /// </summary>
        public FastForwardState State { get; private set; }
        private Process? Process { get; set; }
        public bool Start(string command)
        {
            if (IsRunning)
            {
                throw new FastForwardException($"A Process is already running");
            }

            var processInfo = CreateCommand(command);
            Process = Process.Start(processInfo);

            if (Process == null)
            {
                throw new FastForwardException($"Failed to start Process.");
            }

            ProcessId = Process.Id;
            Process.ErrorDataReceived += Process_ErrorDataReceived;
            Process.Exited += Process_Exited;
            Process.BeginErrorReadLine();

            return Process != null && !Process.HasExited;
        }

        public async Task<bool> Stop(CancellationToken cancellationToken = default)
        {
            if (!IsRunning)
            {
                return false;
            }

            try
            {
                await Process!.StandardInput.WriteAsync("q");
                await Process.WaitForExitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Process!.Refresh();

                if (!Process.HasExited)
                {
                    Process.Kill();
                }
            }

            return true;
        }

        public async Task WaitForExitAsync()
        {
            if (IsRunning)
            {
                await Process!.WaitForExitAsync();
            }
        }

        private void Clean()
        {
            Process?.Dispose();
            Process = null;
            ProcessId = default;
        }
        private ProcessStartInfo CreateCommand(string command)
        {
            var process = new ProcessStartInfo
            {
                FileName = Binary,
                WorkingDirectory = WorkingDirectory,
                Arguments = command,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            };

            return process;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            State.Parse(e.Data!);
            Log?.Invoke(this, e.Data!);
        }

        private void Process_Exited(object? sender, EventArgs e)
        {
            if(sender is Process process)
            {
                ExitCode = process.ExitCode;
                Clean();
                Exited?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}