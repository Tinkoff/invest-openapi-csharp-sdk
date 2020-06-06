using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace Tinkoff.Trading.OpenApi
{
    public static class EnumExtensions
    {
        private static readonly ConcurrentDictionary<object, string> CachedEnumMemberValues =
            new ConcurrentDictionary<object, string>();

        public static string GetEnumMemberValue<T>(this T @enum) where T : Enum
        {
            return CachedEnumMemberValues.GetOrAdd(@enum, e =>
            {
                var memInfo = typeof(T).GetMember(e.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false);
                return ((EnumMemberAttribute) attributes[0]).Value;
            });
        }
    }
}
