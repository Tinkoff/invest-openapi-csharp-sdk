using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
        private Task _webSocketTask;

        protected Connection(string baseUri, string webSocketBaseUri, string token, HttpClient httpClient)
        {
            _baseUri = new Uri(baseUri);
            _webSocketBaseUri = new Uri(webSocketBaseUri);
            _token = token;
            _httpClient = httpClient;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public Defaults Defaults { get; } = new Defaults();
        public abstract TContext Context { get; }

        public event EventHandler<StreamingEventReceivedEventArgs> StreamingEventReceived;
        public event EventHandler<WebSocketException> WebSocketException;
        public event EventHandler StreamingClosed;

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
                var body = JsonSerializer.Serialize(payload, payload.GetType(), SerializationOptions.Instance);
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            return await HandleResponseAsync<TOut>(response).ConfigureAwait(false);
        }

        public async Task SendStreamingRequestAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default(CancellationToken))
            where TRequest : StreamingRequest
        {
            await EnsureWebSocketConnectionAsync(cancellationToken).ConfigureAwait(false);

            var requestJson = JsonSerializer.Serialize(request, request.GetType(), SerializationOptions.Instance);
            var data = Encoding.UTF8.GetBytes(requestJson);
            var buffer = new ArraySegment<byte>(data);
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken)
                .ConfigureAwait(false);
        }

        private static async Task<OpenApiResponse<TOut>> HandleResponseAsync<TOut>(HttpResponseMessage response)
        {
            byte[] content = default;
            int contentLength = 0;
            var arrayPool = ArrayPool<byte>.Shared;
            try
            {
                if (response.Content?.Headers.ContentLength > 0)
                {
                    contentLength = (int) response.Content.Headers.ContentLength;
                    content = arrayPool.Rent(contentLength);
                    var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    await stream.ReadAsync(content, 0, contentLength).ConfigureAwait(false);
                }

                // .NET Standard 2.0 doesn't have too many requests status code
                // https://github.com/dotnet/runtime/issues/15650#issue-558031319
                const HttpStatusCode TooManyRequestsStatusCode = (HttpStatusCode) 429;
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return JsonSerializer.Deserialize<OpenApiResponse<TOut>>(content.AsSpan(0, contentLength),
                            SerializationOptions.Instance);
                    case HttpStatusCode.Unauthorized:
                        throw new OpenApiException("You have no access to that resource.", HttpStatusCode.Unauthorized);
                    case TooManyRequestsStatusCode:
                        throw new OpenApiException("Too many requests.", TooManyRequestsStatusCode);
                    default:
                        var openApiResponse =
                            JsonSerializer.Deserialize<OpenApiResponse<OpenApiExceptionPayload>>(content.AsSpan(0, contentLength),
                                SerializationOptions.Instance);
                        throw new OpenApiException(
                            openApiResponse.Payload.Message,
                            openApiResponse.Payload.Code,
                            openApiResponse.TrackingId,
                            response.StatusCode);
                }
            }
            catch (OpenApiException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new OpenApiInvalidResponseException("Unable to handle response.",
                    content == null ? string.Empty : Encoding.UTF8.GetString(content.AsSpan(0, contentLength).ToArray()), e);
            }
            finally
            {
                if (content != null)
                {
                    arrayPool.Return(content);
                }
            }
        }

        private async Task EnsureWebSocketConnectionAsync(CancellationToken cancellationToken)
        {
            if (_webSocket != null) return;

            if (Interlocked.CompareExchange(ref _webSocket, new ClientWebSocket(), null) != null) return;

            _webSocket.Options.SetRequestHeader("Authorization", $"Bearer {_token}");
            await _webSocket.ConnectAsync(_webSocketBaseUri, cancellationToken).ConfigureAwait(false);

            _webSocketTask = Task.Run(async () =>
            {
                var bufferCapacity = 8192;
                var transferBuffer = new byte[bufferCapacity];
                var messageBuffer = new MemoryStream(bufferCapacity);
                var messageLength = 0;
                try
                {
                    while (_webSocket.State == WebSocketState.Open)
                    {
                        var buffer = new ArraySegment<byte>(transferBuffer);
                        var result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);

                        switch (result.MessageType)
                        {
                            case WebSocketMessageType.Close:
                                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Ok",
                                    CancellationToken.None);
                                StreamingClosed?.Invoke(this, EventArgs.Empty);
                                return;
                            case WebSocketMessageType.Text:
                                if (result.EndOfMessage)
                                {
                                    StreamingResponse response;

                                    // We can use buffer directly if we got a message without chunking
                                    // This is almost always the case
                                    if (messageLength == 0)
                                    {
                                        response = JsonSerializer.Deserialize<StreamingResponse>(
                                            buffer.Array.AsSpan(0, result.Count),
                                            SerializationOptions.Instance);
                                    }
                                    else
                                    {
                                        // ReSharper disable once AssignNullToNotNullAttribute
                                        messageBuffer.Write(buffer.Array, 0, result.Count);
                                        messageLength += result.Count;

                                        response = JsonSerializer.Deserialize<StreamingResponse>(
                                            messageBuffer.GetBuffer().AsSpan(0, messageLength),
                                            SerializationOptions.Instance);

                                        messageBuffer.Position = 0;
                                        messageLength = 0;
                                    }

                                    OnStreamingEvent(response);
                                }
                                else
                                {
                                    // ReSharper disable once AssignNullToNotNullAttribute
                                    messageBuffer.Write(buffer.Array, 0, result.Count);
                                    messageLength += result.Count;
                                }


                                break;
                        }
                    }
                }
                catch (WebSocketException e)
                {
                    WebSocketException?.Invoke(this, e);
                }
                finally
                {
                    _webSocket?.Dispose();
                    _webSocket = null;
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
