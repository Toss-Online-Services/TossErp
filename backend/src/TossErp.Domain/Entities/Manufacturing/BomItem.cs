using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Manufacturing;

/// <summary>
/// Individual component/material in a Bill of Materials
/// </summary>
public class BomItem : BaseEntity
{
    public int BomId { get; set; }
    public int Sequence { get; set; } // Order of assembly
    
    // Component Details
    public int ComponentId { get; set; }
    public string ComponentName { get; set; } = string.Empty;
    public string? ComponentSku { get; set; }
    public string? Description { get; set; }
    
    // Quantity
    public decimal Quantity { get; set; }
    public string? UnitOfMeasure { get; set; }
    public decimal? WastagePercentage { get; set; } // Expected wastage
    public decimal EffectiveQuantity => Quantity * (1 + (WastagePercentage ?? 0) / 100);
    
    // Costing
    public decimal UnitCost { get; set; }
    public decimal TotalCost => EffectiveQuantity * UnitCost;
    
    // Options
    public bool IsOptional { get; set; }
    public bool IsPhantom { get; set; } // Component doesn't exist in inventory
    public int? SubstituteBomId { get; set; } // Alternative BOM if this is sub-assembly
    
    // Supply
    public ItemSupplyType SupplyType { get; set; } = ItemSupplyType.Purchase;
    public int? PreferredSupplierId { get; set; }
    public string? PreferredSupplierName { get; set; }
    public decimal? LeadTimeDays { get; set; }
    
    // Reference location
    public string? ReferenceDesignator { get; set; } // e.g., "R1", "C5" for electronics
    public string? Notes { get; set; }
    
    // Navigation Properties
    public BillOfMaterials BillOfMaterials { get; set; } = null!;
}

public enum ItemSupplyType
{
    Purchase,       // Buy from supplier
    Make,           // Manufacture internally
    Transfer,       // Transfer from another location
    Phantom         // Doesn't exist in inventory
}

