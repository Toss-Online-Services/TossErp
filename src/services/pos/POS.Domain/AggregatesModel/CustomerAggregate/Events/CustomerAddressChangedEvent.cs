using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events
{
    public class CustomerAddressChangedEvent : DomainEvent
    {
        public Guid CustomerId { get; }
        public Guid AddressId { get; }
        public string AddressType { get; }
        public string OldAddress { get; }
        public string NewAddress { get; }
        public DateTime ChangedAt { get; }

        public CustomerAddressChangedEvent(
            Guid customerId,
            Guid addressId,
            string addressType,
            string oldAddress,
            string newAddress)
        {
            CustomerId = customerId;
            AddressId = addressId;
            AddressType = addressType;
            OldAddress = oldAddress;
            NewAddress = newAddress;
            ChangedAt = DateTime.UtcNow;
        }
    }
} 
