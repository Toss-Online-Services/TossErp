using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleProcessingStartedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public DateTime StartedAt { get; }

    public SaleProcessingStartedDomainEvent(Guid saleId, DateTime startedAt)
    {
        SaleId = saleId;
        StartedAt = startedAt;
    }
} 
