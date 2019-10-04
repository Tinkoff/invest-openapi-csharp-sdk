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
        /// Регистрацияя в песочнице.
        /// </summary>
        Task RegisterAsync();

        /// <summary>
        /// Установка значения валютного актива.
        /// </summary>
        /// <param name="currency">Валюта.</param>
        /// <param name="balance">Желаемое значение.</param>
        Task SetCurrencyBalanceAsync(Currency currency, decimal balance);

        /// <summary>
        /// Установка позиции по инструменту.
        /// </summary>
        /// <param name="figi">FIGI.</param>
        /// <param name="balance">Желаемое значение.</param>
        Task SetPositionBalanceAsync(string figi, decimal balance);

        /// <summary>
        /// Сброс всех установленных значений по активам.
        /// </summary>
        Task ClearAsync();
    }
}