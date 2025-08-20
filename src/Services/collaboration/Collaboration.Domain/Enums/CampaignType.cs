namespace Collaboration.Domain.Enums;

/// <summary>
/// Represents the type of group-buy campaign
/// </summary>
public enum CampaignType
{
    /// <summary>
    /// Product-based group-buy (e.g., electronics, clothing)
    /// </summary>
    Product = 0,

    /// <summary>
    /// Service-based group-buy (e.g., consulting, training)
    /// </summary>
    Service = 1,

    /// <summary>
    /// Bulk purchase of raw materials
    /// </summary>
    RawMaterials = 2,

    /// <summary>
    /// Equipment or machinery group-buy
    /// </summary>
    Equipment = 3,

    /// <summary>
    /// Software or digital product group-buy
    /// </summary>
    Software = 4,

    /// <summary>
    /// Custom or specialized group-buy
    /// </summary>
    Custom = 5
}
