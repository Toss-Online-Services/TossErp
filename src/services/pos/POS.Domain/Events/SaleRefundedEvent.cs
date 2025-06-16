using POS.Domain.SeedWork;

namespace POS.Domain.Events;

public class SaleRefundedEvent : DomainEvent
{
    public Guid SaleId { get; }
    public decimal RefundAmount { get; }
    public string Currency { get; }
    public string Reason { get; }
    public DateTime RefundedAt { get; }

    public SaleRefundedEvent(Guid saleId, decimal refundAmount, string currency, string reason)
    {
        SaleId = saleId;
        RefundAmount = refundAmount;
        Currency = currency;
        Reason = reason;
        RefundedAt = DateTime.UtcNow;
    }
} 
