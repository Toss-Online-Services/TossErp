using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.StoreAggregate
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
