using Ardalis.GuardClauses;

namespace POS.Domain.ValueObjects;

/// <summary>
/// Represents a monetary value with a specific currency
/// </summary>
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money() { Currency = string.Empty; }

    public Money(decimal amount, string currency)
    {
        Amount = Guard.Against.Negative(amount, nameof(amount));
        Currency = Guard.Against.NullOrWhiteSpace(currency, nameof(currency)).ToUpper();
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
        Guard.Against.InvalidInput(left, nameof(left), m => m.Amount >= right.Amount, "Result cannot be negative");
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        Guard.Against.Negative(multiplier, nameof(multiplier));
        return new Money(money.Amount * multiplier, money.Currency);
    }

    public static Money operator /(Money money, decimal divisor)
    {
        Guard.Against.NegativeOrZero(divisor, nameof(divisor));
        return new Money(money.Amount / divisor, money.Currency);
    }

    public static bool operator ==(Money left, Money right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;
        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;
        return left.Amount == right.Amount && left.Currency == right.Currency;
    }

    public static bool operator !=(Money left, Money right)
    {
        return !(left == right);
    }

    public static bool operator >(Money left, Money right)
    {
        Guard.Against.InvalidInput(left, nameof(left), m => m.Currency == right.Currency, "Cannot compare money with different currencies");
        return left.Amount > right.Amount;
    }

    public static bool operator <(Money left, Money right)
    {
        Guard.Against.InvalidInput(left, nameof(left), m => m.Currency == right.Currency, "Cannot compare money with different currencies");
        return left.Amount < right.Amount;
    }

    public static bool operator >=(Money left, Money right)
    {
        Guard.Against.InvalidInput(left, nameof(left), m => m.Currency == right.Currency, "Cannot compare money with different currencies");
        return left.Amount >= right.Amount;
    }

    public static bool operator <=(Money left, Money right)
    {
        Guard.Against.InvalidInput(left, nameof(left), m => m.Currency == right.Currency, "Cannot compare money with different currencies");
        return left.Amount <= right.Amount;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Money other)
        {
            return Amount == other.Amount && Currency == other.Currency;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    public override string ToString()
    {
        return $"{Amount} {Currency}";
    }

    public Money Round(int decimals = 2)
    {
        return new Money(Math.Round(Amount, decimals), Currency);
    }

    public Money Abs()
    {
        return new Money(Math.Abs(Amount), Currency);
    }

    public bool IsZero()
    {
        return Amount == 0;
    }

    public bool IsPositive()
    {
        return Amount > 0;
    }

    public bool IsNegative()
    {
        return Amount < 0;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
} 
