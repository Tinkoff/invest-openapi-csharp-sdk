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
            var streamingResponse =
                JsonSerializer.Deserialize<StreamingResponse>(JsonFile.Read("streaming-candle-response"),
                    SerializationOptions.Instance);
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

        [Fact]
        public void DeserializeCandleWithNewValueTest()
        {
            Action act = () =>
                JsonSerializer.Deserialize<StreamingResponse>(JsonFile.Read("streaming-candle-response-with-new-field"),
                    SerializationOptions.Instance);

            act.Should().NotThrow("The serialization shouldn't fail if there are new properties in the response");
        }

        [Fact]
        public void DeserializeInstrumentInfoPayloadScientificStyleLotTest()
        {
            var payload =
                "{\"figi\":\"BBG000RG4ZQ4\",\"trade_status\":\"normal_trading\",\"min_price_increment\":1e-05,\"lot\":1e+06}";

            Action act = () => JsonSerializer.Deserialize<InstrumentInfoPayload>(payload, SerializationOptions.Instance);
            act.Should().NotThrow();
        }
    }
}