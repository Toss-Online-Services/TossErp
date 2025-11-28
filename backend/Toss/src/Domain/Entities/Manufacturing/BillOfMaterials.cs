using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Entities.Catalog;

namespace Toss.Domain.Entities.Manufacturing;

/// <summary>
/// Represents a Bill of Materials (BOM) for a finished product
/// </summary>
public class BillOfMaterials : BaseAuditableEntity, IBusinessScopedEntity
{
    public BillOfMaterials()
    {
        Components = new List<BillOfMaterialsComponent>();
        IsActive = true;
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the finished product ID
    /// </summary>
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Gets or sets whether this BOM is active (only one active BOM per product)
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the BOM version/revision number
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets optional notes about this BOM
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the collection of components required for this BOM
    /// </summary>
    public ICollection<BillOfMaterialsComponent> Components { get; private set; }
}

