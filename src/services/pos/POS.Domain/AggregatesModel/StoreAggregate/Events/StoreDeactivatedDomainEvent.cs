using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events
{
    public class StoreDeactivatedDomainEvent : DomainEvent
    {
        public Guid StoreId { get; }

        public StoreDeactivatedDomainEvent(Guid storeId)
        {
            StoreId = storeId;
        }
    }
} 
