using POS.Domain.Common.Events;
using POS.Domain.AggregatesModel.SaleAggregate;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleStatusChangedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public SaleStatus Status { get; }
    public DateTime ChangedAt { get; }

    public SaleStatusChangedDomainEvent(Guid saleId, SaleStatus status, DateTime changedAt)
    {
        SaleId = saleId;
        Status = status;
        ChangedAt = changedAt;
    }
} 
