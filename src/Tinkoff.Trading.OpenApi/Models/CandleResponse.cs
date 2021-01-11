using System;
using System.Text.Json.Serialization;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class CandleResponse : StreamingResponse<CandlePayload>
    {
        public override string Event => "candle";

        [JsonConstructor]
        public CandleResponse(CandlePayload payload, DateTime time)
            : base(payload, time)
        {
        }
    }
}