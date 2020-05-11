using System.Net.Http;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;

namespace Tinkoff.Trading.OpenApi.Tests.TestHelpers
{
    public class JsonContentMatcher : IMockedRequestMatcher
    {
        private readonly JToken _expectedJson;

        public JsonContentMatcher(string json)
        {
            _expectedJson = JToken.Parse(json);
        }

        public bool Matches(HttpRequestMessage message)
        {
            if (message.Content == null || message.Content.Headers.ContentType.MediaType != "application/json")
                return false;
            var content = message.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JToken.DeepEquals(_expectedJson, JToken.Parse(content));
        }
    }
}
