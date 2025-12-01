using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Assets;

/// <summary>
/// Represents a physical or intangible asset owned by a business.
/// </summary>
public class Asset : BaseAuditableEntity, IBusinessScopedEntity
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Name or description of the asset.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional asset code or serial number for identification.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Current estimated value of the asset.
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Purchase or acquisition cost of the asset.
    /// </summary>
    public decimal? PurchaseCost { get; set; }

    /// <summary>
    /// Date when the asset was acquired.
    /// </summary>
    public DateTimeOffset? PurchaseDate { get; set; }

    /// <summary>
    /// Physical location where the asset is stored or used.
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// The shop/store where this asset is located (optional).
    /// </summary>
    public int? ShopId { get; set; }
    public Store? Shop { get; set; }

    /// <summary>
    /// Current condition of the asset.
    /// </summary>
    public AssetCondition Condition { get; set; }

    /// <summary>
    /// Category or type of asset (e.g., "Equipment", "Vehicle", "Furniture", "IT Equipment").
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Brand or manufacturer of the asset.
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// Model number or identifier.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Serial number for tracking.
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// Additional notes or description.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Whether the asset is currently active and in use.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Collection of maintenance logs for this asset.
    /// </summary>
    public ICollection<AssetMaintenanceLog> MaintenanceLogs { get; private set; } = new List<AssetMaintenanceLog>();
}

