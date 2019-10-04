using Tinkoff.Trading.OpenApi.Models;

namespace Tinkoff.Trading.OpenApi.Network
{
    public class StreamingEventReceivedEventArgs
    {
        public StreamingResponse Response { get; }

        public StreamingEventReceivedEventArgs(StreamingResponse response)
        {
            Response = response;
        }
    }
}