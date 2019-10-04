using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class CandleResponse : StreamingResponse<CandlePayload>
    {
        public override string Event => "candle";

        [JsonConstructor]
        public CandleResponse(CandlePayload payload)
            : base(payload)
        {
        }
    }
}