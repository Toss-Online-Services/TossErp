namespace Toss.Domain.Entities.Localization;

/// <summary>
/// Represents a localized property value for entities
/// </summary>
public class LocalizedProperty : BaseEntity
{
    public LocalizedProperty()
    {
        LocaleKeyGroup = string.Empty;
        LocaleKey = string.Empty;
        LocaleValue = string.Empty;
    }

    /// <summary>
    /// Gets or sets the entity ID
    /// </summary>
    public int EntityId { get; set; }

    /// <summary>
    /// Gets or sets the language ID
    /// </summary>
    public int LanguageId { get; set; }
    public Language? Language { get; set; }

    /// <summary>
    /// Gets or sets the locale key group (entity name)
    /// </summary>
    public string LocaleKeyGroup { get; set; }

    /// <summary>
    /// Gets or sets the locale key (property name)
    /// </summary>
    public string LocaleKey { get; set; }

    /// <summary>
    /// Gets or sets the locale value (translated value)
    /// </summary>
    public string LocaleValue { get; set; }
}

