namespace TossErp.Stock.Domain.ValueObjects;

public sealed record BinCode
{
    public string Value { get; }

    public BinCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Bin code cannot be empty.", nameof(value));

        if (value.Length > 20)
            throw new ArgumentException("Bin code cannot exceed 20 characters.", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static implicit operator string(BinCode binCode) => binCode.Value;
    public static explicit operator BinCode(string value) => new(value);

    public override string ToString() => Value;
} 