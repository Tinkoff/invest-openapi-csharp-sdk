using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class Portfolio
    {
        public List<Position> Positions { get; }

        [JsonConstructor]
        public Portfolio(List<Position> positions)
        {
            Positions = positions;
        }

        public class Position
        {
            public string Figi { get; }
            public string Ticker { get; }
            public string Isin { get; }
            public InstrumentType InstrumentType { get; }
            public decimal Balance { get; }
            public decimal Blocked { get; }
            public MoneyAmount ExpectedYield { get; }
            public int Lots { get; }


            [JsonConstructor]
            public Position(string figi, string ticker, string isin, InstrumentType instrumentType, decimal balance, decimal blocked, MoneyAmount expectedYield, int lots)
            {
                Figi = figi;
                Ticker = ticker;
                Isin = isin;
                InstrumentType = instrumentType;
                Balance = balance;
                Blocked = blocked;
                ExpectedYield = expectedYield;
                Lots = lots;
            }
        }
    }
}