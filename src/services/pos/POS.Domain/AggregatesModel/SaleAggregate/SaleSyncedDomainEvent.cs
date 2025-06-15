using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
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
} 
