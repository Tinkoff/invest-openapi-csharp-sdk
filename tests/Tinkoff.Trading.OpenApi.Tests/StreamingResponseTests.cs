using System;
using System.Text.Json;
using FluentAssertions;

using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Tests.TestHelpers;

using Xunit;

namespace Tinkoff.Trading.OpenApi.Tests
{
    public class StreamingResponseTests
    {
        [Fact]
        public void DeserializeCandleTest()
        {
            var streamingResponse = JsonSerializer.Deserialize<StreamingResponse>(JsonFile.Read("streaming-candle-response"), new JsonSerializerOptions(JsonSerializerDefaults.Web));
            var response = streamingResponse as StreamingResponse<CandlePayload>;

            var expectedResponse = new CandleResponse(
                new CandlePayload(
                    64.0575m,
                    64.0578m,
                    64.0579m,
                    64.0573m,
                    156,
                    new DateTime(2019, 08, 07, 15, 35, 00, DateTimeKind.Utc),
                    CandleInterval.FiveMinutes,
                    "BBG0013HGFT4"),
                new DateTime(2019, 08, 07, 15, 35, 01, 029, DateTimeKind.Utc).AddTicks(7212));

            response.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
