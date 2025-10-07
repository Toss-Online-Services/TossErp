using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Manufacturing;

/// <summary>
/// Specific operation instance for a work order
/// </summary>
public class WorkOrderOperation : BaseEntity
{
    public int WorkOrderId { get; set; }
    public int Sequence { get; set; }
    public string OperationCode { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;
    public OperationStatus Status { get; set; } = OperationStatus.Pending;
    
    // Work Center / Resource
    public int? WorkCenterId { get; set; }
    public string? WorkCenterName { get; set; }
    public int? MachineId { get; set; }
    public string? MachineName { get; set; }
    public int? OperatorId { get; set; }
    public string? OperatorName { get; set; }
    
    // Timing
    public decimal EstimatedSetupTime { get; set; }
    public decimal EstimatedRunTime { get; set; }
    public decimal ActualSetupTime { get; set; }
    public decimal ActualRunTime { get; set; }
    
    public DateTime? ScheduledStartDate { get; set; }
    public DateTime? ScheduledEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    
    // Quantities
    public decimal QuantityCompleted { get; set; }
    public decimal QuantityRejected { get; set; }
    public decimal QuantityScrapped { get; set; }
    
    // Costing
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    
    // Quality
    public bool RequiresInspection { get; set; }
    public bool InspectionPassed { get; set; }
    public int? InspectedBy { get; set; }
    public string? InspectedByName { get; set; }
    public DateTime? InspectedAt { get; set; }
    public string? InspectionNotes { get; set; }
    
    // Instructions & Notes
    public string? WorkInstructions { get; set; }
    public string? Notes { get; set; }
    
    // Navigation Properties
    public WorkOrder WorkOrder { get; set; } = null!;
}

public enum OperationStatus
{
    Pending,
    InProgress,
    Completed,
    OnHold,
    Cancelled
}

