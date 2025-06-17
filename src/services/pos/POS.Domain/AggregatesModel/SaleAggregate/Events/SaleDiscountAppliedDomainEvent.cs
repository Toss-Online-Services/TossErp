using POS.Domain.Common.Events;
using POS.Domain.AggregatesModel.SaleAggregate;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleDiscountAppliedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public decimal DiscountAmount { get; }
    public DiscountType DiscountType { get; }
    public string? DiscountCode { get; }
    public DateTime AppliedAt { get; }

    public SaleDiscountAppliedDomainEvent(
        Guid saleId, 
        decimal discountAmount, 
        DiscountType discountType, 
        string? discountCode, 
        DateTime appliedAt)
    {
        SaleId = saleId;
        DiscountAmount = discountAmount;
        DiscountType = discountType;
        DiscountCode = discountCode;
        AppliedAt = appliedAt;
    }
} 
