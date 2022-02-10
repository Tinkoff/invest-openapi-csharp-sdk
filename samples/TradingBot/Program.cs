#if NETCOREAPP3_1

using System.IO;
using System.Threading.Tasks;

namespace TradingBot
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var token = (await File.ReadAllTextAsync("token.txt")).Trim();
            await using var bot = new SandboxBot(token);
            await bot.StartAsync();
        }
    }
}

#endif

#if NET6_0_OR_GREATER

using TradingBot;

var token = (await File.ReadAllTextAsync("token.txt")).Trim();
await using var bot = new SandboxBot(token);
await bot.StartAsync();

#endif