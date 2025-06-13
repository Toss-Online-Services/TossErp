#nullable enable
using eShop.POS.Domain.Common;

namespace eShop.POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleSyncedDomainEvent : DomainEvent
{
    public Sale Sale { get; }
    public DateTime SyncedAt { get; }

    public SaleSyncedDomainEvent(Sale sale, DateTime syncedAt)
    {
        Sale = sale;
        SyncedAt = syncedAt;
    }
} 
