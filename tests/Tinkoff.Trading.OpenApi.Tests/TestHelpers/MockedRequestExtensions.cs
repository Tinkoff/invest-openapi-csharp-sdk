using System;
using RichardSzalay.MockHttp;

namespace Tinkoff.Trading.OpenApi.Tests.TestHelpers
{
    public static class MockedRequestExtensions
    {
        public static MockedRequest WithoutContent(this MockedRequest source)
        {
            return source.With(new EmptyContentMatcher());
        }

        public static MockedRequest WithJsonContent(this MockedRequest source, string content)
        {
            return source.With(new JsonContentMatcher(content));
        }

        public static MockedRequest WithJsonContentFromFile(this MockedRequest source, string fileName)
        {
            var content = JsonFile.Read(fileName);
            return source.WithJsonContent(content);
        }

        public static void RespondJson(this MockedRequest source, string json)
        {
            source.Respond("application/json", json);
        }

        public static void RespondJsonFromFile(this MockedRequest source, string fileName)
        {
            var content = JsonFile.Read(fileName);
            source.RespondJson(content);
        }

        public static void Throw(this MockedRequest source)
        {
            source.Respond(message =>
            {
                var content = message.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
                throw new InvalidOperationException(
                    $"No matching mock handler. Message: {message}. Content: {content}");
            });
        }
    }
}
