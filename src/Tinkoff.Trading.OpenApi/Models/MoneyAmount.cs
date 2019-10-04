using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class MoneyAmount
    {
        public Currency Currency { get; }
        public decimal Value { get; }

        [JsonConstructor]
        public MoneyAmount(Currency currency, decimal value)
        {
            Currency = currency;
            Value = value;
        }
    }
}