using eShop.POS.Domain.SeedWork;

namespace eShop.POS.Domain.AggregatesModel.SaleAggregate;

public class Payment : ValueObject
{
    public string Method { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string? Reference { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Payment() 
    {
        CreatedAt = DateTime.UtcNow;
    }

    public Payment(string method, decimal amount, string? reference = null)
    {
        Method = method;
        Amount = amount;
        Reference = reference;
        CreatedAt = DateTime.UtcNow;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Method;
        yield return Amount;
        if (Reference != null)
            yield return Reference;
        yield return CreatedAt;
    }
} 
