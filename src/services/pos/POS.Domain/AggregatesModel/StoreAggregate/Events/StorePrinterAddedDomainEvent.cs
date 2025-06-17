using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.StoreAggregate.Events
{
    public class StorePrinterAddedDomainEvent : DomainEvent
    {
        public Guid StoreId { get; }
        public string PrinterId { get; }
        public string PrinterType { get; }

        public StorePrinterAddedDomainEvent(Guid storeId, string printerId, string printerType)
        {
            StoreId = storeId;
            PrinterId = printerId;
            PrinterType = printerType;
        }
    }
} 
