using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    [JsonConverter(typeof(StreamingResponseConverter))]
    public abstract class StreamingResponse
    {
        [JsonIgnore] public abstract string Event { get; }
    }

    public abstract class StreamingResponse<T> : StreamingResponse
    {
        public T Payload { get; }

        protected StreamingResponse(T payload)
        {
            Payload = payload;
        }
    }
}