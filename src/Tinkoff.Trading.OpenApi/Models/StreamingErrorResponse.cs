using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class StreamingErrorResponse : StreamingResponse<StreamingErrorPayload>
    {
        public override string Event => "error";

        [JsonConstructor]
        public StreamingErrorResponse(StreamingErrorPayload payload)
            : base(payload)
        {
        }
    }
}