using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.StoreAggregate
{
    public class StoreCreatedEvent : DomainEvent
    {
        public Store Store { get; }

        public StoreCreatedEvent(Store store)
        {
            Store = store;
        }
    }
} 
