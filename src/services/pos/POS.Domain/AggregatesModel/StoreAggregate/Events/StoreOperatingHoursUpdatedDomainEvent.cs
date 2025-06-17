using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events
{
    public class StoreOperatingHoursUpdatedDomainEvent : DomainEvent
    {
        public Guid StoreId { get; }
        public StoreOperatingHoursUpdatedDomainEvent(Guid storeId)
        {
            StoreId = storeId;
        }
    }
} 
