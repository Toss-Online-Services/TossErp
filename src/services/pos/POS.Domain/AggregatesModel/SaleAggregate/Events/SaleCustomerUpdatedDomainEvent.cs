using POS.Domain.SeedWork;
using System;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events
{
    public class SaleCustomerUpdatedDomainEvent : DomainEvent
    {
        public Guid SaleId { get; }
        public Guid CustomerId { get; }
        public string CustomerName { get; }
        public SaleCustomerUpdatedDomainEvent(Guid saleId, Guid customerId, string customerName)
        {
            SaleId = saleId;
            CustomerId = customerId;
            CustomerName = customerName;
        }
    }
} 
