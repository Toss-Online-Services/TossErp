namespace Toss.Domain.Entities.Directory;

/// <summary>
/// Represents a measure dimension (length)
/// </summary>
public class MeasureDimension : BaseEntity
{
    public MeasureDimension()
    {
        SystemKeyword = string.Empty;
        Name = string.Empty;
        Ratio = 1.0m;
        DisplayOrder = 0;
    }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the system keyword
    /// </summary>
    public string SystemKeyword { get; set; }

    /// <summary>
    /// Gets or sets the ratio to the primary dimension
    /// </summary>
    public decimal Ratio { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}

