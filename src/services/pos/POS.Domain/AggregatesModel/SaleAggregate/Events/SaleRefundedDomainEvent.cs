#nullable enable

#nullable enable
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleRefundedDomainEvent : DomainEvent
{
    public int SaleId { get; }
    public decimal RefundAmount { get; }

    public SaleRefundedDomainEvent(int saleId, decimal refundAmount)
    {
        SaleId = saleId;
        RefundAmount = refundAmount;
    }
} 
