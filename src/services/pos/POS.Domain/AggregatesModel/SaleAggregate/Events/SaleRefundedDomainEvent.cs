#nullable enable

using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleRefundedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public decimal Amount { get; }
    public string Reason { get; }
    public DateTime RefundedAt { get; }

    public SaleRefundedDomainEvent(Guid saleId, decimal amount, string reason, DateTime refundedAt)
    {
        SaleId = saleId;
        Amount = amount;
        Reason = reason;
        RefundedAt = refundedAt;
    }
} 
