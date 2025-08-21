using TossErp.CRM.Domain.SeedWork;

namespace TossErp.CRM.Domain.ValueObjects;

/// <summary>
/// Customer Number - Unique identifier for customers
/// </summary>
public class CustomerNumber : ValueObject
{
    public string Value { get; private set; }

    public CustomerNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Customer number cannot be empty", nameof(value));
        if (value.Length > 20)
            throw new ArgumentException("Customer number cannot exceed 20 characters", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

/// <summary>
/// Email Address value object with validation
/// </summary>
public class EmailAddress : ValueObject
{
    public string Value { get; private set; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email address cannot be empty", nameof(value));
        
        var trimmed = value.Trim().ToLowerInvariant();
        
        if (!IsValidEmail(trimmed))
            throw new ArgumentException("Invalid email address format", nameof(value));

        Value = trimmed;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

/// <summary>
/// Phone Number value object with formatting
/// </summary>
public class PhoneNumber : ValueObject
{
    public string Value { get; private set; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty", nameof(value));

        // Remove all non-numeric characters except + for international numbers
        var cleaned = System.Text.RegularExpressions.Regex.Replace(value, @"[^\d+]", "");
        
        if (cleaned.Length < 7 || cleaned.Length > 15)
            throw new ArgumentException("Phone number must be between 7 and 15 digits", nameof(value));

        Value = cleaned;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

/// <summary>
/// Address value object for customer/contact locations
/// </summary>
public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }

    public Address(string street, string city, string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));

        Street = street.Trim();
        City = city.Trim();
        State = state?.Trim() ?? string.Empty;
        PostalCode = postalCode?.Trim() ?? string.Empty;
        Country = country.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return PostalCode;
        yield return Country;
    }

    public override string ToString() => $"{Street}, {City}, {State} {PostalCode}, {Country}";
}

/// <summary>
/// Money value object for financial amounts
/// </summary>
public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));
        if (currency.Length != 3)
            throw new ArgumentException("Currency must be 3 characters (ISO code)", nameof(currency));

        Amount = Math.Round(amount, 2);
        Currency = currency.ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
        
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot subtract different currencies");
        
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        return new Money(Amount * multiplier, Currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:C} {Currency}";
}

/// <summary>
/// Lead Score value object for qualification tracking
/// </summary>
public class LeadScore : ValueObject
{
    public int Value { get; private set; }

    public LeadScore(int value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentException("Lead score must be between 0 and 100", nameof(value));

        Value = value;
    }

    public LeadScore Increase(int points)
    {
        return new LeadScore(Math.Min(100, Value + points));
    }

    public LeadScore Decrease(int points)
    {
        return new LeadScore(Math.Max(0, Value - points));
    }

    public bool IsQualified => Value >= 70;
    public bool IsHot => Value >= 90;
    public bool IsCold => Value <= 30;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => $"{Value}/100";
}

/// <summary>
/// Sales Target value object
/// </summary>
public class SalesTarget : ValueObject
{
    public Money TargetAmount { get; private set; }
    public DateTime StartPeriod { get; private set; }
    public DateTime EndPeriod { get; private set; }

    public SalesTarget(Money targetAmount, DateTime startPeriod, DateTime endPeriod)
    {
        if (endPeriod <= startPeriod)
            throw new ArgumentException("End period must be after start period");

        TargetAmount = targetAmount;
        StartPeriod = startPeriod;
        EndPeriod = endPeriod;
    }

    public TimeSpan Duration => EndPeriod - StartPeriod;
    public bool IsActive => DateTime.UtcNow >= StartPeriod && DateTime.UtcNow <= EndPeriod;
    public double ProgressPercentage => IsActive ? (DateTime.UtcNow - StartPeriod).TotalDays / Duration.TotalDays * 100 : 0;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TargetAmount;
        yield return StartPeriod;
        yield return EndPeriod;
    }
}

/// <summary>
/// Opportunity Value with probability
/// </summary>
public class OpportunityValue : ValueObject
{
    public Money EstimatedValue { get; private set; }
    public decimal Probability { get; private set; }

    public OpportunityValue(Money estimatedValue, decimal probability)
    {
        if (probability < 0 || probability > 100)
            throw new ArgumentException("Probability must be between 0 and 100", nameof(probability));

        EstimatedValue = estimatedValue;
        Probability = probability;
    }

    public Money WeightedValue => EstimatedValue.Multiply(Probability / 100);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EstimatedValue;
        yield return Probability;
    }

    public override string ToString() => $"{EstimatedValue} ({Probability}%)";
}
