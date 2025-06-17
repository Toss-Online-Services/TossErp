using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events
{
    public class SaleNotesUpdatedDomainEvent : DomainEvent
    {
        public Guid SaleId { get; }
        public string Notes { get; }
        public SaleNotesUpdatedDomainEvent(Guid saleId, string notes)
        {
            SaleId = saleId;
            Notes = notes;
        }
    }
} 
