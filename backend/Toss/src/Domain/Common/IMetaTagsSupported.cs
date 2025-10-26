namespace Toss.Domain.Common;

/// <summary>
/// Represents an entity that supports meta tags
/// </summary>
public interface IMetaTagsSupported
{
    /// <summary>
    /// Gets or sets the meta keywords
    /// </summary>
    string? MetaKeywords { get; set; }

    /// <summary>
    /// Gets or sets the meta description
    /// </summary>
    string? MetaDescription { get; set; }

    /// <summary>
    /// Gets or sets the meta title
    /// </summary>
    string? MetaTitle { get; set; }
}

