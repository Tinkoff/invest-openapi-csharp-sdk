using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tinkoff.Trading.OpenApi.Models;

namespace Tinkoff.Trading.OpenApi.Network
{
    public class SandboxContext : Context, ISandboxContext
    {
        public SandboxContext(IConnection<Context> connection)
            : base(connection)
        {
        }

        public async Task<SandboxAccount> RegisterAsync(BrokerAccountType? brokerAccountType)
        {
            var request = brokerAccountType.HasValue ? new SandboxRegisterRequest(brokerAccountType.Value) : null;
            var response = await Connection
                .SendPostRequestAsync<SandboxRegisterRequest, SandboxAccount>(Endpoints.Register, request)
                .ConfigureAwait(false);
            return response?.Payload;
        }

        public async Task SetCurrencyBalanceAsync(Currency currency, decimal balance, string brokerAccountId = null)
        {
            var endpoint = AppendQueryParams(Endpoints.CurrenciesBalance, (BrokerAccountId, brokerAccountId));
            await Connection
                .SendPostRequestAsync<CurrencyBalance, EmptyPayload>(endpoint, new CurrencyBalance(currency, balance))
                .ConfigureAwait(false);
        }

        public async Task SetPositionBalanceAsync(string figi, decimal balance, string brokerAccountId = null)
        {
            var endpoint = AppendQueryParams(Endpoints.PositionsBalance, (BrokerAccountId, brokerAccountId));
            await Connection
                .SendPostRequestAsync<PositionBalance, EmptyPayload>(endpoint, new PositionBalance(figi, balance))
                .ConfigureAwait(false);
        }

        public async Task RemoveAsync(string brokerAccountId = null)
        {
            var endpoint = AppendQueryParams(Endpoints.Remove, (BrokerAccountId, brokerAccountId));
            await Connection
                .SendPostRequestAsync<object, EmptyPayload>(endpoint, null)
                .ConfigureAwait(false);
        }

        public async Task ClearAsync(string brokerAccountId = null)
        {
            var endpoint = AppendQueryParams(Endpoints.Clear, (BrokerAccountId, brokerAccountId));
            await Connection
                .SendPostRequestAsync<object, EmptyPayload>(endpoint, null)
                .ConfigureAwait(false);
        }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        private class SandboxRegisterRequest
        {
            public SandboxRegisterRequest(BrokerAccountType brokerAccountType)
            {
                BrokerAccountType = brokerAccountType;
            }

            public BrokerAccountType BrokerAccountType { get; }
        }

        private class CurrencyBalance
        {
            [JsonProperty(PropertyName = "currency")]
            public Currency Currency { get; }

            [JsonProperty(PropertyName = "balance")]
            public decimal Balance { get; }

            public CurrencyBalance(Currency currency, decimal balance)
            {
                Currency = currency;
                Balance = balance;
            }
        }

        private class PositionBalance
        {
            [JsonProperty(PropertyName = "figi")]
            public string Figi { get; }

            [JsonProperty(PropertyName = "balance")]
            public decimal Balance { get; }

            public PositionBalance(string figi, decimal balance)
            {
                Figi = figi;
                Balance = balance;
            }
        }

        private static class Endpoints
        {
            public const string Register = "sandbox/register";
            public const string CurrenciesBalance = "sandbox/currencies/balance";
            public const string PositionsBalance = "sandbox/positions/balance";
            public const string Remove = "sandbox/remove";
            public const string Clear = "sandbox/clear";
        }
    }
}
