namespace Toss.Domain.Entities.Localization;

/// <summary>
/// Represents a language for multi-language support in TOSS
/// </summary>
public class Language : BaseAuditableEntity
{
    public Language()
    {
        Published = true;
        DisplayOrder = 0;
    }

    /// <summary>
    /// Gets or sets the name (e.g., "English", "isiZulu")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the language culture code (e.g., "en-ZA", "zu-ZA")
    /// </summary>
    public string LanguageCulture { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique SEO code (e.g., "en", "zu")
    /// </summary>
    public string UniqueSeoCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the flag image file name
    /// </summary>
    public string? FlagImageFileName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the language supports right-to-left
    /// </summary>
    public bool Rtl { get; set; }

    /// <summary>
    /// Gets or sets whether the language is published and available
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}

