#nullable enable
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate.Events;

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
