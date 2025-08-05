namespace TossErp.Stock.Domain.ValueObjects;

public sealed record ItemCode
{
    public string Value { get; }

    public ItemCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Item code cannot be empty.", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("Item code cannot exceed 50 characters.", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static implicit operator string(ItemCode itemCode) => itemCode.Value;
    public static explicit operator ItemCode(string value) => new(value);

    public override string ToString() => Value;
} 
