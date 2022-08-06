using FastForwardLibrary;
using FastForwardRecorder.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FastForwardRecorder
{
    public class RecordWorker : IHostedService
    {
        private FastForward FastForward { get; set; }

        private AutoResetEvent autoReset = new AutoResetEvent(false);

        TaskCompletionSource<bool> IsSomethingLoading = new TaskCompletionSource<bool>();

        private ILogger Log { get; set; }

        public IHubContext<RecordHub, IRecordClient> RecordHub { get; private set; }

        public RecordWorker(
                IHubContext<RecordHub, IRecordClient> recordHub,
                ILogger<RecordWorker> log
            )
        {
            Log = log;
            RecordHub = recordHub;

            var bin = @"c:\Program Files\ffmpeg\ffmpeg-5.1\bin\ffmpeg.exe";
            FastForward = new FastForward(bin);
            FastForward.Log += logger;
            FastForward.State.Update += stats;
        }

        public void Start(string command)
        {
            FastForward.Start(command);
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
