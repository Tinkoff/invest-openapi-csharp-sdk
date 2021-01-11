using System.Text.Json.Serialization;

namespace Tinkoff.Trading.OpenApi.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ExtendedOperationType
    {
        Buy,
        BuyCard,
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
