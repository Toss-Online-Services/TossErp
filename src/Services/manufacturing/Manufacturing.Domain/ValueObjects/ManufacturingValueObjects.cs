using TossErp.Manufacturing.Domain.SeedWork;

namespace TossErp.Manufacturing.Domain.ValueObjects;

/// <summary>
/// Work Order Number value object
/// </summary>
public class WorkOrderNumber : ValueObject
{
    public string Value { get; }

    public WorkOrderNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Work order number cannot be empty", nameof(value));

        if (value.Length > 20)
            throw new ArgumentException("Work order number cannot exceed 20 characters", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(WorkOrderNumber workOrderNumber) => workOrderNumber.Value;
    public static implicit operator WorkOrderNumber(string value) => new(value);
    
    public override string ToString() => Value;
}

/// <summary>
/// BOM (Bill of Materials) Number value object
/// </summary>
public class BOMNumber : ValueObject
{
    public string Value { get; }

    public BOMNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("BOM number cannot be empty", nameof(value));

        if (value.Length > 20)
            throw new ArgumentException("BOM number cannot exceed 20 characters", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(BOMNumber bomNumber) => bomNumber.Value;
    public static implicit operator BOMNumber(string value) => new(value);
    
    public override string ToString() => Value;
}

/// <summary>
/// Quantity value object for manufacturing
/// </summary>
public class Quantity : ValueObject
{
    public decimal Value { get; }
    public string UOM { get; }

    public Quantity(decimal value, string uom)
    {
        if (value < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(value));

        if (string.IsNullOrWhiteSpace(uom))
            throw new ArgumentException("Unit of measure cannot be empty", nameof(uom));

        Value = value;
        UOM = uom.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return UOM;
    }

    public Quantity Add(Quantity other)
    {
        if (UOM != other.UOM)
            throw new InvalidOperationException($"Cannot add quantities with different UOMs: {UOM} vs {other.UOM}");

        return new Quantity(Value + other.Value, UOM);
    }

    public Quantity Subtract(Quantity other)
    {
        if (UOM != other.UOM)
            throw new InvalidOperationException($"Cannot subtract quantities with different UOMs: {UOM} vs {other.UOM}");

        var result = Value - other.Value;
        if (result < 0)
            throw new InvalidOperationException("Result quantity cannot be negative");

        return new Quantity(result, UOM);
    }

    public override string ToString() => $"{Value:N2} {UOM}";
}

/// <summary>
/// Production time value object
/// </summary>
public class ProductionTime : ValueObject
{
    public TimeSpan Value { get; }

    public ProductionTime(TimeSpan value)
    {
        if (value < TimeSpan.Zero)
            throw new ArgumentException("Production time cannot be negative", nameof(value));

        Value = value;
    }

    public ProductionTime(double hours)
    {
        if (hours < 0)
            throw new ArgumentException("Production time cannot be negative", nameof(hours));

        Value = TimeSpan.FromHours(hours);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public ProductionTime Add(ProductionTime other) => new(Value + other.Value);

    public double TotalHours => Value.TotalHours;
    public double TotalMinutes => Value.TotalMinutes;

    public override string ToString() => $"{Value.TotalHours:F2} hours";
}
