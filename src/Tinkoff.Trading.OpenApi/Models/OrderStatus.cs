using System.Text.Json.Serialization;

namespace Tinkoff.Trading.OpenApi.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        New,
        PartiallyFill,
        Fill,
        Cancelled,
        Replaced,
        PendingCancel,
        Rejected,
        PendingReplace,
        PendingNew
    }
}