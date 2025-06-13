using eShop.POS.Domain.SeedWork;

namespace eShop.POS.Domain.AggregatesModel.SaleAggregate;

public class SaleDiscount : ValueObject
{
    public string Name { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public DiscountType Type { get; private set; }

    public SaleDiscount() { }

    public SaleDiscount(string name, decimal amount, DiscountType type)
    {
        Name = name;
        Amount = amount;
        Type = type;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Amount;
        yield return Type;
    }
}

public enum DiscountType
{
    Percentage,
    FixedAmount
} 
