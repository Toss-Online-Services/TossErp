using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Stores;

namespace Toss.Domain.Entities.Manufacturing;

/// <summary>
/// Represents production of finished goods for a production order
/// </summary>
public class ProductionOrderProduction : BaseAuditableEntity, IBusinessScopedEntity
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the production order ID
    /// </summary>
    public int ProductionOrderId { get; set; }
    public ProductionOrder ProductionOrder { get; set; } = null!;

    /// <summary>
    /// Gets or sets the finished product ID produced
    /// </summary>
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Gets or sets the shop/location where production occurred
    /// </summary>
    public int ShopId { get; set; }
    public Store Shop { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quantity produced
    /// </summary>
    public int Quantity { get; set; }
}

