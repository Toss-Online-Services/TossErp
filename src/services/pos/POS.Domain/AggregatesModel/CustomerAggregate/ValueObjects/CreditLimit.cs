using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.CustomerAggregate.ValueObjects;

public record CreditLimit
{
    public decimal Amount { get; }
    public string Currency { get; }

    public CreditLimit(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new DomainException("Credit limit cannot be negative");
        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency cannot be empty");

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static CreditLimit Create(decimal amount, string currency = "USD") => 
        new(amount, currency);

    public bool HasAvailableCredit(decimal currentBalance) => 
        currentBalance < Amount;

    public decimal GetAvailableCredit(decimal currentBalance) => 
        Amount - currentBalance;
} 
