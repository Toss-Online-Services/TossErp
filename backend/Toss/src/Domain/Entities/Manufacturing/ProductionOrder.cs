using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Manufacturing;

/// <summary>
/// Represents a production order for manufacturing finished goods
/// </summary>
public class ProductionOrder : BaseAuditableEntity, IBusinessScopedEntity
{
    public ProductionOrder()
    {
        Consumed = new List<ProductionOrderConsumption>();
        Produced = new List<ProductionOrderProduction>();
    }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the finished product ID to produce
    /// </summary>
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Gets or sets the shop/location where production occurs
    /// </summary>
    public int ShopId { get; set; }
    public Store Shop { get; set; } = null!;

    /// <summary>
    /// Gets or sets the planned quantity to produce
    /// </summary>
    public int PlannedQty { get; set; }

    /// <summary>
    /// Gets or sets the production order status
    /// </summary>
    public ProductionOrderStatus Status { get; set; } = ProductionOrderStatus.Draft;

    /// <summary>
    /// Gets or sets when production started
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// Gets or sets when production was completed
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the collection of consumed raw materials
    /// </summary>
    public ICollection<ProductionOrderConsumption> Consumed { get; private set; }

    /// <summary>
    /// Gets or sets the collection of produced finished goods
    /// </summary>
    public ICollection<ProductionOrderProduction> Produced { get; private set; }
}

