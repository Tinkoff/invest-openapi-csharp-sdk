using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using RichardSzalay.MockHttp;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;
using Tinkoff.Trading.OpenApi.Tests.TestHelpers;
using Xunit;

namespace Tinkoff.Trading.OpenApi.Tests
{
    public class SandboxContextTests
    {
        public SandboxContextTests()
        {
            _handler = new MockHttpMessageHandler();
            _handler.Fallback.Throw();
            _context = new SandboxConnection(BaseUri, WebSocketBaseUri, Token, _handler.ToHttpClient()).Context;
        }

        private const string BaseUri = "https://api-invest.tinkoff.ru/openapi/sandbox/";
        private const string WebSocketBaseUri = "wss://api-invest.tinkoff.ru/openapi/md/v1/md-openapi/ws";
        private const string Token = "";
        private const string BrokerAccountId = "SB000000";

        private readonly MockHttpMessageHandler _handler;
        private readonly SandboxContext _context;

        [Fact]
        public async Task ClearTest()
        {
            _handler.Expect(HttpMethod.Post, $"{BaseUri}sandbox/clear")
                .WithQueryString("brokerAccountId", BrokerAccountId)
                .WithoutContent()
                .RespondJsonFromFile("ok");

            await _context.ClearAsync(BrokerAccountId);
        }

        [Fact]
        public async Task RegisterTest()
        {
            _handler.Expect(HttpMethod.Post, $"{BaseUri}sandbox/register")
                .WithJsonContentFromFile("sandbox-register-request")
                .RespondJsonFromFile("sandbox-register-response");

            var response = await _context.RegisterAsync(BrokerAccountType.Tinkoff);

            var expectedResponse = new SandboxAccount(BrokerAccountType.Tinkoff, BrokerAccountId);
            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task RemoveTest()
        {
            _handler.Expect(HttpMethod.Post, $"{BaseUri}sandbox/remove")
                .WithQueryString("brokerAccountId", BrokerAccountId)
                .WithoutContent()
                .RespondJsonFromFile("ok");

            await _context.RemoveAsync(BrokerAccountId);
        }

        [Fact]
        public async Task SetCurrencyBalanceTest()
        {
            _handler.Expect(HttpMethod.Post, $"{BaseUri}sandbox/currencies/balance")
                .WithQueryString("brokerAccountId", BrokerAccountId)
                .WithJsonContentFromFile("set-currency-balance-request")
                .RespondJsonFromFile("ok");

            await _context.SetCurrencyBalanceAsync(Currency.Usd, 100.5m, BrokerAccountId);
        }

        [Fact]
        public async Task SetPositionBalanceTest()
        {
            _handler.Expect(HttpMethod.Post, $"{BaseUri}sandbox/positions/balance")
                .WithQueryString("brokerAccountId", BrokerAccountId)
                .WithJsonContentFromFile("set-position-balance-request")
                .RespondJsonFromFile("ok");

            await _context.SetPositionBalanceAsync("BBG000CL9VN6", 100.7m, BrokerAccountId);
        }
    }
}
