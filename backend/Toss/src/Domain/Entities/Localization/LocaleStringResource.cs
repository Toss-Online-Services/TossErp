namespace Toss.Domain.Entities.Localization;

/// <summary>
/// Represents a locale string resource for translations
/// </summary>
public class LocaleStringResource : BaseEntity
{
    public LocaleStringResource()
    {
        ResourceName = string.Empty;
        ResourceValue = string.Empty;
    }

    /// <summary>
    /// Gets or sets the language ID
    /// </summary>
    public int LanguageId { get; set; }
    public Language? Language { get; set; }

    /// <summary>
    /// Gets or sets the resource name/key
    /// </summary>
    public string ResourceName { get; set; }

    /// <summary>
    /// Gets or sets the resource value (translated text)
    /// </summary>
    public string ResourceValue { get; set; }
}

