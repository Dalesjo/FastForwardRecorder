using System.Globalization;
using System.Text.RegularExpressions;

namespace FastForwardLibrary
{
    public class FastForwardState
    {
        private readonly Regex bitrateExpression = new(@"bitrate=\s*(?<bitrate>[0-9\.]+)(?<si>bits\/s|mbits\/s|kbits\/s)?.*");
        private readonly Regex sizeExpression = new(@"size=\s*(?<size>[0-9]+)(?<si>kB|mB|b)?");
        private readonly Regex speedExpression = new(@"speed=\s*(?<speed>[0-9\.]+)x");
        private readonly Regex timeExpression = new(@"time=s*(?<duration>[0-9\:\.]+)");
        /// <summary>
        /// will trigger eachtime the State (size,bitrate,time,speed) have been updated for ffmpeg.
        /// </summary>
        public event EventHandler? Update;

        /// <summary>
        /// The current bitrate in bytes.
        /// </summary>
        public float Bitrate { get; private set; } = 0;

        /// <summary>
        /// The current file size in bytes of the output file
        /// </summary>
        public long Size { get; private set; } = 0;

        /// <summary>
        /// The current file size in bytes of the output file
        /// </summary>
        public long FreeSpace { get; private set; } = 0;

        /// <summary>
        /// How fast the encoding is going 1 means realtime.
        /// </summary>
        public float Speed { get; private set; } = 0;

        /// <summary>
        /// How long (according to ffmpeg) the encoding have lasted
        /// </summary>
        public double Time { get; private set; } = default;

        /// <summary>
        /// path to ffmpeg
        /// </summary>
        public string WorkingDirectory { get; private set; }

        public long Iterartions { get; private set; } = 0;

        public int EveryTime { get; set; } = 10;

        public FastForwardState(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
            getFreeSpace();
        }

        public void Parse(string line)
        {
            if (line == null)
            {
                return;
            }

            var isDuration = GetDuration(line);

            if (!isDuration)
            {
                return;
            }

            GetSize(line);
            GetSpeed(line);
            GetBitrate(line);

            if (Iterartions++ % EveryTime == 0)
            {
                getFreeSpace();
            }

            Update?.Invoke(this, EventArgs.Empty);
        }

        private void getFreeSpace()
        {
            FreeSpace = new DriveInfo(WorkingDirectory).AvailableFreeSpace;
        }

        private bool GetBitrate(string line)
        {
            var match = bitrateExpression.Match(line);
            if (!match.Success)
            {
                return false;
            }

            var si = match.Groups["si"].Value;
            Bitrate = float.Parse(match.Groups["bitrate"].Value, CultureInfo.InvariantCulture);

            if (si == "kbits/s")
            {
                Bitrate *= 1000;
            }

            if (si == "mbits/s")
            {
                Bitrate *= 1000000;
            }

            return match.Success;
        }

        private bool GetDuration(string line)
        {
            var match = timeExpression.Match(line);
            if (!match.Success)
            {
                return false;
            }

            var seconds = match.Groups[1].Value;
            Time = TimeSpan.Parse(seconds).TotalSeconds;

            return match.Success;
        }

        private bool GetSize(string line)
        {
            var match = sizeExpression.Match(line);

            if (!match.Success)
            {
                return false;
            }

            var si = match.Groups["si"].Value;
            Size = Int64.Parse(match.Groups["size"].Value);

            if (si == "kB")
            {
                Size *= 1000;
            }

            if (si == "mB")
            {
                Size *= 1000000;
            }

            return match.Success;
        }

        private bool GetSpeed(string line)
        {
            var match = speedExpression.Match(line);
            if (!match.Success)
            {
                return false;
            }

            Speed = float.Parse(match.Groups["speed"].Value, CultureInfo.InvariantCulture);

            return match.Success;
        }
    }
}