using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events
{
    public class SaleMarkedAsOfflineDomainEvent : DomainEvent
    {
        public Guid SaleId { get; }
        public SaleMarkedAsOfflineDomainEvent(Guid saleId)
        {
            SaleId = saleId;
        }
    }
} 
