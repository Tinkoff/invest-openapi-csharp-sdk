using System.Text.Json.Serialization;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class InstrumentInfoPayload
    {
        [JsonPropertyName("trade_status")]
        public string TradeStatus { get; }

        [JsonPropertyName("min_price_increment")]
        public decimal MinPriceIncrement { get; }

        public int Lot { get; }

        [JsonPropertyName("accrued_interest")]
        public decimal AccruedInterest { get; }

        [JsonPropertyName("limit_up")]
        public decimal LimitUp { get; }

        [JsonPropertyName("limit_down")]
        public decimal LimitDown { get; }

        public string Figi { get; }

        [JsonConstructor]
        public InstrumentInfoPayload(
            string tradeStatus,
            decimal minPriceIncrement,
            int lot,
            decimal accruedInterest,
            decimal limitUp,
            decimal limitDown,
            string figi)
        {
            TradeStatus = tradeStatus;
            MinPriceIncrement = minPriceIncrement;
            Lot = lot;
            AccruedInterest = accruedInterest;
            LimitUp = limitUp;
            LimitDown = limitDown;
            Figi = figi;
        }
    }
}