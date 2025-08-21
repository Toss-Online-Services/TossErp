using TossErp.AssetManagement.Domain.SeedWork;
using TossErp.AssetManagement.Domain.ValueObjects;
using TossErp.AssetManagement.Domain.Enums;

namespace TossErp.AssetManagement.Domain.Aggregates.AssetAggregate.Entities;

/// <summary>
/// Asset Movement - Tracks asset location and assignment changes
/// </summary>
public class AssetMovement : Entity
{
    public AssetMovementType MovementType { get; private set; }
    public DateTime MovementDate { get; private set; }
    public AssetLocation FromLocation { get; private set; } = null!;
    public AssetLocation ToLocation { get; private set; } = null!;
    public Guid? FromUserId { get; private set; }
    public Guid? ToUserId { get; private set; }
    public string? Reason { get; private set; }
    public string? Notes { get; private set; }

    protected AssetMovement() { } // For EF Core

    public AssetMovement(
        AssetMovementType movementType,
        DateTime movementDate,
        AssetLocation fromLocation,
        AssetLocation toLocation,
        Guid? fromUserId = null,
        Guid? toUserId = null,
        string? reason = null)
    {
        Id = Guid.NewGuid();
        MovementType = movementType;
        MovementDate = movementDate;
        FromLocation = fromLocation;
        ToLocation = toLocation;
        FromUserId = fromUserId;
        ToUserId = toUserId;
        Reason = reason?.Trim();
    }

    public void AddNotes(string notes)
    {
        Notes = notes?.Trim();
        MarkAsModified();
    }
}

/// <summary>
/// Asset Maintenance - Tracks maintenance activities and schedules
/// </summary>
public class AssetMaintenance : Entity
{
    public MaintenanceType MaintenanceType { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public MaintenancePriority Priority { get; private set; }
    public MaintenanceStatus Status { get; private set; }
    public decimal? Cost { get; private set; }
    public string? TechnicianNotes { get; private set; }
    public string? CompletionNotes { get; private set; }
    public Guid? TechnicianId { get; private set; }

    protected AssetMaintenance() { } // For EF Core

    public AssetMaintenance(
        MaintenanceType maintenanceType,
        DateTime scheduledDate,
        string description,
        MaintenancePriority priority = MaintenancePriority.Medium)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        Id = Guid.NewGuid();
        MaintenanceType = maintenanceType;
        ScheduledDate = scheduledDate;
        Description = description.Trim();
        Priority = priority;
        Status = MaintenanceStatus.Scheduled;
    }

    public void StartMaintenance(Guid technicianId, string? notes = null)
    {
        if (Status != MaintenanceStatus.Scheduled)
            throw new InvalidOperationException("Can only start scheduled maintenance");

        Status = MaintenanceStatus.InProgress;
        TechnicianId = technicianId;
        TechnicianNotes = notes?.Trim();
        MarkAsModified();
    }

    public void Complete(decimal cost, string? completionNotes = null)
    {
        if (Status != MaintenanceStatus.InProgress && Status != MaintenanceStatus.Scheduled)
            throw new InvalidOperationException("Can only complete in-progress or scheduled maintenance");

        Status = MaintenanceStatus.Completed;
        CompletedDate = DateTime.UtcNow;
        Cost = cost;
        CompletionNotes = completionNotes?.Trim();
        MarkAsModified();
    }

    public void Cancel(string reason)
    {
        if (Status == MaintenanceStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed maintenance");

        Status = MaintenanceStatus.Cancelled;
        CompletionNotes = reason?.Trim();
        MarkAsModified();
    }

    public void Reschedule(DateTime newScheduledDate, string? reason = null)
    {
        if (Status != MaintenanceStatus.Scheduled)
            throw new InvalidOperationException("Can only reschedule scheduled maintenance");

        ScheduledDate = newScheduledDate;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            TechnicianNotes = (TechnicianNotes + Environment.NewLine + $"Rescheduled: {reason}").Trim();
        }
        MarkAsModified();
    }

    public bool IsOverdue => Status == MaintenanceStatus.Scheduled && ScheduledDate < DateTime.UtcNow;
    public TimeSpan? TimeSinceCompletion => CompletedDate.HasValue ? DateTime.UtcNow - CompletedDate.Value : null;
}

/// <summary>
/// Asset Document - Manages documents related to assets
/// </summary>
public class AssetDocument : Entity
{
    public string DocumentType { get; private set; } = string.Empty;
    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime UploadDate { get; private set; }
    public Guid? UploadedByUserId { get; private set; }
    public long? FileSizeBytes { get; private set; }
    public string? ContentType { get; private set; }
    public bool IsActive { get; private set; }

    protected AssetDocument() { } // For EF Core

    public AssetDocument(
        string documentType,
        string fileName,
        string filePath,
        string? description = null,
        Guid? uploadedByUserId = null,
        long? fileSizeBytes = null,
        string? contentType = null)
    {
        if (string.IsNullOrWhiteSpace(documentType))
            throw new ArgumentException("Document type cannot be empty", nameof(documentType));
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty", nameof(fileName));
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        Id = Guid.NewGuid();
        DocumentType = documentType.Trim();
        FileName = fileName.Trim();
        FilePath = filePath.Trim();
        Description = description?.Trim();
        UploadDate = DateTime.UtcNow;
        UploadedByUserId = uploadedByUserId;
        FileSizeBytes = fileSizeBytes;
        ContentType = contentType?.Trim();
        IsActive = true;
    }

