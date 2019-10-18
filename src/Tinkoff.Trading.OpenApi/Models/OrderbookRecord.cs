using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class OrderbookRecord
    {
        public int Quantity { get; }
        public decimal Price { get; }

        [JsonConstructor]
        public OrderbookRecord(int quantity, decimal price)
        {
            Quantity = quantity;
            Price = price;
        }
    }
}