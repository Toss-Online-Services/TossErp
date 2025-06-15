using System;
using TossErp.POS.Domain.SeedWork;
namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleDiscount : Entity
    {
        public Guid SaleId { get; private set; }
        public string Name { get; private set; }
        public decimal Amount { get; private set; }
        public DiscountType Type { get; private set; }

        protected SaleDiscount()
        {
            Name = string.Empty;
        }

        public SaleDiscount(Guid saleId, string name, decimal amount, DiscountType type)
        {
            if (saleId == Guid.Empty)
                throw new DomainException("Sale ID cannot be empty");
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Discount name cannot be empty");
            if (amount < 0)
                throw new DomainException("Discount amount cannot be negative");

            SaleId = saleId;
            Name = name;
            Amount = amount;
            Type = type;
        }

        public void UpdateAmount(decimal newAmount)
        {
            if (newAmount < 0)
                throw new DomainException("Discount amount cannot be negative");

            Amount = newAmount;
        }
    }
}
