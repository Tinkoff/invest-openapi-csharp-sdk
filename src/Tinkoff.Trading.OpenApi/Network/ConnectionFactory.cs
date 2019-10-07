using System.Net.Http;

namespace Tinkoff.Trading.OpenApi.Network
{
    public class ConnectionFactory
    {
        private const string BaseUri = "https://api-invest.tinkoff.ru/openapi/";
        private const string SandboxBaseUri = "https://api-invest.tinkoff.ru/openapi/sandbox/";
        private const string WebSocketBaseUri = "wss://api-invest.tinkoff.ru/openapi/md/v1/md-openapi/ws";
        
        /// <summary>
        /// Создаёт объект подключения к OpenAPI.
        /// </summary>
        /// <param name="token">Токен аутентификации.</param>
        /// <returns>Подключение к бирже.</returns>
        public static Connection GetConnection(string token)
        {
            return new Connection(BaseUri, WebSocketBaseUri, token, new HttpClient());
        }

        /// <summary>
        /// Создаёт объект подключения к OpenAPI в режиме песочницы.
        /// </summary>
        /// <param name="token">Токен аутентификации.</param>
        /// <returns>Подключение к бирже в режиме песочницы.</returns>
        public static SandboxConnection GetSandboxConnection(string token)
        {
            return new SandboxConnection(SandboxBaseUri, WebSocketBaseUri, token, new HttpClient());
        }
    }
}