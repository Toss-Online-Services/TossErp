using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events
{
    public class StoreUpdatedDomainEvent : DomainEvent
    {
        public Guid StoreId { get; }
        public string Name { get; }
        public string Address { get; }

        public StoreUpdatedDomainEvent(Guid storeId, string name, string address)
        {
            StoreId = storeId;
            Name = name;
            Address = address;
        }
    }
} 
