using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class Orderbook
    {
        public int Depth { get; }
        public List<OrderbookRecord> Bids { get; }
        public List<OrderbookRecord> Asks { get; }
        public string Figi { get; }
        public TradeStatus TradeStatus { get; }
        public decimal MinPriceIncrement { get; }
        public decimal LastPrice { get; }
        public decimal ClosePrice { get; }
        public decimal LimitUp { get; }
        public decimal LimitDown { get; }

        [JsonConstructor]
        public Orderbook(
            int depth,
            List<OrderbookRecord> bids,
            List<OrderbookRecord> asks,
            string figi,
            TradeStatus tradeStatus,
            decimal minPriceIncrement,
            decimal lastPrice,
            decimal closePrice,
            decimal limitUp,
            decimal limitDown)
        {
            Depth = depth;
            Bids = bids;
            Asks = asks;
            Figi = figi;
            TradeStatus = tradeStatus;
            MinPriceIncrement = minPriceIncrement;
            LastPrice = lastPrice;
            ClosePrice = closePrice;
            LimitUp = limitUp;
            LimitDown = limitDown;
        }
    }
}