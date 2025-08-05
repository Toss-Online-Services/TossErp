using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Domain.ValueObjects;

public sealed record WarehouseCode
{
    public string Value { get; private set; }

    private WarehouseCode() { Value = string.Empty; } // For EF Core

    public WarehouseCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Warehouse code cannot be empty.", nameof(value));

        if (value.Length > 20)
            throw new ArgumentException("Warehouse code cannot exceed 20 characters.", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static implicit operator string(WarehouseCode warehouseCode) => warehouseCode.Value;
    public static explicit operator WarehouseCode(string value) => new(value);

    public override string ToString() => Value;
} 
