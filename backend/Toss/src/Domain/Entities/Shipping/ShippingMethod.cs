namespace Toss.Domain.Entities.Shipping;

/// <summary>
/// Represents a shipping method
/// </summary>
public class ShippingMethod : BaseAuditableEntity
{
    public ShippingMethod()
    {
        Name = string.Empty;
        DisplayOrder = 0;
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
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}

