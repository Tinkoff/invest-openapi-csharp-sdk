using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class MarketInstrument
    {
        public string Figi { get; }
        public string Ticker { get; }
        public string Isin { get; }
        /// <summary>
        /// Шаг цены
        /// </summary>
        public decimal MinPriceIncrement { get; }
        public int Lot { get; }
        public Currency Currency { get; }
        public string Name { get; }
        public InstrumentType Type { get; }

        [JsonConstructor]
        public MarketInstrument(
            string figi,
            string ticker,
            string isin,
            decimal minPriceIncrement,
            int lot,
            Currency currency,
            string name,
            InstrumentType type)
        {
            Figi = figi;
            Ticker = ticker;
            Isin = isin;
            MinPriceIncrement = minPriceIncrement;
            Lot = lot;
            Currency = currency;
            Name = name;
            Type = type;
        }
    }
}
