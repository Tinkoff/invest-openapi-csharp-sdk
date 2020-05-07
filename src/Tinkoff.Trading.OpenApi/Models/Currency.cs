using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tinkoff.Trading.OpenApi.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Currency
    {
        [EnumMember(Value = "RUB")] Rub,
        [EnumMember(Value = "USD")] Usd,
        [EnumMember(Value = "EUR")] Eur,
        [EnumMember(Value = "GBP")] Gbp,
        [EnumMember(Value = "HKD")] Hkd,
        [EnumMember(Value = "CHF")] Chf,
        [EnumMember(Value = "JPY")] Jpy,
        [EnumMember(Value = "CNY")] Cny,
        [EnumMember(Value = "TRY")] Try,
    }
}