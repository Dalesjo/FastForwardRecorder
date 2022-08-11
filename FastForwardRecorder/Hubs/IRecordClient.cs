using FastForwardLibrary;

namespace FastForwardRecorder.Hubs
{
    public interface IRecordClient
    {
        Task Started();

        Task Stopped();

        Task State(FastForwardState fastForwardState);
    }
}
