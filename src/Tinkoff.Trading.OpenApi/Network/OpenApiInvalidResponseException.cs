using System;
using System.Runtime.Serialization;

namespace Tinkoff.Trading.OpenApi.Network
{
    [Serializable]
    public class OpenApiInvalidResponseException : Exception
    {
        public OpenApiInvalidResponseException()
        {
        }

        public OpenApiInvalidResponseException(string message)
            : base(message)
        {
        }

        public OpenApiInvalidResponseException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public OpenApiInvalidResponseException(string message, string rawResponse, Exception inner)
            : base($"{message}\nResponse:\n{rawResponse}", inner)
        {
        }

        protected OpenApiInvalidResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
