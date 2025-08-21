using TossErp.Accounts.Domain.Enums;
using TossErp.Accounts.Domain.SeedWork;

namespace TossErp.Accounts.Domain.ValueObjects;

/// <summary>
/// Money value object with currency support for multi-currency accounting
/// </summary>
public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public CurrencyCode Currency { get; private set; }

    public Money(decimal amount, CurrencyCode currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative for Money value object", nameof(amount));
        
        Amount = Math.Round(amount, 2);
        Currency = currency;
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add different currencies: {Currency} and {other.Currency}");
        
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot subtract different currencies: {Currency} and {other.Currency}");
        
        return new Money(Math.Max(0, Amount - other.Amount), Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        if (multiplier < 0)
            throw new ArgumentException("Multiplier cannot be negative", nameof(multiplier));
            
        return new Money(Amount * multiplier, Currency);
    }

    public Money Divide(decimal divisor)
    {
        if (divisor <= 0)
            throw new ArgumentException("Divisor must be positive", nameof(divisor));
            
        return new Money(Amount / divisor, Currency);
    }

    public bool IsZero => Amount == 0;
    public bool IsPositive => Amount > 0;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:C} {Currency}";

    public static Money Zero(CurrencyCode currency) => new(0, currency);
}

/// <summary>
/// Signed money value object that can represent debits and credits
/// </summary>
public class SignedMoney : ValueObject
{
    public decimal Amount { get; private set; }
    public CurrencyCode Currency { get; private set; }
    public DebitCredit Type { get; private set; }

    public SignedMoney(decimal amount, CurrencyCode currency, DebitCredit type)
    {
        Amount = Math.Abs(Math.Round(amount, 2));
        Currency = currency;
        Type = type;
    }

    public decimal SignedAmount => Type == DebitCredit.Debit ? Amount : -Amount;

    public SignedMoney Negate()
    {
        var newType = Type == DebitCredit.Debit ? DebitCredit.Credit : DebitCredit.Debit;
        return new SignedMoney(Amount, Currency, newType);
    }

    public Money ToMoney() => new(Amount, Currency);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
        yield return Type;
    }

    public override string ToString() => $"{(Type == DebitCredit.Debit ? "DR" : "CR")} {Amount:C} {Currency}";
}

/// <summary>
/// Account number value object with validation
/// </summary>
public class AccountNumber : ValueObject
{
    public string Value { get; private set; }

    public AccountNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Account number cannot be empty", nameof(value));
        
        var trimmed = value.Trim();
        if (trimmed.Length < 3 || trimmed.Length > 20)
            throw new ArgumentException("Account number must be between 3 and 20 characters", nameof(value));
        
        if (!IsValidAccountNumber(trimmed))
            throw new ArgumentException("Account number contains invalid characters", nameof(value));

        Value = trimmed.ToUpperInvariant();
    }

    private static bool IsValidAccountNumber(string value)
    {
        return value.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '.');
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

/// <summary>
/// Tax rate value object with validation
/// </summary>
public class TaxRate : ValueObject
{
    public decimal Rate { get; private set; }
    public TaxType Type { get; private set; }

    public TaxRate(decimal rate, TaxType type)
    {
        if (rate < 0 || rate > 100)
            throw new ArgumentException("Tax rate must be between 0 and 100 percent", nameof(rate));

        Rate = Math.Round(rate, 4);
        Type = type;
    }

    public Money CalculateTax(Money baseAmount)
    {
        if (baseAmount.Currency != CurrencyCode.USD) // Default validation
            throw new InvalidOperationException("Currency mismatch for tax calculation");
            
        var taxAmount = baseAmount.Amount * (Rate / 100);
        return new Money(taxAmount, baseAmount.Currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Rate;
        yield return Type;
    }

    public override string ToString() => $"{Rate:F2}% {Type}";

    public static TaxRate Zero(TaxType type) => new(0, type);
}

/// <summary>
/// Exchange rate value object for currency conversion
/// </summary>
public class ExchangeRate : ValueObject
{
    public CurrencyCode FromCurrency { get; private set; }
    public CurrencyCode ToCurrency { get; private set; }
    public decimal Rate { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidTo { get; private set; }

    public ExchangeRate(CurrencyCode fromCurrency, CurrencyCode toCurrency, decimal rate, DateTime validFrom, DateTime validTo)
    {
        if (fromCurrency == toCurrency)
            throw new ArgumentException("From and To currencies cannot be the same");
        
        if (rate <= 0)
            throw new ArgumentException("Exchange rate must be positive", nameof(rate));
        
        if (validTo <= validFrom)
            throw new ArgumentException("Valid to date must be after valid from date");

        FromCurrency = fromCurrency;
        ToCurrency = toCurrency;
        Rate = Math.Round(rate, 6);
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public Money Convert(Money amount)
    {
        if (amount.Currency != FromCurrency)
            throw new InvalidOperationException($"Amount currency {amount.Currency} does not match exchange rate from currency {FromCurrency}");
        
        if (!IsValid(DateTime.UtcNow))
            throw new InvalidOperationException("Exchange rate is not valid for current date");

        var convertedAmount = amount.Amount * Rate;
        return new Money(convertedAmount, ToCurrency);
    }

    public bool IsValid(DateTime date) => date >= ValidFrom && date <= ValidTo;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FromCurrency;
        yield return ToCurrency;
        yield return Rate;
        yield return ValidFrom;
        yield return ValidTo;
    }

    public override string ToString() => $"{FromCurrency}/{ToCurrency} = {Rate:F6}";
}

/// <summary>
/// Budget allocation value object
/// </summary>
public class BudgetAllocation : ValueObject
{
    public Money AllocatedAmount { get; private set; }
    public Money SpentAmount { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }

