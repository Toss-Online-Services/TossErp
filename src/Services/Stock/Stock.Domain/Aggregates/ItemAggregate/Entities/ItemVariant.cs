using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;

/// <summary>
/// Item Variant - Child entity of Item Aggregate
/// Represents different variations of an item (size, color, etc.)
/// </summary>
public class ItemVariant : Entity
{
    public string VariantCode { get; private set; } = string.Empty;
    public string VariantName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal? AdditionalCost { get; private set; }
    public bool IsActive { get; private set; }
    
    // Alias properties for compatibility with tests
    public string Size => VariantName;
    public string Value => VariantCode;
    public string Code => VariantCode;
    public decimal Price => AdditionalCost ?? 0;

    protected ItemVariant() { } // For EF Core

    public ItemVariant(string variantCode, string variantName, string? description = null, decimal? additionalCost = null)
    {
        if (string.IsNullOrWhiteSpace(variantCode))
            throw new ArgumentException("Variant code cannot be empty", nameof(variantCode));
        if (string.IsNullOrWhiteSpace(variantName))
            throw new ArgumentException("Variant name cannot be empty", nameof(variantName));

        VariantCode = variantCode.Trim().ToUpperInvariant();
        VariantName = variantName.Trim();
        Description = description?.Trim();
        AdditionalCost = additionalCost;
        IsActive = true;
    }

    // Constructor for compatibility with tests
    public ItemVariant(string size, string value, string code, decimal price)
    {
        if (string.IsNullOrWhiteSpace(size))
            throw new ArgumentException("Size cannot be empty", nameof(size));
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty", nameof(value));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));

        VariantName = size.Trim();
        VariantCode = code.Trim().ToUpperInvariant();
        Description = value.Trim();
        AdditionalCost = price;
        IsActive = true;
    }

    public void UpdateInfo(string variantName, string? description)
    {
        if (string.IsNullOrWhiteSpace(variantName))
            throw new ArgumentException("Variant name cannot be empty", nameof(variantName));

        VariantName = variantName.Trim();
        Description = description?.Trim();
    }

    public void UpdateAdditionalCost(decimal? additionalCost)
    {
        if (additionalCost.HasValue && additionalCost.Value < 0)
            throw new ArgumentException("Additional cost cannot be negative", nameof(additionalCost));

        AdditionalCost = additionalCost;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public bool IsAvailable() => IsActive;
} 
