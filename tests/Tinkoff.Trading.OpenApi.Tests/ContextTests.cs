using System;
using System.Collections.Generic;
using System.Net;
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
    public class ContextTests
    {
        public ContextTests()
        {
            _handler = new MockHttpMessageHandler();
            _handler.Fallback.Throw();
            _context = new SandboxConnection(BaseUri, WebSocketBaseUri, Token, _handler.ToHttpClient()).Context;
        }

        private const string BaseUri = "https://api-invest.tinkoff.ru/openapi/";
        private const string WebSocketBaseUri = "wss://api-invest.tinkoff.ru/openapi/md/v1/md-openapi/ws";
        private const string Token = "";
        private const string BrokerAccountId = "SB000000";
        private const string Figi = "BBG000CL9VN6";

        private readonly MockHttpMessageHandler _handler;
        private readonly SandboxContext _context;

        [Fact]
        public async Task AccountsTest()
        {
            _handler.Expect(HttpMethod.Get, $"{BaseUri}user/accounts")
                .WithoutContent()
                .RespondJsonFromFile("accounts-response");

            var response = await _context.AccountsAsync();

            var expectedResponse = new object[]
            {
                new Account(BrokerAccountType.Tinkoff, "SB000000"),
                new Account(BrokerAccountType.TinkoffIis, "SB000001")
            };
            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task CancelOrderTest()
        {
            const string orderId = "12345687";
            _handler.Expect(HttpMethod.Post, $"{BaseUri}orders/cancel")
                .WithQueryString(new Dictionary<string, string>
                {
                    ["orderId"] = orderId,
                    ["brokerAccountId"] = BrokerAccountId
                })
                .WithoutContent()
                .RespondJsonFromFile("ok");

            await _context.CancelOrderAsync(orderId, BrokerAccountId);
        }

        [Fact]
        public async Task MarketBondsTest()
        {
            _handler.Expect(HttpMethod.Get, $"{BaseUri}market/bonds")
                .WithoutContent()
                .RespondJsonFromFile("market-bonds-response");

            var bonds = await _context.MarketBondsAsync();

            var expected = new MarketInstrumentList(1, new List<MarketInstrument>
            {
                new MarketInstrument("BBG00FFH2SJ8", "SU24019RMFS0", "RU000A0JX0J2", 0.001m, 1, Currency.Rub,
                    "ОФЗ 24019", InstrumentType.Bond)
            });

            bonds.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task MarketCandlesTest()
        {
            const string figi = Figi;
            const string from = "2019-10-17T18:38:33.1316420Z";
            const string to = "2019-10-17T18:39:33.1316420Z";
            _handler.Expect(HttpMethod.Get, $"{BaseUri}market/candles")
                .WithQueryString(new Dictionary<string, string>
                {
                    ["figi"] = figi,
                    ["from"] = from,
                    ["to"] = to,
                    ["interval"] = "1min"
                })
                .WithoutContent()
                .RespondJsonFromFile("market-candles-response");


            var candles = await _context.MarketCandlesAsync(figi,
                DateTime.Parse(from).ToUniversalTime(),
                DateTime.Parse(to).ToUniversalTime(), CandleInterval.Minute);

            var expected = new CandleList(figi, CandleInterval.Minute, new List<CandlePayload>
            {
                new CandlePayload(299.5m, 298.87m, 299.59m, 298.8m, 18887,
                    DateTime.Parse("2019-10-17T15:39Z").ToUniversalTime(), CandleInterval.Minute, figi)
            });

            candles.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task MarketCurrenciesTest()
        {
            _handler.Expect(HttpMethod.Get, $"{BaseUri}market/currencies")
                .WithoutContent()
                .RespondJsonFromFile("market-currencies-response");

            var currencies = await _context.MarketCurrenciesAsync();

            var expected = new MarketInstrumentList(2, new List<MarketInstrument>
            {
                new MarketInstrument("BBG0013HGFT4", "USD000UTSTOM", null, 0.0025m, 1000, Currency.Rub, "Доллар США",
                    InstrumentType.Currency),
                new MarketInstrument("BBG0013HJJ31", "EUR_RUB__TOM", null, 0.0025m, 1000, Currency.Rub, "Евро",
                    InstrumentType.Currency)
            });

            currencies.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task MarketEtfsTest()
        {
            _handler.Expect(HttpMethod.Get, $"{BaseUri}market/etfs")
                .WithoutContent()
                .RespondJsonFromFile("market-etfs-response");

            var etfs = await _context.MarketEtfsAsync();

            var expected = new MarketInstrumentList(1, new List<MarketInstrument>
            {
                new MarketInstrument("BBG005HM5979", "FXJP", "IE00BD3QJ310", 1, 1, Currency.Rub,
                    "Акции японских компаний", InstrumentType.Etf)
            });

            etfs.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task MarketOrderbookTest()
        {
            _handler.Expect(HttpMethod.Get, $"{BaseUri}market/orderbook")
                .WithQueryString(new Dictionary<string, string>
                {
                    ["figi"] = Figi,
                    ["depth"] = "5"
                })
                .WithoutContent()
                .RespondJsonFromFile("market-orderbook-response");

            var orderbook = await _context.MarketOrderbookAsync(Figi, 5);

            var expected = new Orderbook(
                5,
                new List<OrderbookRecord>
                {
                    new OrderbookRecord(37, 293.44m),
                    new OrderbookRecord(9, 293.42m),
                    new OrderbookRecord(1, 293.4m),
                    new OrderbookRecord(35, 293.1m),
                    new OrderbookRecord(1, 293.09m)
                },
                new List<OrderbookRecord>
                {
                    new OrderbookRecord(1, 293.69m),
                    new OrderbookRecord(1, 293.72m),
                    new OrderbookRecord(35, 293.79m),
                    new OrderbookRecord(1, 293.8m),
                    new OrderbookRecord(20, 293.87m)
                },
                Figi,
                TradeStatus.NormalTrading,
                0.01m,
                293.55m,
                293.55m,
                293.35m,
                307.5m,
                280.73m);

            orderbook.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task MarketSearchByFigiTest()
        {
            const string figi = Figi;
            _handler.Expect(HttpMethod.Get, $"{BaseUri}market/search/by-figi")
                .WithQueryString("figi", figi)
                .WithoutContent()
                .RespondJsonFromFile("market-search-by-figi-response");

            var instrumentList = await _context.MarketSearchByFigiAsync(figi);

            var expected = new MarketInstrument(figi, "NFLX", "US64110L1061", 0.01m, 1, Currency.Usd, "Netflix", InstrumentType.Stock);

            instrumentList.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task MarketSearchByTickerTest()
        {
            const string ticker = "NFLX";
            _handler.Expect(HttpMethod.Get, $"{BaseUri}market/search/by-ticker")
                .WithQueryString("ticker", ticker)
                .WithoutContent()
                .RespondJsonFromFile("market-search-by-ticker-response");

            var instrumentList = await _context.MarketSearchByTickerAsync(ticker);

            var expected = new MarketInstrumentList(1, new List<MarketInstrument>
            {
                new MarketInstrument(Figi, ticker, "US64110L1061", 0.01m, 1, Currency.Usd, "Netflix",
                    InstrumentType.Stock)
            });

            instrumentList.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task MarketStocksTest()
        {
            _handler.Expect(HttpMethod.Get, $"{BaseUri}market/stocks")
                .WithoutContent()
                .RespondJsonFromFile("market-stocks-response");

            var stocks = await _context.MarketStocksAsync();

            var expected = new MarketInstrumentList(1, new List<MarketInstrument>
            {
                new MarketInstrument("BBG000BR37X2", "PGR", "US7433151039", 0.01m, 1, Currency.Usd, "Progressive",
                    InstrumentType.Stock)
            });

            stocks.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task OperationsIntervalTest()
        {
            _handler.Expect(HttpMethod.Get, $"{BaseUri}operations")
                .WithQueryString(new Dictionary<string, string>
                {
                    ["figi"] = Figi,
                    ["from"] = "2019-08-19",
                    ["interval"] = "1day",
                    ["brokerAccountId"] = BrokerAccountId
                })
                .WithoutContent()
                .RespondJsonFromFile("operations-interval-response");

            var operations = await _context.OperationsAsync(DateTime.Parse("2019-08-19T18:38:33.1316420Z"),
                Interval.Day, Figi, BrokerAccountId);
            var expected = new List<Operation>
            {
                new Operation(
                    "12345687",
                    OperationStatus.Done,
                    new List<Trade>
                    {
                        new Trade("12345687", DateTime.Parse("2019-08-19T18:38:33.131642Z").ToUniversalTime(),
                            100.3m, 15)
                    },
                    new MoneyAmount(Currency.Rub, 21),
                    Currency.Rub,
                    1504.5m,
                    100.3m,
                    15,
                    Figi,
                    InstrumentType.Stock,
                    true,
                    DateTime.Parse("2019-08-19T18:38:33.131642Z").ToUniversalTime(),
                    ExtendedOperationType.Buy)
            };
            operations.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task OperationsRangeTest()
        {
            _handler.Expect(HttpMethod.Get, $"{BaseUri}operations")
                .WithQueryString(new Dictionary<string, string>
                {
                    ["figi"] = Figi,
                    ["from"] = "2019-08-19T18:38:33.1316420Z",
                    ["to"] = "2019-08-19T18:48:33.1316420Z",
                    ["brokerAccountId"] = BrokerAccountId
                })
                .WithoutContent()
                .RespondJsonFromFile("operations-range-response");

            var operations = await _context.OperationsAsync(
                DateTime.Parse("2019-08-19T18:38:33.1316420Z").ToUniversalTime(),
                DateTime.Parse("2019-08-19T18:48:33.1316420Z").ToUniversalTime(), Figi, BrokerAccountId);

            var expected = new List<Operation>
            {
                new Operation(
                    "12345687",
                    OperationStatus.Done,
                    new List<Trade>
                    {
                        new Trade("12345687", DateTime.Parse("2019-08-19T18:38:33.131642Z").ToUniversalTime(), 100.3m,
                            15)
                    },
                    new MoneyAmount(Currency.Rub, 21),
                    Currency.Rub,
                    1504.5m,
                    100.3m,
                    15,
                    Figi,
                    InstrumentType.Stock,
                    true,
                    DateTime.Parse("2019-08-19T18:38:33.131642Z").ToUniversalTime(),
                    ExtendedOperationType.Buy)
            };
            operations.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task OrdersTest()
        {
            _handler.Expect(HttpMethod.Get, $"{BaseUri}orders")
                .WithQueryString("brokerAccountId", BrokerAccountId)
                .WithoutContent()
                .RespondJsonFromFile("orders-response");

            var response = await _context.OrdersAsync(BrokerAccountId);

            var expected = new List<Order>
            {
                new Order(
                    "12345687",
                    Figi,
                    OperationType.Buy,
                    OrderStatus.New,
                    10,
                    0,
                    OrderType.Limit,
                    288.7m)
            };
            response.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task PlaceLimitOrderTest()
        {
            _handler.Expect(HttpMethod.Post, $"{BaseUri}orders/limit-order")
                .WithQueryString(new Dictionary<string, string>
                {
                    ["figi"] = Figi,
                    ["brokerAccountId"] = BrokerAccountId
                })
                .WithJsonContentFromFile("limit-order-request")
                .RespondJsonFromFile("limit-order-response");

            var placedLimitOrder = await _context.PlaceLimitOrderAsync(new LimitOrder(Figi, 10,
                OperationType.Sell, 288.3m, BrokerAccountId));

            var expected = new PlacedLimitOrder(
                "12345687",
                OperationType.Sell,
                OrderStatus.New,
                "That's Why",
                10,
                0,
                new MoneyAmount(Currency.Usd, 1.44m));

            placedLimitOrder.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task PlaceMarketOrderTest()
        {
            _handler.Expect(HttpMethod.Post, $"{BaseUri}orders/market-order")
                .WithQueryString(new Dictionary<string, string>
                {
                    ["figi"] = Figi,
                    ["brokerAccountId"] = BrokerAccountId
                })
                .WithJsonContentFromFile("market-order-request")
                .RespondJsonFromFile("market-order-response");

            var response =
                await _context.PlaceMarketOrderAsync(new MarketOrder(Figi, 10, OperationType.Buy,
                    BrokerAccountId));

            var expected = new PlacedMarketOrder(
                "40fcdfa2-084b-41bc-8f9d-3414a5eb9e8c",
                OperationType.Buy,
                OrderStatus.Fill,
                "That's Why",
                10,
                5,
                new MoneyAmount(Currency.Usd, 1.44m));

            response.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task PortfolioCurrenciesTest()
        {
            _handler.Expect(HttpMethod.Get, $"{BaseUri}portfolio/currencies")
                .WithQueryString("brokerAccountId", BrokerAccountId)
                .WithoutContent()
                .RespondJsonFromFile("portfolio-currencies-response");

            var currencies = await _context.PortfolioCurrenciesAsync(BrokerAccountId);

            var expected = new PortfolioCurrencies(new List<PortfolioCurrencies.PortfolioCurrency>
            {
                new PortfolioCurrencies.PortfolioCurrency(Currency.Eur, 200.7m, 100m),
                new PortfolioCurrencies.PortfolioCurrency(Currency.Jpy, 42.2m, 0m)
            });

            currencies.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task PortfolioTest()
        {
            _handler.Expect(HttpMethod.Get, $"{BaseUri}portfolio")
                .WithQueryString("brokerAccountId", BrokerAccountId)
                .WithoutContent()
                .RespondJsonFromFile("portfolio-response");

            var portfolio = await _context.PortfolioAsync(BrokerAccountId);

            var expected = new Portfolio(new List<Portfolio.Position>
            {
                new Portfolio.Position(
                    "name",
                    Figi,
                    "NFLX",
                    "US0004026250",
                    InstrumentType.Stock,
                    10,
                    5,
                    new MoneyAmount(Currency.Usd, 10),
                    10,
                    new MoneyAmount(Currency.Usd, 295.4m),
                    new MoneyAmount(Currency.Usd, 292.4m))
            });

            portfolio.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task InvalidResponseTest()
        {
            var invalidJson = "Invalid JSON";
            _handler.Expect(HttpMethod.Get, $"{BaseUri}portfolio/currencies")
                .WithQueryString("brokerAccountId", BrokerAccountId)
                .Respond("text/html", invalidJson);

            var exception = await Assert.ThrowsAsync<OpenApiInvalidResponseException>(() => _context.PortfolioCurrenciesAsync(BrokerAccountId));

            Assert.NotNull(exception.InnerException);
            Assert.Equal($"Unable to handle response.\nResponse:\n{invalidJson}", exception.Message);
        }

        [Fact]
        public async Task UnauthorizedResponseTest()
        {
            _handler.Expect(HttpMethod.Get, $"{BaseUri}portfolio/currencies")
                .WithQueryString("brokerAccountId", BrokerAccountId)
                .Respond(HttpStatusCode.Unauthorized);

            await Assert.ThrowsAsync<OpenApiException>(() => _context.PortfolioCurrenciesAsync(BrokerAccountId));
        }

        [Fact]
        public async Task TooManyRequestsResponseTest()
        {
            _handler.Expect(HttpMethod.Get, $"{BaseUri}portfolio/currencies")
                .WithQueryString("brokerAccountId", BrokerAccountId)
                .Respond(HttpStatusCode.TooManyRequests);

            var exception = await Assert.ThrowsAsync<OpenApiException>(() => _context.PortfolioCurrenciesAsync(BrokerAccountId));

            Assert.Equal(HttpStatusCode.TooManyRequests, exception.HttpStatusCode);
        }
    }
}
