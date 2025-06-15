#nullable enable

#nullable enable
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleSyncedDomainEvent : DomainEvent
{
    public int SaleId { get; }
    public DateTime SyncedAt { get; }

    public SaleSyncedDomainEvent(int saleId, DateTime syncedAt)
    {
        SaleId = saleId;
        SyncedAt = syncedAt;
    }
} 
