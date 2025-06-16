#nullable enable

using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleRefundedDomainEvent : DomainEvent
{
    public Guid SaleId { get; }
    public decimal RefundAmount { get; }
    public string Reason { get; }
    public DateTime RefundedAt { get; }

    public SaleRefundedDomainEvent(Sale sale, decimal refundAmount, string reason)
    {
        SaleId = sale.Id;
        RefundAmount = refundAmount;
        Reason = reason;
        RefundedAt = DateTime.UtcNow;
    }
} 