    public void UpdateDescription(string description)
    {
        Description = description?.Trim();
        MarkAsModified();
    }

    public void Archive()
    {
        IsActive = false;
        MarkAsModified();
    }

    public void Restore()
    {
        IsActive = true;
        MarkAsModified();
    }

    public void UpdateFilePath(string newFilePath)
    {
        if (string.IsNullOrWhiteSpace(newFilePath))
            throw new ArgumentException("File path cannot be empty", nameof(newFilePath));

        FilePath = newFilePath.Trim();
        MarkAsModified();
    }
}

/// <summary>
/// Asset Depreciation Record - Tracks depreciation calculations over time
/// </summary>
public class AssetDepreciationRecord : Entity
{
    public DateTime CalculationDate { get; private set; }
    public decimal BookValue { get; private set; }
    public decimal DepreciationAmount { get; private set; }
    public decimal AccumulatedDepreciation { get; private set; }
    public DepreciationMethod Method { get; private set; }
    public string? Notes { get; private set; }
    public bool IsAdjustment { get; private set; }

    protected AssetDepreciationRecord() { } // For EF Core

    public AssetDepreciationRecord(
        DateTime calculationDate,
        decimal bookValue,
        decimal depreciationAmount,
        decimal accumulatedDepreciation,
        DepreciationMethod method,
        string? notes = null,
        bool isAdjustment = false)
    {
        Id = Guid.NewGuid();
        CalculationDate = calculationDate;
        BookValue = bookValue;
        DepreciationAmount = depreciationAmount;
        AccumulatedDepreciation = accumulatedDepreciation;
        Method = method;
        Notes = notes?.Trim();
        IsAdjustment = isAdjustment;
    }

    public void AddNotes(string notes)
    {
        if (string.IsNullOrWhiteSpace(Notes))
            Notes = notes?.Trim();
        else
            Notes = $"{Notes}{Environment.NewLine}{notes?.Trim()}";
        
        MarkAsModified();
    }
}

/// <summary>
/// Asset Allocation - Tracks asset checkout/checkin for temporary assignments
/// </summary>
public class AssetAllocation : Entity
{
    public Guid AllocatedToUserId { get; private set; }
    public DateTime AllocationDate { get; private set; }
    public DateTime? ExpectedReturnDate { get; private set; }
    public DateTime? ActualReturnDate { get; private set; }
    public AllocationStatus Status { get; private set; }
    public string? Purpose { get; private set; }
    public string? AllocationNotes { get; private set; }
    public string? ReturnNotes { get; private set; }
    public Guid? AllocatedByUserId { get; private set; }
    public Guid? ReturnedToUserId { get; private set; }

    protected AssetAllocation() { } // For EF Core

    public AssetAllocation(
        Guid allocatedToUserId,
        DateTime allocationDate,
        DateTime? expectedReturnDate = null,
        string? purpose = null,
        string? allocationNotes = null,
        Guid? allocatedByUserId = null)
    {
        Id = Guid.NewGuid();
        AllocatedToUserId = allocatedToUserId;
        AllocationDate = allocationDate;
        ExpectedReturnDate = expectedReturnDate;
        Purpose = purpose?.Trim();
        AllocationNotes = allocationNotes?.Trim();
        AllocatedByUserId = allocatedByUserId;
        Status = AllocationStatus.Allocated;
    }

    public void CheckOut()
    {
        if (Status != AllocationStatus.Allocated)
            throw new InvalidOperationException("Can only check out allocated assets");

        Status = AllocationStatus.CheckedOut;
        MarkAsModified();
    }

    public void ReturnAsset(Guid? returnedToUserId = null, string? returnNotes = null)
    {
        if (Status != AllocationStatus.CheckedOut && Status != AllocationStatus.Allocated)
            throw new InvalidOperationException("Can only return checked out or allocated assets");

        Status = AllocationStatus.Available;
        ActualReturnDate = DateTime.UtcNow;
        ReturnedToUserId = returnedToUserId;
        ReturnNotes = returnNotes?.Trim();
        MarkAsModified();
    }

    public void ExtendAllocation(DateTime newExpectedReturnDate, string? reason = null)
    {
        if (Status != AllocationStatus.Allocated && Status != AllocationStatus.CheckedOut)
            throw new InvalidOperationException("Can only extend active allocations");

        ExpectedReturnDate = newExpectedReturnDate;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            AllocationNotes = string.IsNullOrWhiteSpace(AllocationNotes) 
                ? $"Extended: {reason}" 
                : $"{AllocationNotes}{Environment.NewLine}Extended: {reason}";
        }
        MarkAsModified();
    }

    public bool IsOverdue => ExpectedReturnDate.HasValue && 
                            ExpectedReturnDate.Value < DateTime.UtcNow && 
                            !ActualReturnDate.HasValue;

    public TimeSpan? OverdueDuration => IsOverdue 
        ? DateTime.UtcNow - ExpectedReturnDate!.Value 
        : null;
}
