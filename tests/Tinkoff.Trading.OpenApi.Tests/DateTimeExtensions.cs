using System;

namespace Tinkoff.Trading.OpenApi.Tests
{
    public static class DateTimeExtensions
    {
        public static DateTime AsUnspecified(this DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
        }
    }
}
