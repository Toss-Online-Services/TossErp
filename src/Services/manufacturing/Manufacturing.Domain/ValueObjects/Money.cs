namespace Manufacturing.Domain.ValueObjects;

/// <summary>
/// Value object representing money with amount and currency
/// Consistent with accounting module
/// </summary>
public record Money(decimal Amount, string Currency = "ZAR")
{
    public static Money Zero(string currency = "ZAR") => new(0, currency);
    
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Cannot add money with different currencies: {left.Currency} and {right.Currency}");
        
        return new Money(left.Amount + right.Amount, left.Currency);
    }
    
    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Cannot subtract money with different currencies: {left.Currency} and {right.Currency}");
        
        return new Money(left.Amount - right.Amount, left.Currency);
    }
    
    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier, money.Currency);
    }
    
    public static Money operator /(Money money, decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException("Cannot divide money by zero");
        
        return new Money(money.Amount / divisor, money.Currency);
    }
    
    public bool IsZero() => Amount == 0;
    public bool IsPositive() => Amount > 0;
    public bool IsNegative() => Amount < 0;
    
    public override string ToString() => $"{Amount:F2} {Currency}";
}
