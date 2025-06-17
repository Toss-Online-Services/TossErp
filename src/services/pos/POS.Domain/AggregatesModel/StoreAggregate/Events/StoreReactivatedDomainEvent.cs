using POS.Domain.Common.Events;
using System;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events
{
    public class StoreReactivatedDomainEvent : IDomainEvent
    {
        public Guid StoreId { get; }
        public DateTime ReactivatedAt { get; }

        public StoreReactivatedDomainEvent(Guid storeId, DateTime reactivatedAt)
        {
            StoreId = storeId;
            ReactivatedAt = reactivatedAt;
        }
    }
} 
