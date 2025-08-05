namespace TossErp.Stock.Domain.ValueObjects;

public record Quantity
{
    public decimal Value { get; private set; }

    private Quantity() { Value = 0; } // For EF Core

    public Quantity(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(value));

        Value = value;
    }

    public static implicit operator decimal(Quantity quantity) => quantity.Value;
    public static explicit operator Quantity(decimal value) => new(value);

    public static Quantity operator +(Quantity a, Quantity b) => new(a.Value + b.Value);
    public static Quantity operator -(Quantity a, Quantity b) => new(Math.Max(0, a.Value - b.Value));
    public static bool operator >(Quantity a, Quantity b) => a.Value > b.Value;
    public static bool operator <(Quantity a, Quantity b) => a.Value < b.Value;
    public static bool operator >=(Quantity a, Quantity b) => a.Value >= b.Value;
    public static bool operator <=(Quantity a, Quantity b) => a.Value <= b.Value;

    public override string ToString() => Value.ToString("F2");
} 
