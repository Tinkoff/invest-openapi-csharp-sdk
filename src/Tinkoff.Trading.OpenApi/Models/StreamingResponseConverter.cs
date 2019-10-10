using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Tinkoff.Trading.OpenApi.Models
{
    class StreamingResponseConverter : JsonConverter
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new BaseSpecifiedConcreteClassConverter()
        };

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            switch (jsonObject["event"].Value<string>())
            {
                case "candle":
                    return JsonConvert.DeserializeObject<CandleResponse>(jsonObject.ToString(), Settings);
                case "orderbook":
                    return JsonConvert.DeserializeObject<OrderbookResponse>(jsonObject.ToString(), Settings);
                case "instrument_info":
                    return JsonConvert.DeserializeObject<InstrumentInfoResponse>(jsonObject.ToString(), Settings);
                case "error":
                    return JsonConvert.DeserializeObject<StreamingErrorResponse>(jsonObject.ToString(), Settings);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StreamingResponse);
        }

        private class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
        {
            protected override JsonConverter ResolveContractConverter(Type objectType)
            {
                if (typeof(StreamingResponse).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                    return null;
                return base.ResolveContractConverter(objectType);
            }
        }
    }
}