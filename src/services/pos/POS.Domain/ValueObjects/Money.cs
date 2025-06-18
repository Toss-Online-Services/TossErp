using Ardalis.GuardClauses;

namespace POS.Domain.ValueObjects;

/// <summary>
/// Represents a monetary value with a specific currency
/// </summary>
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money() { } // For EF Core

    public Money(decimal amount, string currency)
    {
        Amount = Guard.Against.Negative(amount, nameof(amount));
        Currency = Guard.Against.NullOrWhiteSpace(currency, nameof(currency));
    }

    public static Money FromDecimal(decimal amount, string currency)
    {
        return new Money(amount, currency);
    }

    public static Money operator +(Money left, Money right)
    {
        Guard.Against.InvalidInput(left, nameof(left), m => m.Currency == right.Currency, "Cannot add money with different currencies");
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        Guard.Against.InvalidInput(left, nameof(left), m => m.Currency == right.Currency, "Cannot subtract money with different currencies");
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString()
    {
        return $"{Amount} {Currency}";
    }
} 
