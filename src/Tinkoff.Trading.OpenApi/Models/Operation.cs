using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class Operation
    {
        public string Id { get; }
        public OperationStatus Status { get; }
        public List<Trade> Trades { get; }
        public MoneyAmount Commission { get; }
        public Currency Currency { get; }
        public decimal Payment { get; }
        public decimal Price { get; }
        public int Quantity { get; }
        public int? QuantityExecuted { get; }
        public string Figi { get; }
        public InstrumentType InstrumentType { get; }
        public bool IsMarginCall { get; }
        public DateTime Date { get; }
        public ExtendedOperationType OperationType { get; }

        [JsonConstructor]
        public Operation(string id, OperationStatus status, List<Trade> trades, MoneyAmount commission, Currency currency, decimal payment, decimal price, int quantity, int? quantityExecuted, string figi, InstrumentType instrumentType, bool isMarginCall, DateTime date, ExtendedOperationType operationType)
        {
            Id = id;
            Status = status;
            Trades = trades;
            Commission = commission;
            Currency = currency;
            Payment = payment;
            Price = price;
            Quantity = quantity;
            QuantityExecuted = quantityExecuted;
            Figi = figi;
            InstrumentType = instrumentType;
            IsMarginCall = isMarginCall;
            Date = date;
            OperationType = operationType;
        }
    }
}