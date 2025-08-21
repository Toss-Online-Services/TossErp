using TossErp.AssetManagement.Domain.SeedWork;

namespace TossErp.AssetManagement.Domain.ValueObjects;

/// <summary>
/// Asset Tag/Serial Number - Unique identifier for physical tracking
/// </summary>
public class AssetTag : ValueObject
{
    public string Value { get; private set; }

    public AssetTag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Asset tag cannot be empty", nameof(value));
        if (value.Length > 50)
            throw new ArgumentException("Asset tag cannot exceed 50 characters", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

/// <summary>
/// Asset Location - Physical or logical location of an asset
/// </summary>
public class AssetLocation : ValueObject
{
    public string Building { get; private set; }
    public string Floor { get; private set; }
    public string Room { get; private set; }
    public string Department { get; private set; }
    public string? Notes { get; private set; }

    public AssetLocation(string building, string floor, string room, string department, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(building))
            throw new ArgumentException("Building cannot be empty", nameof(building));
        if (string.IsNullOrWhiteSpace(floor))
            throw new ArgumentException("Floor cannot be empty", nameof(floor));
        if (string.IsNullOrWhiteSpace(room))
            throw new ArgumentException("Room cannot be empty", nameof(room));
        if (string.IsNullOrWhiteSpace(department))
            throw new ArgumentException("Department cannot be empty", nameof(department));

        Building = building.Trim();
        Floor = floor.Trim();
        Room = room.Trim();
        Department = department.Trim();
        Notes = notes?.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Building;
        yield return Floor;
        yield return Room;
        yield return Department;
        yield return Notes ?? string.Empty;
    }

    public override string ToString() => $"{Building}-{Floor}-{Room} ({Department})";
}

/// <summary>
/// Asset Financial Information - Cost and depreciation details
/// </summary>
public class AssetFinancialInfo : ValueObject
{
    public decimal PurchasePrice { get; private set; }
    public decimal CurrentValue { get; private set; }
    public decimal SalvageValue { get; private set; }
    public string Currency { get; private set; }
    public DateTime? PurchaseDate { get; private set; }

    public AssetFinancialInfo(
        decimal purchasePrice, 
        decimal currentValue, 
        decimal salvageValue, 
        string currency,
        DateTime? purchaseDate = null)
    {
        if (purchasePrice < 0)
            throw new ArgumentException("Purchase price cannot be negative", nameof(purchasePrice));
        if (currentValue < 0)
            throw new ArgumentException("Current value cannot be negative", nameof(currentValue));
        if (salvageValue < 0)
            throw new ArgumentException("Salvage value cannot be negative", nameof(salvageValue));
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));
        if (currency.Length != 3)
            throw new ArgumentException("Currency must be 3 characters (ISO code)", nameof(currency));

        PurchasePrice = purchasePrice;
        CurrentValue = currentValue;
        SalvageValue = salvageValue;
        Currency = currency.ToUpperInvariant();
        PurchaseDate = purchaseDate;
    }

    public decimal DepreciatedAmount => PurchasePrice - CurrentValue;
    public decimal DepreciationPercentage => PurchasePrice > 0 ? (DepreciatedAmount / PurchasePrice) * 100 : 0;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PurchasePrice;
        yield return CurrentValue;
        yield return SalvageValue;
        yield return Currency;
        yield return PurchaseDate ?? DateTime.MinValue;
    }
}

/// <summary>
/// Asset Warranty Information
/// </summary>
public class AssetWarranty : ValueObject
{
    public string Provider { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string? WarrantyNumber { get; private set; }
    public string? Terms { get; private set; }

    public AssetWarranty(
        string provider,
        DateTime startDate,
        DateTime endDate,
        string? warrantyNumber = null,
        string? terms = null)
    {
        if (string.IsNullOrWhiteSpace(provider))
            throw new ArgumentException("Warranty provider cannot be empty", nameof(provider));
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));

        Provider = provider.Trim();
        StartDate = startDate;
        EndDate = endDate;
        WarrantyNumber = warrantyNumber?.Trim();
        Terms = terms?.Trim();
    }

    public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
    public bool IsExpired => DateTime.UtcNow > EndDate;
    public TimeSpan RemainingTime => IsActive ? EndDate - DateTime.UtcNow : TimeSpan.Zero;
    public int DaysRemaining => IsActive ? (int)RemainingTime.TotalDays : 0;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Provider;
        yield return StartDate;
        yield return EndDate;
        yield return WarrantyNumber ?? string.Empty;
        yield return Terms ?? string.Empty;
    }
}

/// <summary>
/// Asset Specification - Technical specifications and characteristics
/// </summary>
public class AssetSpecification : ValueObject
{
    public Dictionary<string, string> Properties { get; private set; }

    public AssetSpecification(Dictionary<string, string> properties)
    {
        Properties = properties?.Where(p => !string.IsNullOrWhiteSpace(p.Key) && !string.IsNullOrWhiteSpace(p.Value))
                                .ToDictionary(p => p.Key.Trim(), p => p.Value.Trim()) 
                                ?? new Dictionary<string, string>();
    }

    public void AddProperty(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Property key cannot be empty", nameof(key));
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Property value cannot be empty", nameof(value));

        Properties[key.Trim()] = value.Trim();
    }

    public string? GetProperty(string key)
    {
        return Properties.TryGetValue(key?.Trim() ?? string.Empty, out var value) ? value : null;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (var property in Properties.OrderBy(p => p.Key))
        {
            yield return property.Key;
            yield return property.Value;
        }
    }
}

/// <summary>
/// Asset Dimensions - Physical dimensions and weight
/// </summary>
public class AssetDimensions : ValueObject
{
    public decimal Length { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    public decimal Weight { get; private set; }
    public string Unit { get; private set; }
    public string WeightUnit { get; private set; }

    public AssetDimensions(
        decimal length, 
        decimal width, 
        decimal height, 
        decimal weight,
        string unit = "cm",
        string weightUnit = "kg")
    {
        if (length < 0)
            throw new ArgumentException("Length cannot be negative", nameof(length));
        if (width < 0)
            throw new ArgumentException("Width cannot be negative", nameof(width));
        if (height < 0)
            throw new ArgumentException("Height cannot be negative", nameof(height));
        if (weight < 0)
            throw new ArgumentException("Weight cannot be negative", nameof(weight));

        Length = length;
        Width = width;
        Height = height;
        Weight = weight;
        Unit = unit.Trim().ToLowerInvariant();
        WeightUnit = weightUnit.Trim().ToLowerInvariant();
    }

    public decimal Volume => Length * Width * Height;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Length;
        yield return Width;
        yield return Height;
        yield return Weight;
        yield return Unit;
        yield return WeightUnit;
    }

    public override string ToString() => $"{Length}×{Width}×{Height} {Unit}, {Weight} {WeightUnit}";
}
