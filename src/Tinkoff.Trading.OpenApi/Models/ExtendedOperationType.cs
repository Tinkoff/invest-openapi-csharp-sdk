using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tinkoff.Trading.OpenApi.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExtendedOperationType
    {
        BuyCard,
        Buy,
        Sell,
        BrokerCommission,
        ExchangeCommission,
        ServiceCommission,
        MarginCommission,
        OtherCommission,
        PayIn,
        PayOut,
        Tax,
        TaxLucre,
        TaxDividend,
        TaxCoupon,
        TaxBack,
        Repayment,
        PartRepayment,
        Coupon,
        Dividend,
        SecurityIn,
        SecurityOut
    }
}
