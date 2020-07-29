using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tinkoff.Trading.OpenApi.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CandleInterval
    {
        [EnumMember(Value = "1min")]
        Minute,
        [EnumMember(Value = "2min")]
        TwoMinutes,
        [EnumMember(Value = "3min")]
        ThreeMinutes,
        [EnumMember(Value = "5min")]
        FiveMinutes,
        [EnumMember(Value = "10min")]
        TenMinutes,
        [EnumMember(Value = "15min")]
        QuarterHour,
        [EnumMember(Value = "30min")]
        HalfHour,
        [EnumMember(Value = "hour")]
        Hour,
        [EnumMember(Value = "day")]
        Day,
        [EnumMember(Value = "week")]
        Week,
        [EnumMember(Value = "month")]
        Month
    }
}