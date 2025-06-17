using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events
{
    public class StoreCreatedDomainEvent : DomainEvent
    {
        public int StoreId { get; }
        public string Name { get; }
        public string Address { get; }

        public StoreCreatedDomainEvent(int storeId, string name, string address)
        {
            StoreId = storeId;
            Name = name;
            Address = address;
        }
    }
} 
