using FastForwardLibrary;
using Microsoft.AspNetCore.SignalR;

namespace FastForwardRecorder.Hubs
{
    public class RecordHub : Hub<IRecordClient>
    {
        private ILogger<RecordWorker> Log { get; set; }

        RecordHub(ILogger<RecordWorker> log)
        {
            Log = log;
        }

        /// <summary>
        /// Sends update about tally to all connected clients.
        /// </summary>
        /// <returns></returns>
        private async Task State(FastForwardState fastForwardState)
        {
            Log.LogInformation($"Sent message to all clients");
            await Clients.All.State(fastForwardState);
        }
    }


}
