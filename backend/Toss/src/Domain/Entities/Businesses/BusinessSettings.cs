using Toss.Domain.Common;

namespace Toss.Domain.Entities.Businesses;

/// <summary>
/// Business-specific settings for localization, currency, and tax
/// </summary>
public class BusinessSettings : BaseAuditableEntity, IBusinessScopedEntity
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the currency code (e.g., "ZAR", "USD", "EUR")
    /// </summary>
    public string Currency { get; set; } = "ZAR";

    /// <summary>
    /// Gets or sets the currency symbol (e.g., "R", "$", "€")
    /// </summary>
    public string CurrencySymbol { get; set; } = "R";

    /// <summary>
    /// Gets or sets the VAT rate as a decimal (e.g., 0.15 for 15%)
    /// </summary>
    public decimal VatRate { get; set; } = 0.15m;

    /// <summary>
    /// Gets or sets whether prices include VAT by default
    /// </summary>
    public bool PricesIncludeVat { get; set; } = true;

    /// <summary>
    /// Gets or sets the date format string (e.g., "dd/MM/yyyy")
    /// </summary>
    public string DateFormat { get; set; } = "dd/MM/yyyy";

    /// <summary>
    /// Gets or sets the time format string (e.g., "HH:mm")
    /// </summary>
    public string TimeFormat { get; set; } = "HH:mm";

    /// <summary>
    /// Gets or sets the locale/culture code (e.g., "en-ZA", "af-ZA")
    /// </summary>
    public string Locale { get; set; } = "en-ZA";

    /// <summary>
    /// Gets or sets the timezone identifier (e.g., "South Africa Standard Time")
    /// </summary>
    public string Timezone { get; set; } = "South Africa Standard Time";
}

