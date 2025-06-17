using POS.Domain.Common.Events;
using POS.Domain.AggregatesModel.SaleAggregate;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleStatusChangedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public SaleStatus NewStatus { get; }
    public DateTime ChangedAt { get; }

    public SaleStatusChangedDomainEvent(Guid saleId, SaleStatus newStatus, DateTime changedAt)
    {
        SaleId = saleId;
        NewStatus = newStatus;
        ChangedAt = changedAt;
    }
} 
