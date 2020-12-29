using System.Text.Json;

namespace Tinkoff.Trading.OpenApi
{
    internal class SerializationOptions
    {
        public static JsonSerializerOptions Instance { get; } = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }
}
