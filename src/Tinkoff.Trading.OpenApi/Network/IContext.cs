using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;

namespace Tinkoff.Trading.OpenApi.Network
{
    /// <summary>
    /// Контекст для работы с OpenAPI
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Событие, возникающее при получении сообщения от WebSocket-клиента.
        /// </summary>
        event EventHandler<StreamingEventReceivedEventArgs> StreamingEventReceived;

        /// <summary>
        /// Получение списка активных заявок.
        /// </summary>
        /// <returns>Список заявок.</returns>
        Task<List<Order>> OrdersAsync();

        /// <summary>
        /// Размещение лимитной заявки.
        /// </summary>
        /// <param name="limitOrder">Параметры отправляемой заявки.</param>
        /// <returns>Параметры размещённой заявки.</returns>
        Task<PlacedLimitOrder> PlaceLimitOrderAsync(LimitOrder limitOrder);

        /// <summary>
        /// Отзыв лимитной заявки.
        /// </summary>
        /// <param name="id">Идентификатор заявки.</param>
        Task CancelOrderAsync(string id);

        /// <summary>
        /// Получение информации по портфелю инструментов.
        /// </summary>
        /// <returns>Портфель инструментов.</returns>
        Task<Portfolio> PortfolioAsync();

        /// <summary>
        /// Получение информации по валютным активам.
        /// </summary>
        /// <returns>Валютные активы.</returns>
        Task<PortfolioCurrencies> PortfolioCurrenciesAsync();

        /// <summary>
        /// Получение списка акций, доступных для торговли.
        /// </summary>
        /// <returns>Список акций.</returns>
        Task<MarketInstrumentList> MarketStocksAsync();

        /// <summary>
        /// Получение списка бондов, доступных для торговли.
        /// </summary>
        /// <returns>Список бондов.</returns>
        Task<MarketInstrumentList> MarketBondsAsync();

        /// <summary>
        /// Получение списка фондов, доступных для торговли.
        /// </summary>
        /// <returns>Список фондов.</returns>
        Task<MarketInstrumentList> MarketEtfsAsync();

        /// <summary>
        /// Получение списка валют, доступных для торговли.
        /// </summary>
        /// <returns>Список валют.</returns>
        Task<MarketInstrumentList> MarketCurrenciesAsync();

        /// <summary>
        /// Поиск инструмента по FIGI.
        /// </summary>
        /// <param name="figi">FIGI.</param>
        /// <returns></returns>
        Task<MarketInstrumentList> MarketSearchByFigiAsync(string figi);

        /// <summary>
        /// Поиск инструмента по тикеру.
        /// </summary>
        /// <param name="ticker">Тикер.</param>
        /// <returns></returns>
        Task<MarketInstrumentList> MarketSearchByTickerAsync(string ticker);

        /// <summary>
        /// Получение исторических значений свечей по FIGI.
        /// </summary>
        /// <param name="figi">FIGI.</param>
        /// <param name="from">Начало временного промежутка.</param>
        /// <param name="to">Конец временного промежутка.</param>
        /// <param name="interval">Интервал свечи.</param>
        /// <returns>Значения свечей.</returns>
        Task<CandleList> MarketCandlesAsync(string figi, DateTime from, DateTime to, CandleInterval interval);

        /// <summary>
        /// Получение стакана (книги заявок) по FIGI.
        /// </summary>
        /// <param name="figi">FIGI.</param>
        /// <param name="depth">Глубина стакана.</param>
        /// <returns>Книга заявок.</returns>
        Task<Orderbook> MarketOrderbookAsync(string figi, int depth);

        /// <summary>
        /// Получение списка операций.
        /// </summary>
        /// <param name="from">Начало временного промежутка.</param>
        /// <param name="to">Конец временного промежутка.</param>
        /// <param name="figi">FIGI инструмента для фильтрации.</param>
        /// <returns>Список операций.</returns>
        Task<List<Operation>> OperationsAsync(DateTime from, DateTime to, string figi);

        /// <summary>
        /// Получение списка операций.
        /// </summary>
        /// <param name="from">Начало временного промежутка.</param>
        /// <param name="interval">Длительность временного промежутка.</param>
        /// <param name="figi">FIGI инструмента для фильтрации.</param>
        /// <returns>Список операций.</returns>
        Task<List<Operation>> OperationsAsync(DateTime from, Interval interval, string figi);

        /// <summary>
        /// Посылает запрос по streaming-протоколу.
        /// </summary>
        /// <param name="request">Запрос.</param>
        /// <typeparam name="TRequest">Тип запроса.</typeparam>
        Task SendStreamingRequestAsync<T>(T request) where T : StreamingRequest;
    }
}