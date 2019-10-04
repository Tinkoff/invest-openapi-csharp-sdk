using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tinkoff.Trading.OpenApi.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OperationStatus
    {
        Done,
        Decline,
        Progress
    }
}