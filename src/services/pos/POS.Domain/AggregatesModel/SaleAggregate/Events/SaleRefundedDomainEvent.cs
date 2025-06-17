#nullable enable

using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleRefundedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public string Reason { get; }
    public DateTime RefundedAt { get; }
    public decimal RefundAmount { get; }
    public Guid? ProcessedBy { get; }

    public SaleRefundedDomainEvent(Guid saleId, string reason, DateTime refundedAt, decimal refundAmount, Guid? processedBy = null)
    {
        SaleId = saleId;
        Reason = reason;
        RefundedAt = refundedAt;
        RefundAmount = refundAmount;
        ProcessedBy = processedBy;
    }
} 
