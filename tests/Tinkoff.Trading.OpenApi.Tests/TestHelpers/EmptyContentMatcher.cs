using System.Net.Http;
using RichardSzalay.MockHttp;

namespace Tinkoff.Trading.OpenApi.Tests.TestHelpers
{
    public class EmptyContentMatcher : IMockedRequestMatcher
    {
        public bool Matches(HttpRequestMessage message)
        {
            return message.Content == null;
        }
    }
}
