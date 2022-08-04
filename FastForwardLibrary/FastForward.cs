using System.Diagnostics;

namespace FastForwardLibrary
{
    public class FastForward
    {
        public FastForward(string binary)
        {
            if (!File.Exists(binary))
            {
                throw new FastForwardException($"Cannot find ffmpeg binary {binary}");
            }

            Binary = binary;
        }

        /// <summary>
        /// Will capture all log messages outputed by ffmpeg
        /// </summary>
        public event EventHandler<string>? Log;

        /// <summary>
        /// path to ffmpeg
        /// </summary>
        public string Binary { get; private set; }

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
        public FastForwardState State { get; private set; } = new FastForwardState();
        private Process? Process { get; set; }
        public void Start(string command)
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
        }

        public async Task Stop(CancellationToken cancellationToken = default)
        {
            if (IsRunning)
            {
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
            }
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
            }
        }
    }
}