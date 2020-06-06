using System;

using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class OrderbookResponse : StreamingResponse<OrderbookPayload>
    {
        public override string Event => "orderbook";

        [JsonConstructor]
        public OrderbookResponse(OrderbookPayload payload, DateTime time)
            : base(payload, time)
        {
        }
    }
}