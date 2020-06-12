using System;
using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class CandlePayload
    {
        public decimal Open { get; }
        public decimal Close { get; }
        public decimal High { get; }
        public decimal Low { get; }
        public decimal Volume { get; }
        public DateTime Time { get; }
        public CandleInterval Interval { get; }
        public string Figi { get; }

        [JsonConstructor]
        public CandlePayload(
            [JsonProperty("o")] decimal open,
            [JsonProperty("c")] decimal close,
            [JsonProperty("h")] decimal high,
            [JsonProperty("l")] decimal low,
            [JsonProperty("v")] decimal volume,
            DateTime time,
            CandleInterval interval,
            string figi)
        {
            Open = open;
            Close = close;
            High = high;
            Low = low;
            Volume = volume;
            Time = time;
            Interval = interval;
            Figi = figi;
        }

        public override string ToString()
        {
            return $"{nameof(Figi)}: {Figi}, {nameof(Interval)}: {Interval}, {nameof(Time)}: {Time}, {nameof(Open)}: {Open}, {nameof(Close)}: {Close}, {nameof(High)}: {High}, {nameof(Low)}: {Low}, {nameof(Volume)}: {Volume}";
        }
    }
}
