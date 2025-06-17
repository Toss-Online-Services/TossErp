using POS.Domain.Common.Events;
using System;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events
{
    public class SaleCustomerUpdatedDomainEvent : IDomainEvent
    {
        public Guid SaleId { get; }
        public Guid CustomerId { get; }
        public string CustomerName { get; }
        public DateTime UpdatedAt { get; }

        public SaleCustomerUpdatedDomainEvent(Guid saleId, Guid customerId, string customerName, DateTime updatedAt)
        {
            SaleId = saleId;
            CustomerId = customerId;
            CustomerName = customerName;
            UpdatedAt = updatedAt;
        }
    }
} 
