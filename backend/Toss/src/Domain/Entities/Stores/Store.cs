using Toss.Domain.Entities.Businesses;

namespace Toss.Domain.Entities.Stores;

/// <summary>
/// Represents a shop/store (unified concept for township shops and multi-store management)
/// </summary>
public class Store : BaseAuditableEntity
{
    public Store()
    {
        Name = string.Empty;
        OwnerId = string.Empty;
        IsActive = true;
        Currency = "ZAR";
        TaxRate = 15m;
        Language = "en";
        Timezone = "Africa/Johannesburg";
        WhatsAppAlertsEnabled = true;
        GroupBuyingEnabled = true;
        AIAssistantEnabled = true;
        Url = string.Empty;
        Ssl_enabled = false;
        Hosts = string.Empty;
        DisplayOrder = 0;
    }

    /// <summary>
    /// Gets or sets the business/tenant identifier.
    /// </summary>
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the shop/store name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the owner ID
    /// </summary>
    public string OwnerId { get; set; }

    /// <summary>
    /// Gets or sets the location (GPS coordinates)
    /// </summary>
    public Location? Location { get; set; }

    /// <summary>
    /// Gets or sets the address ID
    /// </summary>
    public int? AddressId { get; set; }
    public Address? Address { get; set; }

    /// <summary>
    /// Gets or sets the contact phone number
    /// </summary>
    public PhoneNumber? ContactPhone { get; set; }

    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets whether the shop is active
    /// </summary>
    public bool IsActive { get; set; }

    // Settings
    /// <summary>
    /// Gets or sets the currency code
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Gets or sets the tax rate (e.g., 15% VAT in South Africa)
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Gets or sets the language code
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the timezone
    /// </summary>
    public string Timezone { get; set; }

    /// <summary>
    /// Gets or sets the area group (for geographic organization)
    /// </summary>
    public string? AreaGroup { get; set; }

    // Business hours
    /// <summary>
    /// Gets or sets the opening time
    /// </summary>
    public TimeOnly? OpeningTime { get; set; }

    /// <summary>
    /// Gets or sets the closing time
    /// </summary>
    public TimeOnly? ClosingTime { get; set; }

    // Feature flags
    /// <summary>
    /// Gets or sets whether WhatsApp alerts are enabled
    /// </summary>
    public bool WhatsAppAlertsEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether group buying is enabled
    /// </summary>
    public bool GroupBuyingEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether AI assistant is enabled
    /// </summary>
    public bool AIAssistantEnabled { get; set; }

    // Multi-store/Ecommerce properties (from Store)
    /// <summary>
    /// Gets or sets the store URL (for ecommerce/web scenarios)
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets whether SSL is enabled
    /// </summary>
    public bool Ssl_enabled { get; set; }

    /// <summary>
    /// Gets or sets the comma-separated list of possible HTTP_HOST values
    /// </summary>
    public string Hosts { get; set; }

    /// <summary>
    /// Gets or sets the default language ID (for multi-language support)
    /// </summary>
    public int? DefaultLanguageId { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the company name
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Gets or sets the company address (text representation)
    /// </summary>
    public string? CompanyAddress { get; set; }

    /// <summary>
    /// Gets or sets the company phone number
    /// </summary>
    public string? CompanyPhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the company VAT number
    /// </summary>
    public string? CompanyVat { get; set; }
}

