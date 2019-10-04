using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tinkoff.Trading.OpenApi.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
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