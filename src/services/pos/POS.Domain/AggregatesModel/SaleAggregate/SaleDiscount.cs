using POS.Domain.AggregatesModel.StaffAggregate;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate
{
    public class SaleDiscount : Entity
    {
        public Guid SaleId { get; private set; }
        public Sale Sale { get; private set; } = null!; // Fix: Initialize with null-forgiving operator
        public string Name { get; private set; }
        public decimal Amount { get; private set; }
        public DiscountType Type { get; private set; }
        public string? Reason { get; private set; }
        public Guid? StaffId { get; private set; }
        public Staff? Staff { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        protected SaleDiscount()
        {
            Name = string.Empty;
            CreatedAt = DateTime.UtcNow;
        }

        public SaleDiscount(Guid saleId, string name, decimal amount, DiscountType type, string? reason = null, Guid? staffId = null)
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
            Reason = reason;
            StaffId = staffId;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateAmount(decimal newAmount)
        {
            if (newAmount < 0)
                throw new DomainException("Discount amount cannot be negative");

            Amount = newAmount;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateReason(string? reason)
        {
            Reason = reason;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStaff(Guid? staffId)
        {
            StaffId = staffId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
