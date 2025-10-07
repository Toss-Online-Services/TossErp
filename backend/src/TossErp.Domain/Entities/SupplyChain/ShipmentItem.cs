using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.SupplyChain;

/// <summary>
/// Individual item in a shipment
/// </summary>
public class ShipmentItem : BaseEntity
{
    public int ShipmentId { get; set; }
    public int LineNumber { get; set; }
    
    // Product Details
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    public string? Barcode { get; set; }
    
    // Quantities
    public decimal QuantityOrdered { get; set; }
    public decimal QuantityShipped { get; set; }
    public decimal QuantityReceived { get; set; }
    public decimal QuantityDamaged { get; set; }
    public string? UnitOfMeasure { get; set; }
    
    // Packaging
    public int? PackageNumber { get; set; }
    public string? PackageType { get; set; } // Box, Pallet, Container, etc.
    public decimal? Weight { get; set; }
    public string? WeightUnit { get; set; } = "kg";
    
    // Batch/Serial
    public string? BatchNumber { get; set; }
    public string? SerialNumbers { get; set; } // JSON array or comma-separated
    public DateTime? ExpiryDate { get; set; }
    
    // Handling
    public bool IsFragile { get; set; }
    public bool RequiresRefrigeration { get; set; }
    public string? HandlingInstructions { get; set; }
    
    // Reference
    public int? PurchaseOrderItemId { get; set; }
    public int? SalesOrderItemId { get; set; }
    public string? Notes { get; set; }
    
    // Navigation Properties
    public Shipment Shipment { get; set; } = null!;
}
