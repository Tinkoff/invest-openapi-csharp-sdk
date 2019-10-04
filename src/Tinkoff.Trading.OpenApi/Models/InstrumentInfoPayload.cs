using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class InstrumentInfoPayload
    {
        public string TradeStatus { get; }
        public decimal MinPriceIncrement { get; }
        public int Lot { get; }
        public decimal AccruedInterest { get; }
        public decimal LimitUp { get; }
        public decimal LimitDown { get; }
        public string Figi { get; }

        [JsonConstructor]
        public InstrumentInfoPayload(
            [JsonProperty("trade_status")] string tradeStatus,
            [JsonProperty("min_price_increment")] decimal minPriceIncrement,
            int lot,
            [JsonProperty("accrued_interest")] decimal accruedInterest,
            [JsonProperty("limit_up")] decimal limitUp,
            [JsonProperty("limit_down")] decimal limitDown,
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