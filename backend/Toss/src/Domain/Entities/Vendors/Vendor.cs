using Toss.Domain.Common;
using Toss.Domain.Entities.Localization;

namespace Toss.Domain.Entities.Vendors;

/// <summary>
/// Represents a vendor/supplier (unified entity for ERP and marketplace)
/// </summary>
public class Vendor : BaseAuditableEntity, IMetaTagsSupported, ILocalizedEntity, IBusinessScopedEntity
{
    public Vendor()
    {
        Name = string.Empty;
        Email = string.Empty;
        Active = true;
        DisplayOrder = 0;
        PaymentTermsDays = 30;
        VendorNotes = new List<VendorNote>();
        VendorProducts = new List<VendorProduct>();
    }

    /// <summary>
    /// Gets or sets the vendor name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the owning business identifier.
    /// </summary>
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the company registration number
    /// </summary>
    public string? CompanyRegNumber { get; set; }

    /// <summary>
    /// Gets or sets the VAT number
    /// </summary>
    public string? VATNumber { get; set; }

    /// <summary>
    /// Gets or sets the contact person name
    /// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// Gets or sets the contact phone
    /// </summary>
    public PhoneNumber? ContactPhone { get; set; }

    /// <summary>
    /// Alias for handlers expecting string PhoneNumber
    /// </summary>
    public string? PhoneNumber => ContactPhone?.ToString();

    /// <summary>
    /// Gets or sets the website
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is active
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the credit limit
    /// </summary>
    public decimal? CreditLimit { get; set; }

    /// <summary>
    /// Gets or sets the payment terms in days
    /// </summary>
    public int PaymentTermsDays { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the admin comment/notes
    /// </summary>
    public string? AdminComment { get; set; }

    /// <summary>
    /// Gets or sets the address ID
    /// </summary>
    public int? AddressId { get; set; }
    public Address? Address { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity has been deleted
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// Gets or sets the vendor notes
    /// </summary>
    public ICollection<VendorNote> VendorNotes { get; set; }

    /// <summary>
    /// Gets or sets the vendor products (product-vendor associations)
    /// </summary>
    public ICollection<VendorProduct> VendorProducts { get; set; }

    // SEO Properties
    /// <summary>
    /// Gets or sets the meta title for SEO
    /// </summary>
    public string? MetaTitle { get; set; }

    /// <summary>
    /// Gets or sets the meta keywords for SEO
    /// </summary>
    public string? MetaKeywords { get; set; }

    /// <summary>
    /// Gets or sets the meta description for SEO
    /// </summary>
    public string? MetaDescription { get; set; }

    // Alias for compatibility
    public bool IsActive => Active;
}
