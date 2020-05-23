using Newtonsoft.Json.Serialization;

namespace Tinkoff.Trading.OpenApi.Models
{
    /// <summary>
    /// Данные счета песочницы
    /// </summary>
    public class SandboxAccount
    {
        public SandboxAccount(BrokerAccountType brokerAccountType, string brokerAccountId)
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
