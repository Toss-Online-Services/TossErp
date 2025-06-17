using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events
{
    public class SaleItemQuantityUpdatedDomainEvent : DomainEvent
    {
        public Guid SaleId { get; }
        public Guid ItemId { get; }
        public int OldQuantity { get; }
        public int NewQuantity { get; }
        public SaleItemQuantityUpdatedDomainEvent(Guid saleId, Guid itemId, int oldQuantity, int newQuantity)
        {
            SaleId = saleId;
            ItemId = itemId;
            OldQuantity = oldQuantity;
            NewQuantity = newQuantity;
        }
    }
} 
