using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.StoreAggregate
{
    public class StoreStatusChangedEvent : DomainEvent
    {
        public Store Store { get; }
        public bool IsActive { get; }

        public StoreStatusChangedEvent(Store store, bool isActive)
        {
            Store = store;
            IsActive = isActive;
        }
    }
} 
