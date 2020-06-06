using System;

using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class StreamingErrorResponse : StreamingResponse<StreamingErrorPayload>
    {
        public override string Event => "error";

        [JsonConstructor]
        public StreamingErrorResponse(StreamingErrorPayload payload, DateTime time)
            : base(payload, time)
        {
        }
    }
}