using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.CustomerAggregate.ValueObjects;

public record CustomerBalance
{
    public decimal Amount { get; }
    public string Currency { get; }
    public DateTime LastUpdated { get; }

    public CustomerBalance(decimal amount, string currency = "USD")
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency cannot be empty");

        Amount = amount;
        Currency = currency.ToUpperInvariant();
        LastUpdated = DateTime.UtcNow;
    }

    public static CustomerBalance Create(decimal amount, string currency = "USD") => 
        new(amount, currency);

    public CustomerBalance Add(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Amount to add must be greater than zero");

        return new CustomerBalance(Amount + amount, Currency);
    }

    public CustomerBalance Subtract(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Amount to subtract must be greater than zero");

        return new CustomerBalance(Amount - amount, Currency);
    }

    public bool IsPositive => Amount > 0;
    public bool IsNegative => Amount < 0;
    public bool IsZero => Amount == 0;
} 
