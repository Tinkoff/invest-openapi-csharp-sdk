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
            JsonDocument payload = null;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "event")
                {
                    reader.Read();
                    type = reader.GetString();

                    continue;
                }

                if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "time")
                {
                    reader.Read();
                    dateTime = reader.GetDateTime();

                    continue;
                }

                if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "payload")
                {
                    reader.Read();
                    payload = JsonDocument.ParseValue(ref reader);
                    continue;
                }

                if (!string.IsNullOrEmpty(type) && payload != null && dateTime != default)
                {
                    var rawPayload = payload.RootElement.GetRawText();

                    switch (type)
                    {
                        case "candle":
                            return new CandleResponse(JsonSerializer.Deserialize<CandlePayload>(rawPayload, options), dateTime);
                        case "orderbook":
                            return new OrderbookResponse(JsonSerializer.Deserialize<OrderbookPayload>(rawPayload, options), dateTime);
                        case "instrument_info":
                            return new InstrumentInfoResponse(JsonSerializer.Deserialize<InstrumentInfoPayload>(rawPayload, options), dateTime);
                        case "error":
                            return new StreamingErrorResponse(JsonSerializer.Deserialize<StreamingErrorPayload>(rawPayload, options), dateTime);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                continue;
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, StreamingResponse value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}
