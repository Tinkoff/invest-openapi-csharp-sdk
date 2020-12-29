using System;
using System.Text.Json.Serialization;

namespace Tinkoff.Trading.OpenApi.Models
{
    public class CandlePayload
    {
        [JsonPropertyName("o")]
        public decimal Open { get; }
        [JsonPropertyName("c")]
        public decimal Close { get; }
        [JsonPropertyName("h")]
        public decimal High { get; }
        [JsonPropertyName("l")]
        public decimal Low { get; }
        [JsonPropertyName("v")]
        public decimal Volume { get; }
        public DateTime Time { get; }
        public CandleInterval Interval { get; }
        public string Figi { get; }

        [JsonConstructor]
        public CandlePayload(
            decimal open,
            decimal close,
            decimal high,
            decimal low,
            decimal volume,
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
