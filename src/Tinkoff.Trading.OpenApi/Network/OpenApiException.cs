using System;
using System.Net;

namespace Tinkoff.Trading.OpenApi.Network
{
    public class OpenApiException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; }

        public string TrackingId { get; }

        public OpenApiException(string message, string code, string trackingId, HttpStatusCode httpStatusCode)
            : base($"{code}: {message} ({trackingId}).")
        {
            TrackingId = trackingId;
            HttpStatusCode = httpStatusCode;
        }

        public OpenApiException(string message, HttpStatusCode httpStatusCode)
            : base($"{httpStatusCode}: {message}.")
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}
