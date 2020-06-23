using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Tinkoff.Trading.OpenApi.Models;

namespace Tinkoff.Trading.OpenApi.Network
{
    public class Context : IContext, IDisposable
    {
        protected const string BrokerAccountId = "brokerAccountId";
        
        protected readonly IConnection<Context> Connection;

        public event EventHandler<StreamingEventReceivedEventArgs> StreamingEventReceived;
        public event EventHandler<WebSocketException> WebSocketException;
        public event EventHandler StreamingClosed;

        public Context(IConnection<Context> connection)
        {
            Connection = connection;
            Connection.StreamingEventReceived += OnStreamingEventReceived;
            Connection.WebSocketException += OnWebSocketException;
            Connection.StreamingClosed += OnStreamingClosed;
        }

        public async Task<IReadOnlyCollection<Account>> AccountsAsync()
        {
            var response = await Connection.SendGetRequestAsync<AccountsList>(Endpoints.UserAccounts)
                .ConfigureAwait(false);
            return response?.Payload?.Accounts;
        }

        public async Task<List<Order>> OrdersAsync(string brokerAccountId = null)
        {
            var endpoint = AppendQueryParams(Endpoints.Orders, (BrokerAccountId, brokerAccountId));
            var response = await Connection.SendGetRequestAsync<List<Order>>(endpoint)
                .ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<PlacedLimitOrder> PlaceLimitOrderAsync(LimitOrder limitOrder)
        {
            var endpoint = AppendQueryParams(Endpoints.OrdersLimitOrder, 
                ("figi", limitOrder.Figi), (BrokerAccountId, limitOrder.BrokerAccountId));
            var body = new LimitOrderBody(limitOrder.Lots, limitOrder.Operation, limitOrder.Price);
            var response = await Connection.SendPostRequestAsync<LimitOrderBody, PlacedLimitOrder>(endpoint, body)
                .ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<PlacedMarketOrder> PlaceMarketOrderAsync(MarketOrder marketOrder)
        {
            var endpoint = AppendQueryParams(Endpoints.OrdersMarketOrder, 
                ("figi", marketOrder.Figi), (BrokerAccountId, marketOrder.BrokerAccountId));
            var body = new MarketOrderBody(marketOrder.Lots, marketOrder.Operation);
            var response = await Connection.SendPostRequestAsync<MarketOrderBody, PlacedMarketOrder>(endpoint, body)
                .ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task CancelOrderAsync(string id, string brokerAccountId = null)
        {  
            var endpoint = AppendQueryParams(Endpoints.OrdersCancel, 
                ("orderId", id), (BrokerAccountId, brokerAccountId));
            await Connection.SendPostRequestAsync<object, EmptyPayload>(endpoint, null)
                .ConfigureAwait(false);
        }

        public async Task<Portfolio> PortfolioAsync(string brokerAccountId = null)
        {
            var endpoint = AppendQueryParams(Endpoints.Portfolio, (BrokerAccountId, brokerAccountId));
            var response = await Connection.SendGetRequestAsync<Portfolio>(endpoint)
                .ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<PortfolioCurrencies> PortfolioCurrenciesAsync(string brokerAccountId = null)
        {
            var endpoint = AppendQueryParams(Endpoints.PortfolioCurrencies, (BrokerAccountId, brokerAccountId));
            var response = await Connection.SendGetRequestAsync<PortfolioCurrencies>(endpoint)
                .ConfigureAwait(false);
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

        public async Task<MarketInstrumentList> MarketSearchByTickerAsync(string ticker)
        {
            var tickerParam = HttpUtility.UrlEncode(ticker);
            var path = $"{Endpoints.MarketSearchByTicker}?ticker={tickerParam}";
            var response = await Connection.SendGetRequestAsync<MarketInstrumentList>(path).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<CandleList> MarketCandlesAsync(string figi, DateTime from, DateTime to,
            CandleInterval interval)
        {
            var endpoint = AppendQueryParams(Endpoints.MarketCandles, 
                ("figi", figi),
                ("from", FormatDateTime(from)),
                ("to", FormatDateTime(to)),
                ("interval", interval.GetEnumMemberValue()));
            var response = await Connection.SendGetRequestAsync<CandleList>(endpoint)
                .ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<Orderbook> MarketOrderbookAsync(string figi, int depth)
        {
            var figiParam = HttpUtility.UrlEncode(figi);
            var path = $"{Endpoints.MarketOrderbook}?figi={figiParam}&depth={depth.ToString()}";
            var response = await Connection.SendGetRequestAsync<Orderbook>(path).ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task<List<Operation>> OperationsAsync(DateTime from, DateTime to, string figi,
            string brokerAccountId = null)
        {
            var path = AppendQueryParams(Endpoints.Operations, ("from", FormatDateTime(from)),
                ("to", FormatDateTime(to)), ("figi", figi), (BrokerAccountId, brokerAccountId));
            var response = await Connection.SendGetRequestAsync<OperationList>(path)
                .ConfigureAwait(false);
            return response?.Payload?.Operations;
        }

        public async Task<List<Operation>> OperationsAsync(DateTime from, Interval interval, string figi,
            string brokerAccountId = null)
        {
            var path = AppendQueryParams(Endpoints.Operations, ("from", from.ToString("yyyy-MM-dd")),
                ("interval", interval.ToParamString()), ("figi", figi), (BrokerAccountId, brokerAccountId));
            var response = await Connection.SendGetRequestAsync<OperationList>(path)
                .ConfigureAwait(false);
            return response?.Payload?.Operations;
        }

        public async Task SendStreamingRequestAsync<T>(T request)
            where T : StreamingRequest
        {
            await Connection.SendStreamingRequestAsync(request).ConfigureAwait(false);
        }


        private void OnStreamingEventReceived(object sender, StreamingEventReceivedEventArgs args)
        {
            StreamingEventReceived?.Invoke(this, args);
        }

        private void OnWebSocketException(object sender, WebSocketException args)
        {
            WebSocketException?.Invoke(this, args);
        }

        private void OnStreamingClosed(object sender, EventArgs args)
        {
            StreamingClosed?.Invoke(this, args);
        }

        private string FormatDateTime(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                dateTime = DateTime.SpecifyKind(dateTime, Connection.Defaults.DateTimeKind);
            }

            return dateTime.ToString("O");
        }

        public void Dispose()
        {
            Connection.StreamingEventReceived -= OnStreamingEventReceived;
            Connection.WebSocketException -= OnWebSocketException;
            Connection.StreamingClosed -= OnStreamingClosed;
        }

        protected class EmptyPayload
        {
        }
        
        protected static string AppendQueryParams(string endpoint, params (string name, string value)[] queryParams)
        {
            StringBuilder sb = null;
            foreach (var (name, value) in queryParams)
            {
                if (string.IsNullOrWhiteSpace(value)) continue;
                if (sb == null)
                {
                    sb = new StringBuilder(endpoint);
                    sb.Append('?');
                }
                else
                {
                    sb.Append('&');
                }

                sb.Append(name).Append('=').Append(HttpUtility.UrlEncode(value));
            }

            return sb?.ToString() ?? endpoint;
        }
        
        private class AccountsList
        {
            public AccountsList(Account[] accounts)
            {
                Accounts = accounts;
            }

            public Account[] Accounts { get; }
        }

        private class LimitOrderBody
        {
            [JsonProperty(PropertyName = "lots")]
            public int Lots { get; }

            [JsonProperty(PropertyName = "operation")]
            public OperationType Operation { get; }

            [JsonProperty(PropertyName = "price")]
            public decimal Price { get; }

            public LimitOrderBody(int lots, OperationType operation, decimal price)
            {
                Lots = lots;
                Operation = operation;
                Price = price;
            }
        }

        private class MarketOrderBody
        {
            [JsonProperty(PropertyName = "lots")]
            public int Lots { get; }

            [JsonProperty(PropertyName = "operation")]
            public OperationType Operation { get; }

            public MarketOrderBody(int lots, OperationType operation)
            {
                Lots = lots;
                Operation = operation;
            }
        }

        private class OperationList
        {
            public List<Operation> Operations { get; }

            [JsonConstructor]
            public OperationList(List<Operation> operations)
            {
                Operations = operations;
            }
        }

        private static class Endpoints
        {
            public const string Orders = "orders";
            public const string OrdersLimitOrder = "orders/limit-order";
            public const string OrdersMarketOrder = "orders/market-order";
            public const string OrdersCancel = "orders/cancel";
            public const string Portfolio = "portfolio";
            public const string PortfolioCurrencies = "portfolio/currencies";
            public const string MarketStocks = "market/stocks";
            public const string MarketBonds = "market/bonds";
            public const string MarketEtfs = "market/etfs";
            public const string MarketCurrencies = "market/currencies";
            public const string MarketSearchByTicker = "market/search/by-ticker";
            public const string MarketSearchByFigi = "market/search/by-figi";
            public const string MarketCandles = "market/candles";
            public const string MarketOrderbook = "market/orderbook";
            public const string Operations = "operations";
            public const string UserAccounts = "user/accounts";
        }
    }
}
