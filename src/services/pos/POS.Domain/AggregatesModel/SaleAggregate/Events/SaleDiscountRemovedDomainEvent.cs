using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events
{
    public class SaleDiscountRemovedDomainEvent : DomainEvent
    {
        public Guid SaleId { get; }
        public Guid DiscountId { get; }
        public SaleDiscountRemovedDomainEvent(Guid saleId, Guid discountId)
        {
            SaleId = saleId;
            DiscountId = discountId;
        }
    }
} 
