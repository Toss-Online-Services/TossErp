using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events
{
    public class StoreDeviceAddedDomainEvent : DomainEvent
    {
        public Guid StoreId { get; }
        public string DeviceId { get; }
        public string DeviceType { get; }

        public StoreDeviceAddedDomainEvent(Guid storeId, string deviceId, string deviceType)
        {
            StoreId = storeId;
            DeviceId = deviceId;
            DeviceType = deviceType;
        }
    }
} 
