using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;

namespace Tinkoff.Trading.OpenApi.Network
{
    /// <summary>
    /// Интерфейс подключения к OpenAPI.
    /// </summary>
    /// <typeparam name="TContext">Вид контекста.</typeparam>
    public interface IConnection<out TContext>
        where TContext : IContext
    {
        /// <summary>
        /// Контекст OpenAPI.
        /// </summary>
        TContext Context { get; }

        /// <summary>
        /// Значения по умолчанию.
        /// </summary>
        Defaults Defaults { get; }

        /// <summary>
        /// Событие, возникающее при получении сообщения от WebSocket-клиента.
        /// </summary>
        event EventHandler<StreamingEventReceivedEventArgs> StreamingEventReceived;

        /// <summary>
        /// Событие, возникающее при ошибке WebSocket-клиента (например, при обрыве связи).
        /// </summary>
        event EventHandler<WebSocketException> WebSocketException;

        /// <summary>
        /// Событие, возникающее при закрытии WebSocket соединения.
        /// </summary>
        event EventHandler StreamingClosed;

        /// <summary>
        /// Посылает GET-запрос к серверу и возвращает десериализованный ответ.
        /// </summary>
        /// <param name="path">Относительный путь.</param>
        /// <typeparam name="TPayload">Желаемый типа ответа.</typeparam>
        /// <returns></returns>
        Task<OpenApiResponse<TPayload>> SendGetRequestAsync<TPayload>(string path);

        /// <summary>
        /// Посылает POST-запрос к серверу и возвращает десериализованный ответ.
        /// </summary>
        /// <param name="path">Относительный путь.</param>
        /// <param name="payload">Тело запроса.</param>
        /// <typeparam name="TIn">Тип тела запроса.</typeparam>
        /// <typeparam name="TOut">Желаемый тип ответа.</typeparam>
        /// <returns></returns>
        Task<OpenApiResponse<TOut>> SendPostRequestAsync<TIn, TOut>(string path, TIn payload);

        /// <summary>
        /// Посылает запрос по streaming-протоколу.
        /// </summary>
        /// <param name="request">Запрос.</param>
        /// <typeparam name="TRequest">Тип запроса.</typeparam>
        Task SendStreamingRequestAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default(CancellationToken)) where TRequest : StreamingRequest;
    }
}
