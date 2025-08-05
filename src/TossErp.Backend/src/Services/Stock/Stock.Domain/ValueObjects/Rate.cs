using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Domain.ValueObjects;

public sealed record Rate
{
    public decimal Value { get; private set; }
    public string Currency { get; private set; }

    private Rate() { Value = 0; Currency = "ZAR"; } // For EF Core

    public Rate(decimal value, string currency = "ZAR")
    {
        if (value < 0)
            throw new ArgumentException("Rate cannot be negative.", nameof(value));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty.", nameof(currency));

        Value = Math.Round(value, 4);
        Currency = currency.Trim().ToUpperInvariant();
    }

    public static implicit operator decimal(Rate rate) => rate.Value;
    public static explicit operator Rate(decimal value) => new(value);

    public override string ToString() => $"{Value:F4} {Currency}";
} 
