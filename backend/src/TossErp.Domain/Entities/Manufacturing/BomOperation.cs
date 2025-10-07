using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Manufacturing;

/// <summary>
/// Manufacturing operation/step in the production process
/// </summary>
public class BomOperation : BaseEntity
{
    public int BomId { get; set; }
    public int Sequence { get; set; }
    
    // Operation Details
    public string OperationCode { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public OperationType Type { get; set; }
    
    // Work Center / Resource
    public int? WorkCenterId { get; set; }
    public string? WorkCenterName { get; set; }
    public int? MachineId { get; set; }
    public string? MachineName { get; set; }
    
    // Timing
    public decimal SetupTime { get; set; } // in hours
    public decimal RunTimePerUnit { get; set; } // in hours per unit
    public decimal? WaitTime { get; set; }
    public decimal? MoveTime { get; set; }
    public decimal TotalTime { get; set; } // Total estimated time
    
    // Costing
    public decimal LaborRate { get; set; } // per hour
    public decimal LaborCost { get; set; }
    public decimal MachineRate { get; set; } // per hour
    public decimal MachineCost { get; set; }
    public decimal OverheadRate { get; set; } // per hour
    public decimal OverheadCost { get; set; }
    public decimal TotalCost => LaborCost + MachineCost + OverheadCost;
    
    // Dependencies
    public int? PrecedingOperationId { get; set; }
    public bool IsParallel { get; set; } // Can run parallel with previous operation
    
    // Quality
    public bool RequiresQualityCheck { get; set; }
    public string? QualityCheckPoints { get; set; } // JSON or comma-separated
    
    // Instructions
    public string? WorkInstructions { get; set; }
    public string? SafetyInstructions { get; set; }
    public string? ToolsRequired { get; set; }
    public string? Notes { get; set; }
    
    // Navigation Properties
    public BillOfMaterials BillOfMaterials { get; set; } = null!;
    
    // Business Methods
    public void CalculateCosts(decimal productionQuantity)
    {
        var totalRunTime = RunTimePerUnit * productionQuantity;
        var totalOperationTime = SetupTime + totalRunTime;
        
        LaborCost = totalOperationTime * LaborRate;
        MachineCost = totalOperationTime * MachineRate;
        OverheadCost = totalOperationTime * OverheadRate;
        TotalTime = totalOperationTime;
    }
}

public enum OperationType
{
    Setup,
    Processing,
    Assembly,
    Inspection,
    Packaging,
    Testing,
    Finishing,
    Calibration,
    Other
}

