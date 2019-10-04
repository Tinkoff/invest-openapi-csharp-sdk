using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class PortfolioCurrencies
    {
        public List<PortfolioCurrency> Currencies { get; }

        [JsonConstructor]
        public PortfolioCurrencies(List<PortfolioCurrency> currencies)
        {
            Currencies = currencies;
        }

        public class PortfolioCurrency
        {
            public Currency Currency { get; }
            public decimal Balance { get; }
            public decimal Blocked { get; }

            [JsonConstructor]
            public PortfolioCurrency(Currency currency, decimal balance, decimal blocked)
            {
                Currency = currency;
                Balance = balance;
                Blocked = blocked;
            }
        }
    }
}