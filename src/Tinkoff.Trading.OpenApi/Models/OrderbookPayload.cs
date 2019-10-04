using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class OrderbookPayload
    {
        public int Depth { get; }
        public List<decimal[]> Bids { get; }
        public List<decimal[]> Asks { get; }
        public string Figi { get; }

        [JsonConstructor]
        public OrderbookPayload(int depth, List<decimal[]> bids, List<decimal[]> asks, string figi)
        {
            Depth = depth;
            Bids = bids;
            Asks = asks;
            Figi = figi;
        }
    }
}