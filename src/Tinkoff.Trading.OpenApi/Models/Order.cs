using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class Order
    {
        public string OrderId { get; }
        public string Figi { get; }
        public OperationType Operation { get; }
        public OrderStatus Status { get; }
        public int RequestedLots { get; }
        public int ExecutedLots { get; }
        public OrderType Type { get; }
        public decimal Price { get; }

        [JsonConstructor]
        public Order(string orderId, string figi, OperationType operation, OrderStatus status, int requestedLots, int executedLots, OrderType type, decimal price)
        {
            OrderId = orderId;
            Figi = figi;
            Operation = operation;
            Status = status;
            RequestedLots = requestedLots;
            ExecutedLots = executedLots;
            Type = type;
            Price = price;
        }
    }
}