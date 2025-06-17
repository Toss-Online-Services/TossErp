using POS.Domain.Common;

namespace POS.Domain.Common.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private Money()
    {
        Amount = 0;
        Currency = "USD";
    } // For EF Core

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative");
        
        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency cannot be empty");

        Amount = amount;
        Currency = currency.ToUpper();
    }

    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new DomainException("Cannot add money with different currencies");

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new DomainException("Cannot subtract money with different currencies");

        if (left.Amount < right.Amount)
            throw new DomainException("Result cannot be negative");

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        if (multiplier < 0)
            throw new DomainException("Multiplier cannot be negative");

        return new Money(money.Amount * multiplier, money.Currency);
    }

    public static Money operator /(Money money, decimal divisor)
    {
        if (divisor <= 0)
            throw new DomainException("Divisor must be greater than zero");

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
        if (left.Currency != right.Currency)
            throw new DomainException("Cannot compare money with different currencies");

        return left.Amount > right.Amount;
    }

    public static bool operator <(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new DomainException("Cannot compare money with different currencies");

        return left.Amount < right.Amount;
    }

    public static bool operator >=(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new DomainException("Cannot compare money with different currencies");

        return left.Amount >= right.Amount;
    }

    public static bool operator <=(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new DomainException("Cannot compare money with different currencies");

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
