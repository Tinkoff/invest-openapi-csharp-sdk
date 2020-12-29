using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class MarketInstrumentList
    {
        public int Total { get; }
        public List<MarketInstrument> Instruments { get; }

        [JsonConstructor]
        public MarketInstrumentList(int total, List<MarketInstrument> instruments)
        {
            Total = total;
            Instruments = instruments;
        }
    }
}