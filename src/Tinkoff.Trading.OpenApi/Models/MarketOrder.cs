namespace Tinkoff.Trading.OpenApi.Models
{
    public class MarketOrder
    {
        public string Figi { get; }
        public int Lots { get; }
        public OperationType Operation { get; }
        public string BrokerAccountId { get; }

        public MarketOrder(string figi, int lots, OperationType operation, string brokerAccountId = null)
        {
            Figi = figi;
            Lots = lots;
            Operation = operation;
            BrokerAccountId = brokerAccountId;
        }
    }
}
