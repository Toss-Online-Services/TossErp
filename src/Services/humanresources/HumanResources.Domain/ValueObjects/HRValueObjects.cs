using TossErp.HumanResources.Domain.SeedWork;

namespace TossErp.HumanResources.Domain.ValueObjects;

/// <summary>
/// Employee Number value object
/// </summary>
public class EmployeeNumber : ValueObject
{
    public string Value { get; }

    public EmployeeNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Employee number cannot be empty", nameof(value));

        if (value.Length > 20)
            throw new ArgumentException("Employee number cannot exceed 20 characters", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(EmployeeNumber employeeNumber) => employeeNumber.Value;
    public static implicit operator EmployeeNumber(string value) => new(value);
    
    public override string ToString() => Value;
}

/// <summary>
/// Salary amount value object
/// </summary>
public class Salary : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Salary(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Salary amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public Salary Add(Salary other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add salaries with different currencies: {Currency} vs {other.Currency}");

        return new Salary(Amount + other.Amount, Currency);
    }

    public Salary Subtract(Salary other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot subtract salaries with different currencies: {Currency} vs {other.Currency}");

        var result = Amount - other.Amount;
        if (result < 0)
            throw new InvalidOperationException("Result salary cannot be negative");

        return new Salary(result, Currency);
    }

    public Salary Multiply(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("Factor cannot be negative", nameof(factor));

        return new Salary(Amount * factor, Currency);
    }

    public override string ToString() => $"{Amount:C} {Currency}";
}

/// <summary>
/// Phone number value object
/// </summary>
public class PhoneNumber : ValueObject
{
    public string CountryCode { get; }
    public string Number { get; }

    public PhoneNumber(string number, string countryCode = "+1")
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Phone number cannot be empty", nameof(number));

        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentException("Country code cannot be empty", nameof(countryCode));

        // Basic validation - remove all non-numeric characters
        var cleanNumber = new string(number.Where(char.IsDigit).ToArray());
        
        if (cleanNumber.Length < 7 || cleanNumber.Length > 15)
            throw new ArgumentException("Phone number must be between 7 and 15 digits", nameof(number));

        Number = cleanNumber;
        CountryCode = countryCode.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CountryCode;
        yield return Number;
    }

    public string FullNumber => $"{CountryCode}{Number}";

    public override string ToString() => $"{CountryCode} {Number}";
}

/// <summary>
/// Email address value object
/// </summary>
public class EmailAddress : ValueObject
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email address cannot be empty", nameof(value));

        // Basic email validation
        var email = value.Trim().ToLowerInvariant();
        
        if (!email.Contains('@') || !email.Contains('.'))
            throw new ArgumentException("Invalid email address format", nameof(value));

        if (email.Length > 254)
            throw new ArgumentException("Email address cannot exceed 254 characters", nameof(value));

        Value = email;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(EmailAddress email) => email.Value;
    public static implicit operator EmailAddress(string value) => new(value);
    
    public override string ToString() => Value;
}

/// <summary>
/// Address value object
/// </summary>
public class Address : ValueObject
{
    public string Street { get; }
    public string? Street2 { get; }
    public string City { get; }
    public string? State { get; }
    public string PostalCode { get; }
    public string Country { get; }

    public Address(
        string street,
        string city,
        string postalCode,
        string country,
        string? street2 = null,
        string? state = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street address cannot be empty", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty", nameof(postalCode));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));

        Street = street.Trim();
        Street2 = street2?.Trim();
        City = city.Trim();
        State = state?.Trim();
        PostalCode = postalCode.Trim();
        Country = country.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return Street2 ?? string.Empty;
        yield return City;
        yield return State ?? string.Empty;
        yield return PostalCode;
        yield return Country;
    }

    public override string ToString()
    {
        var parts = new List<string> { Street };
        
        if (!string.IsNullOrWhiteSpace(Street2))
            parts.Add(Street2);
            
        parts.Add(City);
        
        if (!string.IsNullOrWhiteSpace(State))
            parts.Add(State);
            
        parts.Add(PostalCode);
        parts.Add(Country);

        return string.Join(", ", parts);
    }
}

/// <summary>
/// Work hours value object
/// </summary>
public class WorkHours : ValueObject
{
    public TimeSpan Value { get; }

    public WorkHours(TimeSpan value)
    {
        if (value < TimeSpan.Zero)
            throw new ArgumentException("Work hours cannot be negative", nameof(value));

        if (value > TimeSpan.FromHours(24))
            throw new ArgumentException("Work hours cannot exceed 24 hours", nameof(value));

        Value = value;
    }

    public WorkHours(double hours)
    {
        if (hours < 0)
            throw new ArgumentException("Work hours cannot be negative", nameof(hours));

        if (hours > 24)
            throw new ArgumentException("Work hours cannot exceed 24 hours", nameof(hours));

        Value = TimeSpan.FromHours(hours);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public WorkHours Add(WorkHours other) => new(Value + other.Value);

    public double TotalHours => Value.TotalHours;
    public double TotalMinutes => Value.TotalMinutes;

    public override string ToString() => $"{Value.TotalHours:F2} hours";
}
