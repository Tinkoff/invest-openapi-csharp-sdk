using System;

namespace Tinkoff.Trading.OpenApi.Models
{
    static class Extensions
    {
        public static string ToParamString(this Interval interval)
        {
            switch (interval)
            {
                case Interval.Day:
                    return "1day";
                case Interval.Week:
                    return "7days";
                case Interval.TwoWeeks:
                    return "14days";
                case Interval.Month:
                    return "30days";
                default:
                    throw new ArgumentOutOfRangeException(nameof(interval), interval, null);
            }
        }
    }
}