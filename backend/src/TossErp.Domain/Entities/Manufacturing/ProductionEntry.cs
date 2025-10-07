using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Manufacturing;

/// <summary>
/// Record of actual production output from shop floor
/// </summary>
public class ProductionEntry : BaseEntity
{
    public int WorkOrderId { get; set; }
    public string EntryNumber { get; set; } = string.Empty;
    
    // Quantities
    public decimal QuantityProduced { get; set; }
    public decimal QuantityRejected { get; set; }
    public decimal QuantityScrapped { get; set; }
    public string? UnitOfMeasure { get; set; }
    
    // Timing
    public DateTime ProductionDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal? ActualHours { get; set; }
    
    // Location & Personnel
    public int? WorkCenterId { get; set; }
    public string? WorkCenterName { get; set; }
    public int? OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public int? SupervisorId { get; set; }
    public string? SupervisorName { get; set; }
    
    // Quality
    public string? QualityInspectionStatus { get; set; }
    public int? InspectedBy { get; set; }
    public string? InspectedByName { get; set; }
    public DateTime? InspectedAt { get; set; }
    
    // Defects
    public string? DefectCodes { get; set; } // JSON array or comma-separated
    public string? DefectDescription { get; set; }
    public string? ReworkRequired { get; set; }
    
    // Materials Consumed (detailed tracking)
    public string? MaterialsConsumed { get; set; } // JSON data
    
    // Costing
    public decimal MaterialCost { get; set; }
    public decimal LaborCost { get; set; }
    public decimal OverheadCost { get; set; }
    public decimal TotalCost { get; set; }
    
    // Metadata
    public string? Notes { get; set; }
    public string? Comments { get; set; }
    
    // Status
    public bool IsPosted { get; set; } // Posted to inventory
    public DateTime? PostedAt { get; set; }
    public int? PostedBy { get; set; }
    public string? PostedByName { get; set; }
    
    // Navigation Properties
    public WorkOrder WorkOrder { get; set; } = null!;
}

