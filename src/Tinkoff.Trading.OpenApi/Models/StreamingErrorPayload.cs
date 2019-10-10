using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class StreamingErrorPayload
    {
        public string Error { get; }
        public string RequestId { get; }

        [JsonConstructor]
        public StreamingErrorPayload(
            [JsonProperty("error")] string error,
            [JsonProperty("request_id")] string requestId)
        {
            Error = error;
            RequestId = requestId;
        }
    }
}