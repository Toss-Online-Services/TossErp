using System.Collections.Generic;
using TossErp.Stock.Domain.SeedWork;

namespace TossErp.Stock.Domain.ValueObjects;

public class Quantity : ValueObject
{
    public decimal Value { get; }
    public string Unit { get; }

    public Quantity(decimal value, string unit)
    {
        if (value < 0)
            throw new DomainException("Quantity cannot be negative");

        if (string.IsNullOrWhiteSpace(unit))
            throw new DomainException("Unit cannot be empty");

        Value = value;
        Unit = unit.ToUpperInvariant();
    }

    public static Quantity Zero(string unit) => new(0, unit);

    public static Quantity operator +(Quantity left, Quantity right)
    {
        if (left.Unit != right.Unit)
            throw new DomainException("Cannot add quantities with different units");

        return new Quantity(left.Value + right.Value, left.Unit);
    }

    public static Quantity operator -(Quantity left, Quantity right)
    {
        if (left.Unit != right.Unit)
            throw new DomainException("Cannot subtract quantities with different units");

        var result = left.Value - right.Value;
        if (result < 0)
            throw new DomainException("Result cannot be negative");

        return new Quantity(result, left.Unit);
    }

    public static bool operator <(Quantity left, Quantity right)
    {
        if (left.Unit != right.Unit)
            throw new DomainException("Cannot compare quantities with different units");

        return left.Value < right.Value;
    }

    public static bool operator >(Quantity left, Quantity right)
    {
        if (left.Unit != right.Unit)
            throw new DomainException("Cannot compare quantities with different units");

        return left.Value > right.Value;
    }

    public static bool operator <=(Quantity left, Quantity right)
    {
        if (left.Unit != right.Unit)
            throw new DomainException("Cannot compare quantities with different units");

        return left.Value <= right.Value;
    }

    public static bool operator >=(Quantity left, Quantity right)
    {
        if (left.Unit != right.Unit)
            throw new DomainException("Cannot compare quantities with different units");

        return left.Value >= right.Value;
    }

    public override string ToString() => $"{Value} {Unit}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
    }
} 
