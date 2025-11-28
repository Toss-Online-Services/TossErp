using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Entities.Catalog;

namespace Toss.Domain.Entities.Manufacturing;

/// <summary>
/// Represents a component item in a Bill of Materials
/// </summary>
public class BillOfMaterialsComponent : BaseAuditableEntity, IBusinessScopedEntity
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the BOM ID
    /// </summary>
    public int BomId { get; set; }
    public BillOfMaterials Bom { get; set; } = null!;

    /// <summary>
    /// Gets or sets the component product ID (raw material)
    /// </summary>
    public int ComponentProductId { get; set; }
    public Product ComponentProduct { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quantity required per unit of finished product
    /// </summary>
    public decimal QuantityPer { get; set; }

    /// <summary>
    /// Gets or sets the unit of measure (should match ComponentProduct.Unit)
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Gets or sets the scrap/waste percentage (0-100)
    /// </summary>
    public decimal ScrapPercent { get; set; }

    /// <summary>
    /// Gets the effective quantity including scrap: QuantityPer * (1 + ScrapPercent / 100)
    /// </summary>
    public decimal EffectiveQuantity => QuantityPer * (1 + ScrapPercent / 100);
}

