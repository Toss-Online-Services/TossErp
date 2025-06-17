using POS.Domain.Common.Events;
using System;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events
{
    public class StoreDeactivatedDomainEvent : IDomainEvent
    {
        public Guid StoreId { get; }
        public DateTime DeactivatedAt { get; }

        public StoreDeactivatedDomainEvent(Guid storeId, DateTime deactivatedAt)
        {
            StoreId = storeId;
            DeactivatedAt = deactivatedAt;
        }
    }
} 
