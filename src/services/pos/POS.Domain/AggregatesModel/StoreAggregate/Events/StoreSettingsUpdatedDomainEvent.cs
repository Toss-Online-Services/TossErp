using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events
{
    public class StoreSettingsUpdatedDomainEvent : DomainEvent
    {
        public Guid StoreId { get; }

        public StoreSettingsUpdatedDomainEvent(Guid storeId)
        {
            StoreId = storeId;
        }
    }
} 
