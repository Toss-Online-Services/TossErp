namespace Toss.Domain.Entities.Tax;

/// <summary>
/// Represents a tax rate
/// </summary>
public class TaxRate : BaseAuditableEntity
{
    public TaxRate()
    {
        Percentage = 0;
        Name = string.Empty;
    }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the tax category ID
    /// </summary>
    public int TaxCategoryId { get; set; }
    public TaxCategory? TaxCategory { get; set; }

    /// <summary>
    /// Gets or sets the country ID
    /// </summary>
    public int? CountryId { get; set; }

    /// <summary>
    /// Gets or sets the state/province ID
    /// </summary>
    public int? StateProvinceId { get; set; }

    /// <summary>
    /// Gets or sets the ZIP/postal code
    /// </summary>
    public string? Zip { get; set; }

    /// <summary>
    /// Gets or sets the percentage
    /// </summary>
    public decimal Percentage { get; set; }
}

