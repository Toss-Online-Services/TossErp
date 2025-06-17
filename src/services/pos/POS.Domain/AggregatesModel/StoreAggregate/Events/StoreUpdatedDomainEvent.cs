using POS.Domain.Common.Events;
using System;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events
{
    public class StoreUpdatedDomainEvent : IDomainEvent
    {
        public Guid StoreId { get; }
        public string Name { get; }
        public string Address { get; }
        public DateTime UpdatedAt { get; }

        public StoreUpdatedDomainEvent(Guid storeId, string name, string address, DateTime updatedAt)
        {
            StoreId = storeId;
            Name = name;
            Address = address;
            UpdatedAt = updatedAt;
        }
    }
} 
