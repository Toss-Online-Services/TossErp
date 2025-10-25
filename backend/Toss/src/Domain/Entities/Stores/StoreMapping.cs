namespace Toss.Domain.Entities.Stores;

/// <summary>
/// Represents a store mapping for entity-store associations
/// </summary>
public class StoreMapping : BaseEntity
{
    public StoreMapping()
    {
        EntityName = string.Empty;
    }

    /// <summary>
    /// Gets or sets the entity ID
    /// </summary>
    public int EntityId { get; set; }

    /// <summary>
    /// Gets or sets the entity name (e.g., "Product", "Category")
    /// </summary>
    public string EntityName { get; set; }

    /// <summary>
    /// Gets or sets the store ID
    /// </summary>
    public int StoreId { get; set; }
    public Store? Store { get; set; }
}

