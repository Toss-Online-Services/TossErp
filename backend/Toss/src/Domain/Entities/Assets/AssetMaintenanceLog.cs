using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;

namespace Toss.Domain.Entities.Assets;

/// <summary>
/// Represents a maintenance record for an asset.
/// </summary>
public class AssetMaintenanceLog : BaseAuditableEntity, IBusinessScopedEntity
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// The asset this maintenance log belongs to.
    /// </summary>
    public int AssetId { get; set; }
    public Asset Asset { get; set; } = null!;

    /// <summary>
    /// Date when the maintenance was performed.
    /// </summary>
    public DateTimeOffset MaintenanceDate { get; set; }

    /// <summary>
    /// Type of maintenance (e.g., "Routine", "Repair", "Inspection", "Upgrade").
    /// </summary>
    public string MaintenanceType { get; set; } = string.Empty;

    /// <summary>
    /// Description of the maintenance work performed.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Cost of the maintenance.
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// Name of the service provider or technician who performed the maintenance.
    /// </summary>
    public string? ServiceProvider { get; set; }

    /// <summary>
    /// Next scheduled maintenance date (if applicable).
    /// </summary>
    public DateTimeOffset? NextMaintenanceDate { get; set; }

    /// <summary>
    /// Additional notes about the maintenance.
    /// </summary>
    public string? Notes { get; set; }
}

