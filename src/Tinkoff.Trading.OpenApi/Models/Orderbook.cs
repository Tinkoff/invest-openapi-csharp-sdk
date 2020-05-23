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
        /// <summary>
        /// Шаг цены
        /// </summary>
        public decimal MinPriceIncrement { get; }
        /// <summary>
        /// Номинал для облигаций
        /// </summary>
        public decimal FaceValue { get; }
        public decimal LastPrice { get; }
        public decimal ClosePrice { get; }
        /// <summary>
        /// Верхняя граница цены
        /// </summary>
        public decimal LimitUp { get; }
        /// <summary>
        /// Нижняя граница цены
        /// </summary>
        public decimal LimitDown { get; }

        [JsonConstructor]
        public Orderbook(
            int depth,
            List<OrderbookRecord> bids,
            List<OrderbookRecord> asks,
            string figi,
            TradeStatus tradeStatus,
            decimal minPriceIncrement,
            decimal faceValue,
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
            FaceValue = faceValue;
            LastPrice = lastPrice;
            ClosePrice = closePrice;
            LimitUp = limitUp;
            LimitDown = limitDown;
        }
    }
}
