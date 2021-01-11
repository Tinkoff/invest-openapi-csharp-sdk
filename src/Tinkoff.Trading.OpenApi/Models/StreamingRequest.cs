using System.Text.Json.Serialization;

namespace Tinkoff.Trading.OpenApi.Models
{
    public abstract class StreamingRequest
    {
        [JsonPropertyName("event")]
        public abstract string Event { get; }

        [JsonPropertyName("request_id")]
        public string RequestId { get; }

        protected StreamingRequest(string requestId)
        {
            RequestId = requestId;
        }

        public static CandleSubscribeRequest SubscribeCandle(string figi, CandleInterval interval)
        {
            return new CandleSubscribeRequest(figi, interval);
        }

        public static CandleUnsubscribeRequest UnsubscribeCandle(string figi, CandleInterval interval)
        {
            return new CandleUnsubscribeRequest(figi, interval);
        }

        public static OrderbookSubscribeRequest SubscribeOrderbook(string figi, int depth)
        {
            return new OrderbookSubscribeRequest(figi, depth);
        }

        public static OrderbookUnsubscribeRequest UnsubscribeOrderbook(string figi, int depth)
        {
            return new OrderbookUnsubscribeRequest(figi, depth);
        }

        public static InstrumentInfoSubscribeRequest SubscribeInstrumentInfo(string figi)
        {
            return new InstrumentInfoSubscribeRequest(figi);
        }

        public static InstrumentInfoUnsubscribeRequest UnsubscribeInstrumentInfo(string figi)
        {
            return new InstrumentInfoUnsubscribeRequest(figi);
        }

        public class CandleSubscribeRequest : StreamingRequest
        {
            public override string Event => "candle:subscribe";

            [JsonPropertyName("figi")]
            public string Figi { get; }

            [JsonPropertyName("interval")]
            public CandleInterval Interval { get; }

            public CandleSubscribeRequest(string figi, CandleInterval interval, string requestId = null)
                : base(requestId)
            {
                Figi = figi;
                Interval = interval;
            }
        }

        public class CandleUnsubscribeRequest : StreamingRequest
        {
            public override string Event => "candle:unsubscribe";

            [JsonPropertyName("figi")]
            public string Figi { get; }

            [JsonPropertyName("interval")]
            public CandleInterval Interval { get; }

            public CandleUnsubscribeRequest(string figi, CandleInterval interval, string requestId = null)
                : base(requestId)
            {
                Figi = figi;
                Interval = interval;
            }
        }

        public class OrderbookSubscribeRequest : StreamingRequest
        {
            public override string Event => "orderbook:subscribe";

            [JsonPropertyName("figi")]
            public string Figi { get; }

            [JsonPropertyName("depth")]
            public int Depth { get; }

            public OrderbookSubscribeRequest(string figi, int depth, string requestId = null)
                : base(requestId)
            {
                Figi = figi;
                Depth = depth;
            }
        }

        public class OrderbookUnsubscribeRequest : StreamingRequest
        {
            public override string Event => "orderbook:unsubscribe";

            [JsonPropertyName("figi")]
            public string Figi { get; }

            [JsonPropertyName("depth")]
            public int Depth { get; }

            public OrderbookUnsubscribeRequest(string figi, int depth, string requestId = null)
                : base(requestId)
            {
                Figi = figi;
                Depth = depth;
            }
        }

        public class InstrumentInfoSubscribeRequest : StreamingRequest
        {
            public override string Event => "instrument_info:subscribe";

            [JsonPropertyName("figi")]
            public string Figi { get; }

            public InstrumentInfoSubscribeRequest(string figi, string requestId = null)
                : base(requestId)
            {
                Figi = figi;
            }
        }

        public class InstrumentInfoUnsubscribeRequest : StreamingRequest
        {
            public override string Event => "instrument_info:unsubscribe";

            [JsonPropertyName("figi")]
            public string Figi { get; }

            public InstrumentInfoUnsubscribeRequest(string figi, string requestId = null)
                : base(requestId)
            {
                Figi = figi;
            }
        }
    }
}