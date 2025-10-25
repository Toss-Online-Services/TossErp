namespace Toss.Domain.Entities.Directory;

/// <summary>
/// Represents a state/province
/// </summary>
public class StateProvince : BaseAuditableEntity
{
    public StateProvince()
    {
        Published = true;
        DisplayOrder = 0;
        Name = string.Empty;
    }

    /// <summary>
    /// Gets or sets the country ID
    /// </summary>
    public int CountryId { get; set; }
    public Country? Country { get; set; }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the abbreviation
    /// </summary>
    public string? Abbreviation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is published
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}

