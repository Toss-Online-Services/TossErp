using System;
using TossErp.Domain.SeedWork;
using TossErp.POS.Domain.Enums;
using TossErp.Domain.Exceptions;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate
{
    public class SalePayment : Entity
    {
        public PaymentMethod PaymentMethod { get; private set; }
        public decimal Amount { get; private set; }
        public string? ReferenceNumber { get; private set; }
        public DateTime PaymentDate { get; private set; }

        protected SalePayment() { }

        public SalePayment(PaymentMethod paymentMethod, decimal amount, string? referenceNumber = null)
        {
            PaymentMethod = paymentMethod;
            Amount = amount > 0 ? amount : throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
            ReferenceNumber = referenceNumber;
            PaymentDate = DateTime.UtcNow;
        }
    }
} 
