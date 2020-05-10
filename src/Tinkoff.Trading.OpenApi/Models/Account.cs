namespace Tinkoff.Trading.OpenApi.Models
{
    /// <summary>
    /// Данные счета
    /// </summary>
    public class Account
    {
        public Account(BrokerAccountType brokerAccountType, string brokerAccountId)
        {
            BrokerAccountType = brokerAccountType;
            BrokerAccountId = brokerAccountId;
        }

        /// <summary>
        /// Тип счета
        /// </summary>
        public BrokerAccountType BrokerAccountType { get; }

        /// <summary>
        /// Id счета
        /// </summary>
        public string BrokerAccountId { get; }
    }
}