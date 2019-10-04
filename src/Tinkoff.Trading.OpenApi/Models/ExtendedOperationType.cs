using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tinkoff.Trading.OpenApi.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExtendedOperationType
    {
        Buy,
        Sell,
        BrokerCommission,
        ExchangeCommission,
        ServiceCommission,
        MarginCommission
    }
}