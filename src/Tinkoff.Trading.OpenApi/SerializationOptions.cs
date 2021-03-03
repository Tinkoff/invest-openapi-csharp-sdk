using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tinkoff.Trading.OpenApi
{
    internal class SerializationOptions
    {
        public static JsonSerializerOptions Instance { get; } = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            Converters =
            {
                new Int32Converter()
            }
        };

        private class Int32Converter : JsonConverter<int>
        {
            public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TryGetInt32(out var result))
                {
                    return result;
                }

                if (reader.TryGetDecimal(out var @decimal))
                {
                    return (int) @decimal;
                }

                return reader.GetInt32();
            }

            public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
            {
                writer.WriteNumberValue(value);
            }
        }
    }
}