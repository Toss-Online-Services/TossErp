using System;
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleDiscount : Entity
    {
        public int SaleId { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Percentage { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected SaleDiscount()
        {
            Description = string.Empty;
        }

        public SaleDiscount(int saleId, string description, decimal amount, decimal percentage)
        {
            if (saleId <= 0)
                throw new DomainException("Sale ID must be greater than zero");
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description cannot be empty");
            if (amount < 0)
                throw new DomainException("Amount cannot be negative");
            if (percentage < 0 || percentage > 100)
                throw new DomainException("Percentage must be between 0 and 100");

            SaleId = saleId;
            Description = description;
            Amount = amount;
            Percentage = percentage;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string description, decimal amount, decimal percentage)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description cannot be empty");
            if (amount < 0)
                throw new DomainException("Amount cannot be negative");
            if (percentage < 0 || percentage > 100)
                throw new DomainException("Percentage must be between 0 and 100");

            Description = description;
            Amount = amount;
            Percentage = percentage;
        }

        public decimal CalculateDiscountAmount(decimal totalAmount)
        {
            return Amount + (totalAmount * Percentage / 100);
        }
    }
}
