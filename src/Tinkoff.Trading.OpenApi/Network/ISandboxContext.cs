using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;

namespace Tinkoff.Trading.OpenApi.Network
{
    /// <summary>
    /// Контекст для работы с OpenAPI в режиме песочницы.
    /// </summary>
    public interface ISandboxContext : IContext
    {
        /// <summary>
        /// Регистрация в песочнице.
        /// </summary>
        /// <param name="brokerAccountType">
        /// Тип счета.
        /// </param>
        Task<SandboxAccount> RegisterAsync(BrokerAccountType? brokerAccountType);

        /// <summary>
        /// Установка значения валютного актива.
        /// </summary>
        /// <param name="currency">Валюта.</param>
        /// <param name="balance">Желаемое значение.</param>
        /// <param name="brokerAccountId">Номер счета (по умолчанию - Тинькофф).</param>
        Task SetCurrencyBalanceAsync(Currency currency, decimal balance, string brokerAccountId = null);

        /// <summary>
        /// Установка позиции по инструменту.
        /// </summary>
        /// <param name="figi">FIGI.</param>
        /// <param name="balance">Желаемое значение.</param>
        /// <param name="brokerAccountId">Номер счета (по умолчанию - Тинькофф).</param>
        Task SetPositionBalanceAsync(string figi, decimal balance, string brokerAccountId = null);

        /// <summary>
        /// Удаление счета клиента.
        /// </summary>
        /// <param name="brokerAccountId">Номер счета (по умолчанию - Тинькофф).</param>
        Task RemoveAsync(string brokerAccountId = null);

        /// <summary>
        /// Сброс всех установленных значений по активам.
        /// </summary>
        /// <param name="brokerAccountId">Номер счета (по умолчанию - Тинькофф).</param>
        Task ClearAsync(string brokerAccountId = null);
    }
}
