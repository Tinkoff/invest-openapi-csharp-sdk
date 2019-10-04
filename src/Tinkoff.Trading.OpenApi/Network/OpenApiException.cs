using System;
using Newtonsoft.Json;

namespace Tinkoff.Trading.OpenApi.Network
{
    public class OpenApiException : Exception
    {
        public string TrackingId { get; }

        [JsonConstructor]
        public OpenApiException(string message, string code, string trackingId)
            : base($"{code}: {message} ({trackingId}).")
        {
            TrackingId = trackingId;
        }

        [JsonConstructor]
        public OpenApiException(string message, string code)
            : base($"{code}: {message}.")
        {
        }
    }
}