namespace Toss.Domain.Entities.Vendors;

/// <summary>
/// Represents a vendor note
/// </summary>
public class VendorNote : BaseEntity
{
    public VendorNote()
    {
        Note = string.Empty;
        CreatedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets or sets the vendor ID
    /// </summary>
    public int VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    /// <summary>
    /// Gets or sets the note
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}

