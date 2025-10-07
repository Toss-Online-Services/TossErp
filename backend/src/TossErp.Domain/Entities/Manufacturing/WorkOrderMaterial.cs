using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Manufacturing;

/// <summary>
/// Materials required and consumed for a work order
/// </summary>
public class WorkOrderMaterial : BaseEntity
{
    public int WorkOrderId { get; set; }
    public int Sequence { get; set; }
    
    // Component Details
    public int ComponentId { get; set; }
    public string ComponentName { get; set; } = string.Empty;
    public string? ComponentSku { get; set; }
    
    // Quantities
    public decimal QuantityRequired { get; set; }
    public decimal QuantityIssued { get; set; }
    public decimal QuantityConsumed { get; set; }
    public decimal QuantityReturned { get; set; }
    public decimal QuantityScrapped { get; set; }
    public string? UnitOfMeasure { get; set; }
    
    // Location
    public int? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? BinLocation { get; set; }
    
    // Costing
    public decimal UnitCost { get; set; }
    public decimal TotalCost => QuantityConsumed * UnitCost;
    
    // Status
    public MaterialStatus Status { get; set; } = MaterialStatus.Required;
    public DateTime? IssuedDate { get; set; }
    public int? IssuedBy { get; set; }
    public string? IssuedByName { get; set; }
    
    // Batch/Serial tracking
    public string? BatchNumber { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    
    // Reference
    public int? BomItemId { get; set; }
    public string? Notes { get; set; }
    
    // Navigation Properties
    public WorkOrder WorkOrder { get; set; } = null!;
}

public enum MaterialStatus
{
    Required,
    Reserved,
    Issued,
    Consumed,
    Returned
}

