using System.Text.Json.Serialization;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class StreamingErrorPayload
    {
        [JsonPropertyName("error")]
        public string Error { get; }

        [JsonPropertyName("request_id")]
        public string RequestId { get; }

        [JsonConstructor]
        public StreamingErrorPayload(
            string error,
            string requestId)
        {
            Error = error;
            RequestId = requestId;
        }
    }
}