namespace Toss.Domain.Entities.Tax;

/// <summary>
/// Represents a tax category
/// </summary>
public class TaxCategory : BaseEntity
{
    public TaxCategory()
    {
        Name = string.Empty;
        DisplayOrder = 0;
    }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}

