using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events
{
    public class StoreDeviceRemovedDomainEvent : DomainEvent
    {
        public int StoreId { get; }
        public string DeviceId { get; }

        public StoreDeviceRemovedDomainEvent(int storeId, string deviceId)
        {
            StoreId = storeId;
            DeviceId = deviceId;
        }
    }
} 
