namespace Tinkoff.Trading.OpenApi.Models
{
    public class LimitOrder
    {
        public string Figi { get; }
        public int Lots { get; }
        public OperationType Operation { get; }
        public decimal Price { get; }

        public LimitOrder(string figi, int lots, OperationType operation, decimal price)
        {
            Figi = figi;
            Lots = lots;
            Operation = operation;
            Price = price;
        }
    }
}