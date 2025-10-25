namespace Toss.Domain.Entities.Stores;

/// <summary>
/// Represents a store (extends the base Shop concept for multi-store scenarios)
/// </summary>
public class Store : BaseAuditableEntity
{
    public Store()
    {
        Name = string.Empty;
        Url = string.Empty;
        Ssl_enabled = false;
        Hosts = string.Empty;
        DisplayOrder = 0;
        StoreMappings = new List<StoreMapping>();
    }

    /// <summary>
    /// Gets or sets the store name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the store URL
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether SSL is enabled
    /// </summary>
    public bool Ssl_enabled { get; set; }

    /// <summary>
    /// Gets or sets the comma-separated list of possible HTTP_HOST values
    /// </summary>
    public string Hosts { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the default language for this store
    /// </summary>
    public int DefaultLanguageId { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the company name
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Gets or sets the company address
    /// </summary>
    public string? CompanyAddress { get; set; }

    /// <summary>
    /// Gets or sets the store phone number
    /// </summary>
    public string? CompanyPhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the company VAT (used in South Africa)
    /// </summary>
    public string? CompanyVat { get; set; }

    /// <summary>
    /// Gets or sets the store mappings
    /// </summary>
    public ICollection<StoreMapping> StoreMappings { get; set; }
}

