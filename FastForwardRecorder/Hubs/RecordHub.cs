using FastForwardLibrary;
using Microsoft.AspNetCore.SignalR;

namespace FastForwardRecorder.Hubs
{
    public class RecordHub : Hub<IRecordClient>
    {
        private ILogger<RecordWorker> Log { get; set; }

        private RecordWorker RecordWorker { get; set; }

        public RecordHub(
            ILogger<RecordWorker> log,
            RecordWorker recordWorker
            )
        {
            Log = log;
            RecordWorker = recordWorker;
            Log.LogInformation($"RecordHub Created");
        }

        public override Task OnConnectedAsync()
        {
            Log.LogInformation("Device connected with connection id {connectionId}", Context.ConnectionId);

            if(RecordWorker.isRunning)
            {
                Clients.Caller.Started();
            } 
            else
            {
                Clients.Caller.Stopped();
            }
            Clients.Caller.State(RecordWorker.State);

            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Log.LogInformation("Device with connection id {connectionId} disconnected", Context.ConnectionId);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sends update about tally to all connected clients.
        /// </summary>
        /// <returns></returns>
        public Task Start()
        {
            var command = @"-stats_period 1 -y -f dshow -i audio=""@device_cm_{33D9A762-90C8-11D0-BD43-00A0C911CE86}\wave_{DE47F64F-DB1D-4EC7-94C9-F4A00D89DDE0}"" -acodec flac ""ljudfile_{date}.ogg""";
            command = replaceCommand(command);
            RecordWorker.Start(command);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Sends update about tally to all connected clients.
        /// </summary>
        /// <returns></returns>
        public async Task Stop()
        {
            await RecordWorker.Stop();
        }


        private string replaceCommand(string command)
        {

            var date = DateTime.Now.ToString("yyyymmdd_HHmmss_fff");
            command = command.Replace("{date}", date);

            return command;
        }


    }

}
