using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tinkoff.Trading.OpenApi.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BrokerAccountType
    {
        Tinkoff = 1,
        TinkoffIis = 2
    }
}