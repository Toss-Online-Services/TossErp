using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Stores;

namespace Toss.Domain.Entities.Projects;

/// <summary>
/// Represents material consumption for a project
/// </summary>
public class ProjectMaterial : BaseAuditableEntity, IBusinessScopedEntity
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the project ID
    /// </summary>
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    /// <summary>
    /// Gets or sets the product/item ID
    /// </summary>
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Gets or sets the shop/store ID where material is consumed
    /// </summary>
    public int ShopId { get; set; }
    public Store Shop { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quantity consumed
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit cost at time of consumption
    /// </summary>
    public decimal UnitCost { get; set; }

    /// <summary>
    /// Gets or sets the total cost (Quantity * UnitCost)
    /// </summary>
    public decimal TotalCost { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the stock movement ID created when material was consumed
    /// </summary>
    public int? StockMovementId { get; set; }
}

