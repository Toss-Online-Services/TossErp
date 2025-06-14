using System;
using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.SaleAggregate;

public class SaleDiscount : Entity
{
    public int SaleId { get; private set; }
    public string Name { get; private set; }
    public DiscountType Type { get; private set; }
    public decimal Value { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected SaleDiscount() { }

    public SaleDiscount(string name, DiscountType type, decimal value)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Discount name cannot be empty");

        if (value <= 0)
            throw new DomainException("Discount value must be greater than zero");

        Name = name;
        Type = type;
        Value = value;
        CreatedAt = DateTime.UtcNow;
    }

    public void CalculateAmount(decimal totalAmount)
    {
        Amount = Type == DiscountType.Percentage
            ? totalAmount * (Value / 100)
            : Value;
    }
}
