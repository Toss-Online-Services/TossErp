#nullable enable

using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleRefundedDomainEvent : DomainEvent
{
    public Guid SaleId { get; }
    public decimal RefundAmount { get; }
    public DateTime RefundedAt { get; }

    public SaleRefundedDomainEvent(Sale sale, decimal refundAmount)
    {
        SaleId = sale.Id;
        RefundAmount = refundAmount;
        RefundedAt = DateTime.UtcNow;
    }
} 
