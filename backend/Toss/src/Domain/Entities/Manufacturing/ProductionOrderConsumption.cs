using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Stores;

namespace Toss.Domain.Entities.Manufacturing;

/// <summary>
/// Represents consumption of raw materials for a production order
/// </summary>
public class ProductionOrderConsumption : BaseAuditableEntity, IBusinessScopedEntity
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the production order ID
    /// </summary>
    public int ProductionOrderId { get; set; }
    public ProductionOrder ProductionOrder { get; set; } = null!;

    /// <summary>
    /// Gets or sets the component product ID (raw material consumed)
    /// </summary>
    public int ComponentProductId { get; set; }
    public Product ComponentProduct { get; set; } = null!;

    /// <summary>
    /// Gets or sets the shop/location where consumption occurred
    /// </summary>
    public int ShopId { get; set; }
    public Store Shop { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quantity consumed
    /// </summary>
    public decimal Quantity { get; set; }
}

