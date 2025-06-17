using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events
{
    public class StorePrinterRemovedDomainEvent : DomainEvent
    {
        public Guid StoreId { get; }
        public StorePrinterRemovedDomainEvent(Guid storeId)
        {
            StoreId = storeId;
        }
    }
} 
