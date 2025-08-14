using System.Collections.Generic;
using System.Text.RegularExpressions;
using TossErp.Stock.Domain.SeedWork;

namespace TossErp.Stock.Domain.ValueObjects;

public class SKU : ValueObject
{
    public string Value { get; }

    public SKU(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("SKU cannot be empty");

        if (value.Length > 50)
            throw new DomainException("SKU cannot exceed 50 characters");

        // Allow alphanumeric characters, hyphens, and underscores
        if (!Regex.IsMatch(value, @"^[A-Za-z0-9\-_]+$"))
            throw new DomainException("SKU can only contain alphanumeric characters, hyphens, and underscores");

        Value = value.ToUpperInvariant();
    }

    public static implicit operator string(SKU sku) => sku.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
