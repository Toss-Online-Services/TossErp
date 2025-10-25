namespace Toss.Domain.Entities.Vendors;

/// <summary>
/// Represents a vendor (enhanced supplier for marketplace scenarios)
/// </summary>
public class Vendor : BaseAuditableEntity
{
    public Vendor()
    {
        Name = string.Empty;
        Email = string.Empty;
        Active = true;
        DisplayOrder = 0;
        VendorNotes = new List<VendorNote>();
    }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is active
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the vendor notes
    /// </summary>
    public ICollection<VendorNote> VendorNotes { get; set; }

    /// <summary>
    /// Gets or sets the address ID
    /// </summary>
    public int? AddressId { get; set; }
    public Address? Address { get; set; }
}

