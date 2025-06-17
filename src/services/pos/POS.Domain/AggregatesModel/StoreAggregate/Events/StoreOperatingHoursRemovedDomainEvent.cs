using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events
{
    public class StoreOperatingHoursRemovedDomainEvent : DomainEvent
    {
        public Guid StoreId { get; }
        public StoreOperatingHoursRemovedDomainEvent(Guid storeId)
        {
            StoreId = storeId;
        }
    }
} 
