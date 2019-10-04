using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;
using Xunit;

namespace Tinkoff.Trading.OpenApi.Tests
{
    public class ContextTests
    {
        private const string BaseUri = "https://api-invest.tinkoff.ru/openapi/";
        private const string WebSocketBaseUri = "wss://api-invest.tinkoff.ru/openapi/md/v1/md-openapi/ws";
        private const string Token = "";

        [Fact]
        public async Task OrdersTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":[{\"orderId\":\"12345687\",\"figi\":\"BBG000CL9VN6\",\"operation\":\"Buy\",\"status\":\"New\",\"requestedLots\":10,\"executedLots\":0,\"type\":\"Limit\",\"price\":288.7}]}");
            var connection = new Connection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            var orders = await context.OrdersAsync();

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Get, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}orders"), handler.RequestMessage.RequestUri);
            Assert.Null(handler.RequestMessage.Content);

            var expected = new List<Order>
            {
                new Order(
                    "12345687",
                    "BBG000CL9VN6",
                    OperationType.Buy,
                    OrderStatus.New,
                    10,
                    0,
                    OrderType.Limit,
                    288.7m)
            };
            Assert.Equal(expected.Count, orders.Count);

            for (var i = 0; i < orders.Count; ++i)
            {
                var expectedOrder = expected[i];
                var actualOrder = orders[i];
                Assert.Equal(expectedOrder.OrderId, actualOrder.OrderId);
                Assert.Equal(expectedOrder.Figi, actualOrder.Figi);
                Assert.Equal(expectedOrder.Operation, actualOrder.Operation);
                Assert.Equal(expectedOrder.Status, actualOrder.Status);
                Assert.Equal(expectedOrder.RequestedLots, actualOrder.RequestedLots);
                Assert.Equal(expectedOrder.ExecutedLots, actualOrder.ExecutedLots);
                Assert.Equal(expectedOrder.Type, actualOrder.Type);
                Assert.Equal(expectedOrder.Price, actualOrder.Price);
            }
        }

        [Fact]
        public async Task PlaceLimitOrderTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{\"orderId\":\"12345687\",\"operation\":\"Sell\",\"status\":\"New\",\"rejectReason\":\"That's Why\",\"requestedLots\":10,\"executedLots\":0,\"commission\":{\"currency\":\"USD\",\"value\":1.44}}}");
            var connection = new Connection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            var placedLimitOrder = await context.PlaceLimitOrderAsync(new LimitOrder("BBG000CL9VN6", 10, OperationType.Sell, 288.3m));

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Post, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}orders/limit-order?figi=BBG000CL9VN6"), handler.RequestMessage.RequestUri);
            Assert.NotNull(handler.RequestMessage.Content);

            var content = await handler.RequestMessage.Content.ReadAsStringAsync();
            Assert.Equal("{\"lots\":10,\"operation\":\"Sell\",\"price\":288.3}", content);

            Assert.NotNull(placedLimitOrder);

            var expected = new PlacedLimitOrder(
                "12345687",
                OperationType.Sell,
                OrderStatus.New,
                "That's Why",
                10,
                0,
                new MoneyAmount(Currency.Usd, 1.44m));
            Assert.Equal(expected.OrderId, placedLimitOrder.OrderId);
            Assert.Equal(expected.Operation, placedLimitOrder.Operation);
            Assert.Equal(expected.Status, placedLimitOrder.Status);
            Assert.Equal(expected.RejectReason, placedLimitOrder.RejectReason);
            Assert.Equal(expected.RequestedLots, placedLimitOrder.RequestedLots);
            Assert.Equal(expected.ExecutedLots, placedLimitOrder.ExecutedLots);
            Assert.Equal(expected.Commission.Value, placedLimitOrder.Commission.Value);
            Assert.Equal(expected.Commission.Currency, placedLimitOrder.Commission.Currency);
        }

        [Fact]
        public async Task CancelOrderTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{}}");
            var connection = new Connection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            const string orderId = "12345687";
            await context.CancelOrderAsync(orderId);

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Post, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}orders/cancel?orderId={orderId}"), handler.RequestMessage.RequestUri);
            Assert.Null(handler.RequestMessage.Content);
        }

        [Fact]
        public async Task PortfolioTest()
        {
            const string response = "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{\"positions\":[{\"figi\":\"BBG000CL9VN6\",\"ticker\":\"NFLX\",\"isin\":\"US0004026250\",\"instrumentType\":\"Stock\",\"balance\":10,\"blocked\":5,\"expectedYield\":{\"currency\":\"USD\",\"value\":10},\"lots\":10}]}}";
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, response);
            var connection = new Connection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            var portfolio = await context.PortfolioAsync();

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Get, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}portfolio"), handler.RequestMessage.RequestUri);
            Assert.Null(handler.RequestMessage.Content);

            Assert.NotNull(portfolio);

            var expected = new Portfolio(new List<Portfolio.Position>
            {
                new Portfolio.Position(
                    "BBG000CL9VN6",
                    "NFLX",
                    "US0004026250",
                    InstrumentType.Stock,
                    10,
                    5,
                    new MoneyAmount(Currency.Usd, 10),
                    10)
            });
            Assert.Equal(expected.Positions.Count, portfolio.Positions.Count);

            for (var i = 0; i < expected.Positions.Count; ++i)
            {
                var expectedPosition = expected.Positions[i];
                var actualPosition = portfolio.Positions[i];
                Assert.Equal(expectedPosition.Figi, actualPosition.Figi);
                Assert.Equal(expectedPosition.Ticker, actualPosition.Ticker);
                Assert.Equal(expectedPosition.Isin, actualPosition.Isin);
                Assert.Equal(expectedPosition.InstrumentType, actualPosition.InstrumentType);
                Assert.Equal(expectedPosition.Balance, actualPosition.Balance);
                Assert.Equal(expectedPosition.Blocked, actualPosition.Blocked);
                Assert.Equal(expectedPosition.ExpectedYield.Value, actualPosition.ExpectedYield.Value);
                Assert.Equal(expectedPosition.ExpectedYield.Currency, actualPosition.ExpectedYield.Currency);
                Assert.Equal(expectedPosition.Lots, actualPosition.Lots);
            }
        }

        [Fact]
        public async Task PortfolioCurrenciesTest()
        {
            const string response = "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{\"currencies\":[{\"currency\":\"EUR\",\"balance\":200.7,\"blocked\":100}]}}";
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, response);
            var connection = new Connection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            var portfolio = await context.PortfolioCurrenciesAsync();

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Get, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}portfolio/currencies"), handler.RequestMessage.RequestUri);
            Assert.Null(handler.RequestMessage.Content);

            Assert.NotNull(portfolio);

            var expected = new PortfolioCurrencies(new List<PortfolioCurrencies.PortfolioCurrency>
            {
                new PortfolioCurrencies.PortfolioCurrency(Currency.Eur, 200.7m, 100m)
            });
            Assert.Equal(expected.Currencies.Count, portfolio.Currencies.Count);

            for (var i = 0; i < expected.Currencies.Count; ++i)
            {
                var expectedPosition = expected.Currencies[i];
                var actualPosition = portfolio.Currencies[i];
                Assert.Equal(expectedPosition.Currency, actualPosition.Currency);
                Assert.Equal(expectedPosition.Balance, actualPosition.Balance);
                Assert.Equal(expectedPosition.Blocked, actualPosition.Blocked);
            }
        }

        [Fact]
        public async Task MarketStocksTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{\"instruments\":[{\"figi\":\"BBG000BR37X2\",\"ticker\":\"PGR\",\"isin\":\"US7433151039\",\"minPriceIncrement\":0.01,\"lot\":1,\"currency\":\"USD\"}],\"total\":1}}");
            var connection = new Connection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            var stocks = await context.MarketStocksAsync();

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Get, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}market/stocks"), handler.RequestMessage.RequestUri);
            Assert.Null(handler.RequestMessage.Content);

            Assert.NotNull(stocks);

            var expected = new MarketInstrumentList(1, new List<MarketInstrument>
            {
                new MarketInstrument("BBG000BR37X2", "PGR", "US7433151039", 0.01m, 1, Currency.Usd)
            });
            Assert.Equal(expected.Total, stocks.Total);
            Assert.Equal(expected.Instruments.Count, stocks.Instruments.Count);

            for (var i = 0; i < expected.Instruments.Count; ++i)
            {
                var expectedInstrument = expected.Instruments[i];
                var actualInstrument = stocks.Instruments[i];
                Assert.Equal(expectedInstrument.Figi, actualInstrument.Figi);
                Assert.Equal(expectedInstrument.Ticker, actualInstrument.Ticker);
                Assert.Equal(expectedInstrument.Isin, actualInstrument.Isin);
                Assert.Equal(expectedInstrument.MinPriceIncrement, actualInstrument.MinPriceIncrement);
                Assert.Equal(expectedInstrument.Lot, actualInstrument.Lot);
                Assert.Equal(expectedInstrument.Currency, actualInstrument.Currency);
            }
        }

        [Fact]
        public async Task MarketBondsTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{\"instruments\":[{\"figi\":\"BBG00FFH2SJ8\",\"ticker\":\"SU24019RMFS0\",\"isin\":\"RU000A0JX0J2\",\"minPriceIncrement\":0.001,\"lot\":1,\"currency\":\"RUB\"}],\"total\":1}}");
            var connection = new Connection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            var stocks = await context.MarketBondsAsync();

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Get, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}market/bonds"), handler.RequestMessage.RequestUri);
            Assert.Null(handler.RequestMessage.Content);

            Assert.NotNull(stocks);

            var expected = new MarketInstrumentList(1, new List<MarketInstrument>
            {
                new MarketInstrument("BBG00FFH2SJ8", "SU24019RMFS0", "RU000A0JX0J2", 0.001m, 1, Currency.Rub)
            });
            Assert.Equal(expected.Total, stocks.Total);
            Assert.Equal(expected.Instruments.Count, stocks.Instruments.Count);

            for (var i = 0; i < expected.Instruments.Count; ++i)
            {
                var expectedInstrument = expected.Instruments[i];
                var actualInstrument = stocks.Instruments[i];
                Assert.Equal(expectedInstrument.Figi, actualInstrument.Figi);
                Assert.Equal(expectedInstrument.Ticker, actualInstrument.Ticker);
                Assert.Equal(expectedInstrument.Isin, actualInstrument.Isin);
                Assert.Equal(expectedInstrument.MinPriceIncrement, actualInstrument.MinPriceIncrement);
                Assert.Equal(expectedInstrument.Lot, actualInstrument.Lot);
                Assert.Equal(expectedInstrument.Currency, actualInstrument.Currency);
            }
        }

        [Fact]
        public async Task MarketEtfsTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{\"instruments\":[{\"figi\":\"BBG005HM5979\",\"ticker\":\"FXJP\",\"isin\":\"IE00BD3QJ310\",\"minPriceIncrement\":1,\"lot\":1,\"currency\":\"RUB\"}],\"total\":1}}");
            var connection = new Connection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            var stocks = await context.MarketEtfsAsync();

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Get, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}market/etfs"), handler.RequestMessage.RequestUri);
            Assert.Null(handler.RequestMessage.Content);

            Assert.NotNull(stocks);

            var expected = new MarketInstrumentList(1, new List<MarketInstrument>
            {
                new MarketInstrument("BBG005HM5979", "FXJP", "IE00BD3QJ310", 1, 1, Currency.Rub)
            });
            Assert.Equal(expected.Total, stocks.Total);
            Assert.Equal(expected.Instruments.Count, stocks.Instruments.Count);

            for (var i = 0; i < expected.Instruments.Count; ++i)
            {
                var expectedInstrument = expected.Instruments[i];
                var actualInstrument = stocks.Instruments[i];
                Assert.Equal(expectedInstrument.Figi, actualInstrument.Figi);
                Assert.Equal(expectedInstrument.Ticker, actualInstrument.Ticker);
                Assert.Equal(expectedInstrument.Isin, actualInstrument.Isin);
                Assert.Equal(expectedInstrument.MinPriceIncrement, actualInstrument.MinPriceIncrement);
                Assert.Equal(expectedInstrument.Lot, actualInstrument.Lot);
                Assert.Equal(expectedInstrument.Currency, actualInstrument.Currency);
            }
        }

        [Fact]
        public async Task MarketCurrenciesTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{\"instruments\":[{\"figi\":\"BBG0013HGFT4\",\"ticker\":\"USD000UTSTOM\",\"minPriceIncrement\":0.0025,\"lot\":1000,\"currency\":\"RUB\"},{\"figi\":\"BBG0013HJJ31\",\"ticker\":\"EUR_RUB__TOM\",\"minPriceIncrement\":0.0025,\"lot\":1000,\"currency\":\"RUB\"}],\"total\":2}}");
            var connection = new Connection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            var stocks = await context.MarketCurrenciesAsync();

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Get, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}market/currencies"), handler.RequestMessage.RequestUri);
            Assert.Null(handler.RequestMessage.Content);

            Assert.NotNull(stocks);

            var expected = new MarketInstrumentList(2, new List<MarketInstrument>
            {
                new MarketInstrument("BBG0013HGFT4", "USD000UTSTOM", null, 0.0025m, 1000, Currency.Rub),
                new MarketInstrument("BBG0013HJJ31", "EUR_RUB__TOM", null, 0.0025m, 1000, Currency.Rub)
            });
            Assert.Equal(expected.Total, stocks.Total);
            Assert.Equal(expected.Instruments.Count, stocks.Instruments.Count);

            for (var i = 0; i < expected.Instruments.Count; ++i)
            {
                var expectedInstrument = expected.Instruments[i];
                var actualInstrument = stocks.Instruments[i];
                Assert.Equal(expectedInstrument.Figi, actualInstrument.Figi);
                Assert.Equal(expectedInstrument.Ticker, actualInstrument.Ticker);
                Assert.Equal(expectedInstrument.Isin, actualInstrument.Isin);
                Assert.Equal(expectedInstrument.MinPriceIncrement, actualInstrument.MinPriceIncrement);
                Assert.Equal(expectedInstrument.Lot, actualInstrument.Lot);
                Assert.Equal(expectedInstrument.Currency, actualInstrument.Currency);
            }
        }

        [Fact]
        public async Task MarketSearchByFigiTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{\"figi\":\"BBG000CL9VN6\",\"ticker\":\"NFLX\",\"isin\":\"US64110L1061\",\"minPriceIncrement\":0.01,\"lot\":1,\"currency\":\"USD\"}}");
            var connection = new Connection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            var instrument = await context.MarketSearchByFigiAsync("BBG000CL9VN6");

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Get, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}market/search/by-figi?figi=BBG000CL9VN6"), handler.RequestMessage.RequestUri);
            Assert.Null(handler.RequestMessage.Content);

            Assert.NotNull(instrument);

            var expected = new MarketInstrument("BBG000CL9VN6", "NFLX", "US64110L1061", 0.01m, 1, Currency.Usd);

            Assert.Equal(expected.Figi, instrument.Figi);
            Assert.Equal(expected.Ticker, instrument.Ticker);
            Assert.Equal(expected.Isin, instrument.Isin);
            Assert.Equal(expected.MinPriceIncrement, instrument.MinPriceIncrement);
            Assert.Equal(expected.Lot, instrument.Lot);
            Assert.Equal(expected.Currency, instrument.Currency);
        }

        [Fact]
        public async Task MarketSearchByTickerTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":{\"figi\":\"BBG000CL9VN6\",\"ticker\":\"NFLX\",\"isin\":\"US64110L1061\",\"minPriceIncrement\":0.01,\"lot\":1,\"currency\":\"USD\"}}");
            var connection = new Connection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            var instrument = await context.MarketSearchByTickerAsync("NFLX");

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Get, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}market/search/by-ticker?ticker=NFLX"), handler.RequestMessage.RequestUri);
            Assert.Null(handler.RequestMessage.Content);

            Assert.NotNull(instrument);

            var expected = new MarketInstrument("BBG000CL9VN6", "NFLX", "US64110L1061", 0.01m, 1, Currency.Usd);

            Assert.Equal(expected.Figi, instrument.Figi);
            Assert.Equal(expected.Ticker, instrument.Ticker);
            Assert.Equal(expected.Isin, instrument.Isin);
            Assert.Equal(expected.MinPriceIncrement, instrument.MinPriceIncrement);
            Assert.Equal(expected.Lot, instrument.Lot);
            Assert.Equal(expected.Currency, instrument.Currency);
        }

        [Fact]
        public async Task OperationsTest()
        {
            var handler = new HttpMessageHandlerStub(HttpStatusCode.OK, "{\"trackingId\":\"QBASTAN\",\"status\":\"OK\",\"payload\":[{\"id\":\"12345687\",\"status\":\"Done\",\"trades\":[{\"tradeId\":\"12345687\",\"date\":\"2019-08-19T18:38:33.131642+03:00\",\"price\":100.3,\"quantity\":15}],\"commission\":{\"currency\":\"RUB\",\"value\":21},\"currency\":\"RUB\",\"payment\":1504.5,\"price\":100.3,\"quantity\":15,\"figi\":\"BBG000CL9VN6\",\"instrumentType\":\"Stock\",\"isMarginCall\":true,\"date\":\"2019-08-19T18:38:33.131642+03:00\",\"operationType\":\"Buy\"}]}");
            var connection = new Connection(BaseUri, WebSocketBaseUri, Token, new HttpClient(handler));
            var context = connection.Context;
            var operations = await context.OperationsAsync(DateTime.Parse("2019-08-19T18:38:33.1316420+03:00"), Interval.Day, "BBG000CL9VN6");

            Assert.NotNull(handler.RequestMessage);
            Assert.Equal(HttpMethod.Get, handler.RequestMessage.Method);
            Assert.Equal(new Uri($"{BaseUri}operations?from={HttpUtility.UrlEncode("2019-08-19")}&interval=1day&figi=BBG000CL9VN6"), handler.RequestMessage.RequestUri);
            Assert.Null(handler.RequestMessage.Content);

            Assert.NotNull(operations);

            var expected = new List<Operation>
            {
                new Operation(
                    "12345687",
                    OperationStatus.Done,
                    new List<Trade>
                    {
                        new Trade("12345687", DateTime.Parse("2019-08-19T18:38:33.131642+03:00"), 100.3m, 15)
                    },
                    new MoneyAmount(Currency.Rub, 21),
                    Currency.Rub,
                    1504.5m,
                    100.3m,
                    15,
                    "BBG000CL9VN6",
                    InstrumentType.Stock,
                    true,
                    DateTime.Parse("2019-08-19T18:38:33.131642+03:00"),
                    ExtendedOperationType.Buy)
            };
            Assert.Equal(expected.Count, operations.Count);

            for (var i = 0; i < expected.Count; ++i)
            {
                var expectedOperation = expected[i];
                var actualOperation = operations[i];
                Assert.Equal(expectedOperation.Id, actualOperation.Id);
                Assert.Equal(expectedOperation.Status, actualOperation.Status);
                Assert.Equal(expectedOperation.Commission.Value, actualOperation.Commission.Value);
                Assert.Equal(expectedOperation.Commission.Currency, actualOperation.Commission.Currency);
                Assert.Equal(expectedOperation.Currency, actualOperation.Currency);
                Assert.Equal(expectedOperation.Payment, actualOperation.Payment);
                Assert.Equal(expectedOperation.Price, actualOperation.Price);
                Assert.Equal(expectedOperation.Quantity, actualOperation.Quantity);
                Assert.Equal(expectedOperation.Figi, actualOperation.Figi);
                Assert.Equal(expectedOperation.InstrumentType, actualOperation.InstrumentType);
                Assert.Equal(expectedOperation.IsMarginCall, actualOperation.IsMarginCall);
                Assert.Equal(expectedOperation.Date, actualOperation.Date);
                Assert.Equal(expectedOperation.OperationType, actualOperation.OperationType);

                Assert.Equal(expectedOperation.Trades.Count, actualOperation.Trades.Count);
                for (var j = 0; j < expectedOperation.Trades.Count; ++j)
                {
                    var expectedTrade = expectedOperation.Trades[j];
                    var actualTrade = actualOperation.Trades[j];
                    Assert.Equal(expectedTrade.TradeId, actualTrade.TradeId);
                    Assert.Equal(expectedTrade.Date, actualTrade.Date);
                    Assert.Equal(expectedTrade.Price, actualTrade.Price);
                    Assert.Equal(expectedTrade.Quantity, actualTrade.Quantity);
                }
            }
        }
    }
}