    public BudgetAllocation(Money allocatedAmount, DateTime periodStart, DateTime periodEnd)
    {
        if (periodEnd <= periodStart)
            throw new ArgumentException("Period end must be after period start");

        AllocatedAmount = allocatedAmount ?? throw new ArgumentNullException(nameof(allocatedAmount));
        SpentAmount = Money.Zero(allocatedAmount.Currency);
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
    }

    private BudgetAllocation(Money allocatedAmount, Money spentAmount, DateTime periodStart, DateTime periodEnd)
    {
        AllocatedAmount = allocatedAmount;
        SpentAmount = spentAmount;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
    }

    public BudgetAllocation SpendAmount(Money amount)
    {
        if (amount.Currency != AllocatedAmount.Currency)
            throw new InvalidOperationException("Currency mismatch for budget spending");

        var newSpentAmount = SpentAmount.Add(amount);
        return new BudgetAllocation(AllocatedAmount, newSpentAmount, PeriodStart, PeriodEnd);
    }

    public Money RemainingAmount => AllocatedAmount.Subtract(SpentAmount);
    public decimal UtilizationPercentage => AllocatedAmount.Amount == 0 ? 0 : (SpentAmount.Amount / AllocatedAmount.Amount) * 100;
    public bool IsOverBudget => SpentAmount.Amount > AllocatedAmount.Amount;
    public bool IsInCurrentPeriod => DateTime.UtcNow >= PeriodStart && DateTime.UtcNow <= PeriodEnd;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AllocatedAmount;
        yield return SpentAmount;
        yield return PeriodStart;
        yield return PeriodEnd;
    }

    public override string ToString() => $"Budget: {AllocatedAmount}, Spent: {SpentAmount}, Remaining: {RemainingAmount}";
}

/// <summary>
/// Billing period value object for subscription management
/// </summary>
public class BillingPeriod : ValueObject
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public BillingFrequency Frequency { get; private set; }

    public BillingPeriod(DateTime startDate, DateTime endDate, BillingFrequency frequency)
    {
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date");

        StartDate = startDate.Date;
        EndDate = endDate.Date;
        Frequency = frequency;
    }

    public TimeSpan Duration => EndDate - StartDate;
    public int DaysInPeriod => Duration.Days;
    public bool IsCurrentPeriod => DateTime.UtcNow.Date >= StartDate && DateTime.UtcNow.Date <= EndDate;
    public bool IsExpired => DateTime.UtcNow.Date > EndDate;

    public BillingPeriod GetNextPeriod()
    {
        var nextStart = EndDate.AddDays(1);
        var nextEnd = Frequency switch
        {
            BillingFrequency.Daily => nextStart,
            BillingFrequency.Weekly => nextStart.AddDays(6),
            BillingFrequency.Monthly => nextStart.AddMonths(1).AddDays(-1),
            BillingFrequency.Quarterly => nextStart.AddMonths(3).AddDays(-1),
            BillingFrequency.SemiAnnually => nextStart.AddMonths(6).AddDays(-1),
            BillingFrequency.Annually => nextStart.AddYears(1).AddDays(-1),
            BillingFrequency.Biennial => nextStart.AddYears(2).AddDays(-1),
            _ => throw new InvalidOperationException($"Unsupported billing frequency: {Frequency}")
        };

        return new BillingPeriod(nextStart, nextEnd, Frequency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
        yield return Frequency;
    }

    public override string ToString() => $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd} ({Frequency})";
}
