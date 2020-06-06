using System;

using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class InstrumentInfoResponse : StreamingResponse<InstrumentInfoPayload>
    {
        public override string Event => "instrument_info";

        [JsonConstructor]
        public InstrumentInfoResponse(InstrumentInfoPayload payload, DateTime time)
            : base(payload, time)
        {
        }
    }
}