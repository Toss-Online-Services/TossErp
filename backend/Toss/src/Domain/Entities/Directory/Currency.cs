namespace Toss.Domain.Entities.Directory;

/// <summary>
/// Represents a currency
/// </summary>
public class Currency : BaseAuditableEntity
{
    public Currency()
    {
        Published = true;
        DisplayOrder = 0;
        Rate = 1.0m;
        Name = string.Empty;
        CurrencyCode = string.Empty;
        DisplayLocale = string.Empty;
    }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the currency code (e.g., ZAR, USD)
    /// </summary>
    public string CurrencyCode { get; set; }

    /// <summary>
    /// Gets or sets the exchange rate against the primary currency
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// Gets or sets the display locale (e.g., en-ZA)
    /// </summary>
    public string DisplayLocale { get; set; }

    /// <summary>
    /// Gets or sets the custom formatting string
    /// </summary>
    public string? CustomFormatting { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is published
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance update
    /// </summary>
    public DateTime UpdatedOnUtc { get; set; }
}

