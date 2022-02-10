#if NETCOREAPP3_1
using System;
using System.Threading.Tasks;
#endif
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace TradingBot
{
    public class SandboxBot : IAsyncDisposable
    {
        private static readonly Random Random = new Random();
        private readonly SandboxContext _context;
        private string _accountId;

        public SandboxBot(string token)
        {
            var connection = ConnectionFactory.GetSandboxConnection(token);
            _context = connection.Context;
        }

        public async Task StartAsync()
        {
            // register new sandbox account
            var sandboxAccount = await _context.RegisterAsync(BrokerAccountType.Tinkoff);
            _accountId = sandboxAccount.BrokerAccountId;

            // set balance
            foreach (var currency in new[] {Currency.Rub, Currency.Usd, Currency.Eur})
                await _context.SetCurrencyBalanceAsync(currency, Random.Next(1, 10) * 100_000,
                    sandboxAccount.BrokerAccountId);

            await CheckBalanceAsync();

            // select random instrument
            var instrumentList = await _context.MarketStocksAsync();
            var randomInstrumentIndex = Random.Next(instrumentList.Total);
            var randomInstrument = instrumentList.Instruments[randomInstrumentIndex];
            Console.WriteLine($"Selected Instrument:\n{randomInstrument.ToString().Replace(", ", "\n")}");
            Console.WriteLine();

            // get candles
            var now = DateTime.Now;
            var candleList = await _context.MarketCandlesAsync(randomInstrument.Figi, now.AddMinutes(-5), now, CandleInterval.Minute);
            foreach (var candle in candleList.Candles)
            {
                Console.WriteLine(candle);
            }
            Console.WriteLine();

            Console.WriteLine("Buy 1 lot");
            await _context.PlaceMarketOrderAsync(new MarketOrder(randomInstrument.Figi, 1, OperationType.Buy,
                _accountId));

            await CheckBalanceAsync();
            await Task.Delay(1000);

            Console.WriteLine("Sell 1 lot");
            await _context.PlaceMarketOrderAsync(new MarketOrder(randomInstrument.Figi, 1, OperationType.Sell,
                _accountId));

            await CheckBalanceAsync();
        }

        private async Task CheckBalanceAsync()
        {
            var portfolio = await _context.PortfolioCurrenciesAsync(_accountId);
            Console.WriteLine("Balance");
            foreach (var currency in portfolio.Currencies) Console.WriteLine($"{currency.Balance} {currency.Currency}");

            Console.WriteLine();
        }

        public async ValueTask DisposeAsync()
        {
            if (_accountId != null) await _context.RemoveAsync(_accountId);
        }
    }
}