using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastForwardLibrary
{
    public class FastForward
    {
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

        /// <summary>
        /// The Process ID for the ffmpeg process in windows/linux
        /// </summary>
        public int ProcessId { get; private set; }


        /// <summary>
        /// The last known statistics off the encoding.
        /// </summary>
        public FastForwardState State { get; private set; } = new FastForwardState();

        /// <summary>
        /// Will capture all log messages outputed by ffmpeg
        /// </summary>
        public event EventHandler<string>? Log;


        private Process? Process { get; set; }

        public FastForward(string binary)
        {
            if (!File.Exists(binary))
            {
                throw new FastForwardException($"Cannot find ffmpeg binary {binary}");
            }

            Binary = binary;
        }

        public async Task Execute(string command, CancellationToken cancellationToken)
        {
            var processInfo = CreateCommand(command);
            Process = Process.Start(processInfo);

            if (Process == null)
            {
                return;
            }

            try
            {
                ProcessId = Process.Id;
                Process.ErrorDataReceived += Process_ErrorDataReceived;
                Process.BeginErrorReadLine();

                await Process.WaitForExitAsync(cancellationToken);
            }
            catch (OperationCanceledException e)
            {
                Process.Refresh();

                if(!Process.HasExited)
                {
                    Process.Kill();
                }
                
                throw new FastForwardException($"Execution have been terminated", e);
            }
            finally
            {
                ExitCode = Process.ExitCode;
                Process.Dispose();
                Process = null;
            }
        }

        public void Close()
        {
            Process?.Refresh();

            if(Process != null && !Process.HasExited)
            {
                Process.StandardInput.Write("q");
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            State.Parse(e.Data!);
            Log?.Invoke(this, e.Data!);
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
    }
}
