using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleOfflineStatusChangedEvent : DomainEvent
    {
        public Sale Sale { get; }
        public bool IsOffline { get; }

        public SaleOfflineStatusChangedEvent(Sale sale, bool isOffline)
        {
            Sale = sale;
            IsOffline = isOffline;
        }
    }
} 
