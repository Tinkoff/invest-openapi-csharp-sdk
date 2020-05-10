using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Json;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;
using Tinkoff.Trading.OpenApi.Tests.Json;
using Xunit;

namespace Tinkoff.Trading.OpenApi.Tests
{
    public class SandboxContextTests
    {
        private const string BaseUri = "https://api-invest.tinkoff.ru/openapi/sandbox/";
        private const string WebSocketBaseUri = "wss://api-invest.tinkoff.ru/openapi/md/v1/md-openapi/ws";
        private const string Token = "";
        private const string BrokerAccountId = "SB000000";

        [Fact]
        public async Task RegisterTest()
        {
            var jsonRequest = JsonFile.Read("sandbox-register-request");
            var jsonResponse = JsonFile.Read("sandbox-register-response");
            var (handler, context) = CreateStubs(jsonResponse);

            var response = await context.RegisterAsync(BrokerAccountType.Tinkoff);

            handler.RequestMessage.Method.Should().Be(HttpMethod.Post);
            handler.RequestMessage.RequestUri.Should().Be(new Uri($"{BaseUri}sandbox/register"));
            var request = await handler.RequestMessage.Content.ReadAsStringAsync();
            request.Should().BeValidJson().Which.Should().BeEquivalentTo(jsonRequest);
            var expectedResponse = new SandboxAccount(BrokerAccountType.Tinkoff, BrokerAccountId);
            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task SetCurrencyBalanceTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{}}");
            var connection = new SandboxConnection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            await context.SetCurrencyBalanceAsync(Currency.Usd, 100.5m, BrokerAccountId);

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Post, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}sandbox/currencies/balance?brokerAccountId={BrokerAccountId}"), handler.RequestMessage.RequestUri);
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
            await context.SetPositionBalanceAsync("BBG000CL9VN6", 100.7m, BrokerAccountId);

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Post, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}sandbox/positions/balance?brokerAccountId={BrokerAccountId}"), handler.RequestMessage.RequestUri);
            Assert.NotNull(handler.RequestMessage.Content);

            var content = await handler.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("{\"figi\":\"BBG000CL9VN6\",\"balance\":100.7}", content);
        }

        [Fact]
        public async Task RemoveTest()
        {
            var (handler, context) = CreateStubs(JsonFile.Read("ok"));
            
            await context.RemoveAsync(BrokerAccountId);

            handler.RequestMessage.Should().NotBeNull();
            handler.RequestMessage.Method.Should().Be(HttpMethod.Post);
            handler.RequestMessage.RequestUri.Should()
                .Be(new Uri($"{BaseUri}sandbox/remove?brokerAccountId={BrokerAccountId}"));
            handler.RequestMessage.Content.Should().BeNull();
        }
        
        [Fact]
        public async Task ClearTest()
        {
            var (handler, context) = CreateStubs(JsonFile.Read("ok"));
            
            await context.ClearAsync(BrokerAccountId);

            handler.RequestMessage.Should().NotBeNull();
            handler.RequestMessage.Method.Should().Be(HttpMethod.Post);
            handler.RequestMessage.RequestUri.Should()
                .Be(new Uri($"{BaseUri}sandbox/clear?brokerAccountId={BrokerAccountId}"));
            handler.RequestMessage.Content.Should().BeNull();
        }

        private static (HttpMessageHandlerStub handler, SandboxContext context) CreateStubs(string jsonResponse)
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, jsonResponse);
            var connection = new SandboxConnection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            return (handler, connection.Context);
        }
    }
}
