using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Tinkoff.Trading.OpenApi.Models;

namespace Tinkoff.Trading.OpenApi.Network
{
    public class Context : IContext
    {
        protected readonly IConnection<Context> Connection;

        public Context(IConnection<Context> connection)
        {
            Connection = connection;
        }

        public async Task<List<Models.Order>> OrdersAsync()
        {
            var response = await Connection.SendGetRequestAsync<List<Models.Order>>(Endpoints.Orders).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<PlacedLimitOrder> PlaceLimitOrderAsync(LimitOrder limitOrder)
        {
            var figiParam = HttpUtility.UrlEncode(limitOrder.Figi);
            var path = $"{Endpoints.OrdersLimitOrder}?figi={figiParam}";
            var body = new Order(limitOrder.Lots, limitOrder.Operation, limitOrder.Price);
            var response = await Connection.SendPostRequestAsync<Order, PlacedLimitOrder>(path, body).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task CancelOrderAsync(string id)
        {
            var idParam = HttpUtility.UrlEncode(id);
            var path = $"{Endpoints.OrdersCancel}?orderId={idParam}";
            await Connection.SendPostRequestAsync<object, EmptyPayload>(path, null).ConfigureAwait(false);
        }

        public async Task<Portfolio> PortfolioAsync()
        {
            var response = await Connection.SendGetRequestAsync<Portfolio>(Endpoints.Portfolio).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<PortfolioCurrencies> PortfolioCurrenciesAsync()
        {
            var response = await Connection.SendGetRequestAsync<PortfolioCurrencies>(Endpoints.PortfolioCurrencies).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<MarketInstrumentList> MarketStocksAsync()
        {
            var response = await Connection.SendGetRequestAsync<MarketInstrumentList>(Endpoints.MarketStocks).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<MarketInstrumentList> MarketBondsAsync()
        {
            var response = await Connection.SendGetRequestAsync<MarketInstrumentList>(Endpoints.MarketBonds).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<MarketInstrumentList> MarketEtfsAsync()
        {
            var response = await Connection.SendGetRequestAsync<MarketInstrumentList>(Endpoints.MarketEtfs).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<MarketInstrumentList> MarketCurrenciesAsync()
        {
            var response = await Connection.SendGetRequestAsync<MarketInstrumentList>(Endpoints.MarketCurrencies).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<MarketInstrument> MarketSearchByFigiAsync(string figi)
        {
            var figiParam = HttpUtility.UrlEncode(figi);
            var path = $"{Endpoints.MarketSearchByFigi}?figi={figiParam}";
            var response = await Connection.SendGetRequestAsync<MarketInstrument>(path).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<MarketInstrument> MarketSearchByTickerAsync(string ticker)
        {
            var tickerParam = HttpUtility.UrlEncode(ticker);
            var path = $"{Endpoints.MarketSearchByTicker}?ticker={tickerParam}";
            var response = await Connection.SendGetRequestAsync<MarketInstrument>(path).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<List<Operation>> OperationsAsync(DateTime from, Interval interval, string figi)
        {
            var fromParam = HttpUtility.UrlEncode(from.ToString("yyyy-MM-dd"));
            var intervalParam = interval.ToParamString();
            var figiParam = HttpUtility.UrlEncode(figi);
            var path = $"{Endpoints.Operations}?from={fromParam}&interval={intervalParam}&figi={figiParam}";
            var response = await Connection.SendGetRequestAsync<List<Operation>>(path).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task SendStreamingRequestAsync<T>(T request)
            where T : StreamingRequest
        {
            await Connection.SendStreamingRequestAsync(request).ConfigureAwait(false);
        }

        protected class EmptyPayload
        {
        }

        private class Order
        {
            [JsonProperty(PropertyName = "lots")]
            public int Lots { get; }

            [JsonProperty(PropertyName = "operation")]
            public OperationType Operation { get; }

            [JsonProperty(PropertyName = "price")]
            public decimal Price { get; }

            public Order(int lots, OperationType operation, decimal price)
            {
                Lots = lots;
                Operation = operation;
                Price = price;
            }
        }

        private static class Endpoints
        {
            public const string Orders = "orders";
            public const string OrdersLimitOrder = "orders/limit-order";
            public const string OrdersCancel = "orders/cancel";
            public const string Portfolio = "portfolio";
            public const string PortfolioCurrencies = "portfolio/currencies";
            public const string MarketStocks = "market/stocks";
            public const string MarketBonds = "market/bonds";
            public const string MarketEtfs = "market/etfs";
            public const string MarketCurrencies = "market/currencies";
            public const string MarketSearchByTicker = "market/search/by-ticker";
            public const string MarketSearchByFigi = "market/search/by-figi";
            public const string Operations = "operations";
        }
    }
}