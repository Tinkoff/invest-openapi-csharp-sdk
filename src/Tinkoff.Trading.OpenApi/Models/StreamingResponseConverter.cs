using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tinkoff.Trading.OpenApi.Models
{
    internal class StreamingResponseConverter : JsonConverter<StreamingResponse>
    {
        public override StreamingResponse Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            string type = string.Empty;
            DateTime dateTime = default;
            Utf8JsonReader payloadReader = reader;

            var eventPropertyName = "event".AsSpan();
            var timePropertyName = "time".AsSpan();
            var payloadPropertyName = "payload".AsSpan();
            var startDepth = reader.CurrentDepth;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals(eventPropertyName))
                {
                    reader.Read();
                    type = reader.GetString();

                    continue;
                }

                if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals(timePropertyName))
                {
                    reader.Read();
                    dateTime = reader.GetDateTime();

                    continue;
                }

                if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals(payloadPropertyName))
                {
                    payloadReader = reader;
                    reader.Skip();

                    continue;
                }

                if (!string.IsNullOrEmpty(type) && dateTime != default && payloadReader.TokenStartIndex != 0)
                {
                    // When reading an object, JsonConverter<T>.Read() must leave the Utf8JsonReader
                    // positioned on the EndObject token of the object where it was originally positioned.
                    while ((reader.TokenType != JsonTokenType.EndObject || reader.CurrentDepth != startDepth) && reader.Read())
                    {
                    }

                    switch (type)
                    {
                        case "candle":
                            return new CandleResponse(JsonSerializer.Deserialize<CandlePayload>(ref payloadReader, options), dateTime);
                        case "orderbook":
                            return new OrderbookResponse(JsonSerializer.Deserialize<OrderbookPayload>(ref payloadReader, options), dateTime);
                        case "instrument_info":
                            return new InstrumentInfoResponse(JsonSerializer.Deserialize<InstrumentInfoPayload>(ref payloadReader, options), dateTime);
                        case "error":
                            return new StreamingErrorResponse(JsonSerializer.Deserialize<StreamingErrorPayload>(ref payloadReader, options), dateTime);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, StreamingResponse value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}
