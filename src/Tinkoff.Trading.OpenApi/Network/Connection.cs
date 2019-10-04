using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tinkoff.Trading.OpenApi.Models;

namespace Tinkoff.Trading.OpenApi.Network
{
    public abstract class Connection<TContext> : IConnection<TContext>, IDisposable
        where TContext : IContext
    {
        private readonly Uri _baseUri;
        private readonly Uri _webSocketBaseUri;
        private readonly string _token;
        private readonly HttpClient _httpClient;
        private ClientWebSocket _webSocket;

        protected Connection(string baseUri, string webSocketBaseUri, string token, HttpClient httpClient)
        {
            _baseUri = new Uri(baseUri);
            _webSocketBaseUri = new Uri(webSocketBaseUri);
            _token = token;
            _httpClient = httpClient;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public abstract TContext Context { get; }

        public event EventHandler<StreamingEventReceivedEventArgs> StreamingEventReceived;

        public async Task<OpenApiResponse<TPayload>> SendGetRequestAsync<TPayload>(string path)
        {
            var uri = new Uri(_baseUri, path);
            var response = await _httpClient.GetAsync(uri).ConfigureAwait(false);

            return await HandleResponseAsync<TPayload>(response).ConfigureAwait(false);
        }

        public async Task<OpenApiResponse<TOut>> SendPostRequestAsync<TIn, TOut>(string path, TIn payload)
        {
            var uri = new Uri(_baseUri, path);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            if (payload != null)
            {
                var body = JsonConvert.SerializeObject(payload);
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            return await HandleResponseAsync<TOut>(response).ConfigureAwait(false);
        }

        public async Task SendStreamingRequestAsync<TRequest>(TRequest request)
            where TRequest : StreamingRequest
        {
            await EnsureWebSocketConnectionAsync().ConfigureAwait(false);

            var requestJson = JsonConvert.SerializeObject(request);
            var data = Encoding.UTF8.GetBytes(requestJson);
            var buffer = new ArraySegment<byte>(data);
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);
        }

        private static async Task<OpenApiResponse<TOut>> HandleResponseAsync<TOut>(HttpResponseMessage response)
        {
            string content;
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<OpenApiResponse<TOut>>(content);
                case HttpStatusCode.Unauthorized:
                    throw new OpenApiException("You have no access to that resource.", "Access Denied");
                default:
                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var openApiResponse = JsonConvert.DeserializeObject<OpenApiResponse<OpenApiExceptionPayload>>(content);
                    throw new OpenApiException(openApiResponse.Payload.Message, openApiResponse.Payload.Code, openApiResponse.TrackingId);
            }
        }

        private async Task EnsureWebSocketConnectionAsync()
        {
            if (_webSocket != null) return;

            if (Interlocked.CompareExchange(ref _webSocket, new ClientWebSocket(), null) != null) return;

            _webSocket.Options.SetRequestHeader("Authorization", $"Bearer {_token}");
            await _webSocket.ConnectAsync(_webSocketBaseUri, CancellationToken.None).ConfigureAwait(false);

            Task.Run(async () =>
            {
                var transferBuffer = new byte[8096];
                var messageBuffer = new List<byte>();
                while (_webSocket.State == WebSocketState.Open)
                {
                    var buffer = new ArraySegment<byte>(transferBuffer);
                    var result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Close:
                            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Ok", CancellationToken.None);
                            _webSocket.Dispose();
                            _webSocket = null;
                            return;
                        case WebSocketMessageType.Text:
                            var receivedBytes = new byte[result.Count];
                            Array.ConstrainedCopy(buffer.Array, 0, receivedBytes, 0, result.Count);
                            messageBuffer.AddRange(receivedBytes);
                            if (result.EndOfMessage)
                            {
                                var data = Encoding.UTF8.GetString(messageBuffer.ToArray());
                                var response = JsonConvert.DeserializeObject<StreamingResponse>(data);
                                OnStreamingEvent(response);
                                messageBuffer.Clear();
                            }
                            break;
                    }
                }
            });
        }

        private void OnStreamingEvent(StreamingResponse response)
        {
            var handler = StreamingEventReceived;
            handler?.Invoke(this, new StreamingEventReceivedEventArgs(response));
        }

        public void Dispose()
        {
            _webSocket?.Dispose();
        }
    }

    public class Connection : Connection<Context>
    {
        public Connection(string baseUri, string webSocketBaseUri, string token, HttpClient httpClient)
            : base(baseUri, webSocketBaseUri, token, httpClient)
        {
        }

        public override Context Context => new Context(this);
    }
}