using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;
using Xunit;

namespace Tinkoff.Trading.OpenApi.Tests
{
    public class SandboxContextTests
    {
        private const string BaseUri = "https://api-invest.tinkoff.ru/openapi/sandbox/";
        private const string WebSocketBaseUri = "wss://api-invest.tinkoff.ru/openapi/md/v1/md-openapi/ws";
        private const string Token = "";

        [Fact]
        public async Task RegisterTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{}}");
            var connection = new SandboxConnection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            await context.RegisterAsync();

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Post, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}sandbox/register"), handler.RequestMessage.RequestUri);
            Assert.Null(handler.RequestMessage.Content);
        }

        [Fact]
        public async Task SetCurrencyBalanceTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{}}");
            var connection = new SandboxConnection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            await context.SetCurrencyBalanceAsync(Currency.Usd, 100.5m);

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Post, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}sandbox/currencies/balance"), handler.RequestMessage.RequestUri);
            Assert.NotNull(handler.RequestMessage.Content);

            var content = await handler.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("{\"currency\":\"USD\",\"balance\":100.5}", content);
        }

        [Fact]
        public async Task SetPositionBalanceTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{}}");
            var connection = new SandboxConnection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            await context.SetPositionBalanceAsync("BBG000CL9VN6", 100.7m);

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Post, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}sandbox/positions/balance"), handler.RequestMessage.RequestUri);
            Assert.NotNull(handler.RequestMessage.Content);

            var content = await handler.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("{\"figi\":\"BBG000CL9VN6\",\"balance\":100.7}", content);
        }

        [Fact]
        public async Task ClearTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{}}");
            var connection = new SandboxConnection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            await context.ClearAsync();

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Post, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}sandbox/clear"), handler.RequestMessage.RequestUri);
            Assert.Null(handler.RequestMessage.Content);
        }
    }
}