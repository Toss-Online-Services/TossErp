#nullable enable
using TossErp.POS.Domain.Events;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleDiscountAddedDomainEvent : DomainEvent
{
    public int SaleId { get; }
    public int DiscountId { get; }
    public decimal Amount { get; }

    public SaleDiscountAddedDomainEvent(int saleId, int discountId, decimal amount)
    {
        SaleId = saleId;
        DiscountId = discountId;
        Amount = amount;
    }
} 
