using System.Threading.Tasks;
using Newtonsoft.Json;
using Tinkoff.Trading.OpenApi.Models;

namespace Tinkoff.Trading.OpenApi.Network
{
    public class SandboxContext : Context, ISandboxContext
    {
        public SandboxContext(IConnection<Context> connection)
            : base(connection)
        {
        }

        public async Task RegisterAsync()
        {
            await Connection.SendPostRequestAsync<object, EmptyPayload>(Endpoints.Register, null).ConfigureAwait(false);
        }

        public async Task SetCurrencyBalanceAsync(Currency currency, decimal balance)
        {
            await Connection.SendPostRequestAsync<CurrencyBalance, EmptyPayload>(Endpoints.CurrenciesBalance, new CurrencyBalance(currency, balance)).ConfigureAwait(false);
        }

        public async Task SetPositionBalanceAsync(string figi, decimal balance)
        {
            await Connection.SendPostRequestAsync<PositionBalance, EmptyPayload>(Endpoints.PositionsBalance, new PositionBalance(figi, balance)).ConfigureAwait(false);
        }

        public async Task ClearAsync()
        {
            await Connection.SendPostRequestAsync<object, EmptyPayload>(Endpoints.Clear, null).ConfigureAwait(false);
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
            public const string Clear = "sandbox/clear";
        }
    }
}