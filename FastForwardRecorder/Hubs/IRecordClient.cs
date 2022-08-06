using FastForwardLibrary;

namespace FastForwardRecorder.Hubs
{
    public interface IRecordClient
    {
        Task Play();

        Task Stop();

        Task State(FastForwardState fastForwardState);
    }
}
