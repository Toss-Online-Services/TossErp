using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleCancelledDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public string Reason { get; }
    public DateTime CancelledAt { get; }

    public SaleCancelledDomainEvent(Guid saleId, string reason, DateTime cancelledAt)
    {
        SaleId = saleId;
        Reason = reason;
        CancelledAt = cancelledAt;
    }
} 
