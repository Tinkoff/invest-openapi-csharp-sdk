﻿using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class PlacedMarketOrder
    {
        public string OrderId { get; }
        public OperationType Operation { get; }
        public OrderStatus Status { get; }
        public string RejectReason { get; }
        public int RequestedLots { get; }
        public int ExecutedLots { get; }
        public MoneyAmount Commission { get; }

        [JsonConstructor]
        public PlacedMarketOrder(string orderId, OperationType operation, OrderStatus status, string rejectReason, int requestedLots, int executedLots, MoneyAmount commission)
        {
            OrderId = orderId;
            Operation = operation;
            Status = status;
            RejectReason = rejectReason;
            RequestedLots = requestedLots;
            ExecutedLots = executedLots;
            Commission = commission;
        }
    }
}
