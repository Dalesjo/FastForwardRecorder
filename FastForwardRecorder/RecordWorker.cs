using FastForwardLibrary;
using FastForwardRecorder.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FastForwardRecorder
{
    public class RecordWorker : IHostedService
    {
        private FastForward FastForward { get; set; }

        private AutoResetEvent autoReset = new AutoResetEvent(false);

        private ILogger Log { get; set; }

        private IHubContext<RecordHub, IRecordClient> RecordHub { get; set; }

        public bool isRunning { get {
                return FastForward.IsRunning;
            }
        }

        public FastForwardState State
        {
            get
            {
                return FastForward.State;
            }
        }

        public RecordWorker(
                IHubContext<RecordHub, IRecordClient> recordHub,
                ILogger<RecordWorker> log
            )
        {
            Log = log;
            RecordHub = recordHub;

            var bin = @"c:\Program Files\ffmpeg\ffmpeg-5.1\bin\ffmpeg.exe";
            var directory = @"c:\tmp\ljud\";
            FastForward = new FastForward(bin, directory);
            FastForward.Log += logger;
            FastForward.State.Update += stats;
            FastForward.Exited += FastForward_Exited;
        }

        private void FastForward_Exited(object? sender, EventArgs e)
        {
            RecordHub.Clients.All.Stopped();
        }

        private void FastForward_Started()
        {
            Log.LogInformation("FastForward_Started");
            RecordHub.Clients.All.Started();
        }

        public void Start(string command)
        {
            var started = FastForward.Start(command);
            
            if(started)
            {
                FastForward_Started();
            }
            
        }

        public async Task Stop()
        {
            await FastForward.Stop();
        }

        private void logger(object? sender, string line)
        {
            if (sender is FastForward fastForward)
            {
                //Console.WriteLine($"log ={line}");
            }
        }

        private void stats(object? sender, EventArgs e)
        {
            if (sender is FastForwardState state)
            {
                RecordHub.Clients.All.State(state);
                //Console.WriteLine($"time={state.Time}");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if(FastForward.IsRunning)
            {
                await FastForward.Stop(cancellationToken);
            }
        }
    }
}
