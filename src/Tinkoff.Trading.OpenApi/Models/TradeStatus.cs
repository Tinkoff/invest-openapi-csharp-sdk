using System.Text.Json.Serialization;

namespace Tinkoff.Trading.OpenApi.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TradeStatus
    {
        NormalTrading,
        NotAvailableForTrading
    }
}