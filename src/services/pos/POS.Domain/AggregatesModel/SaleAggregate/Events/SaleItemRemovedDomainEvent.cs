using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleItemRemovedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public Guid SaleItemId { get; }
    public string Reason { get; }
    public DateTime RemovedAt { get; }

    public SaleItemRemovedDomainEvent(Guid saleId, Guid saleItemId, string reason, DateTime removedAt)
    {
        SaleId = saleId;
        SaleItemId = saleItemId;
        Reason = reason;
        RemovedAt = removedAt;
    }
} 
