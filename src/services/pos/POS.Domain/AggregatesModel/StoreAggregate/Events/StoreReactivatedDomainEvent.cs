using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events
{
    public class StoreReactivatedDomainEvent : DomainEvent
    {
        public Guid StoreId { get; }
        public StoreReactivatedDomainEvent(Guid storeId)
        {
            StoreId = storeId;
        }
    }
} 
