using TossErp.AssetManagement.Domain.SeedWork;
using TossErp.AssetManagement.Domain.ValueObjects;
using TossErp.AssetManagement.Domain.Enums;

namespace TossErp.AssetManagement.Domain.Aggregates.AssetAggregate.Events;

/// <summary>
/// Asset Created Domain Event
/// </summary>
public record AssetCreatedDomainEvent(
    Guid Id,
    Guid AssetId,
    AssetTag AssetTag,
    string AssetName,
    AssetCategory Category,
    Guid TenantId,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Asset Status Changed Domain Event
/// </summary>
public record AssetStatusChangedDomainEvent(
    Guid Id,
    Guid AssetId,
    AssetStatus PreviousStatus,
    AssetStatus NewStatus,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Asset Assigned Domain Event
/// </summary>
public record AssetAssignedDomainEvent(
    Guid Id,
    Guid AssetId,
    Guid AssignedToUserId,
    Guid? PreviousUserId,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Asset Returned Domain Event
/// </summary>
public record AssetReturnedDomainEvent(
    Guid Id,
    Guid AssetId,
    Guid ReturnedFromUserId,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Asset Location Changed Domain Event
/// </summary>
public record AssetLocationChangedDomainEvent(
    Guid Id,
    Guid AssetId,
    AssetLocation PreviousLocation,
    AssetLocation NewLocation,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Asset Condition Changed Domain Event
/// </summary>
public record AssetConditionChangedDomainEvent(
    Guid Id,
    Guid AssetId,
    AssetCondition PreviousCondition,
    AssetCondition NewCondition,
    string? Notes,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Asset Disposed Domain Event
/// </summary>
public record AssetDisposedDomainEvent(
    Guid Id,
    Guid AssetId,
    string Reason,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Asset Maintenance Scheduled Domain Event
/// </summary>
public record AssetMaintenanceScheduledDomainEvent(
    Guid Id,
    Guid AssetId,
    Guid MaintenanceId,
    MaintenanceType MaintenanceType,
    DateTime ScheduledDate,
    MaintenancePriority Priority,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Asset Maintenance Completed Domain Event
/// </summary>
public record AssetMaintenanceCompletedDomainEvent(
    Guid Id,
    Guid AssetId,
    Guid MaintenanceId,
    decimal Cost,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Asset Document Added Domain Event
/// </summary>
public record AssetDocumentAddedDomainEvent(
    Guid Id,
    Guid AssetId,
    Guid DocumentId,
    string DocumentType,
    string FileName,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Asset Financial Info Updated Domain Event
/// </summary>
public record AssetFinancialInfoUpdatedDomainEvent(
    Guid Id,
    Guid AssetId,
    AssetFinancialInfo PreviousInfo,
    AssetFinancialInfo NewInfo,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Asset Warranty Expiring Domain Event (for automated notifications)
/// </summary>
public record AssetWarrantyExpiringDomainEvent(
    Guid Id,
    Guid AssetId,
    AssetTag AssetTag,
    string AssetName,
    DateTime ExpiryDate,
    int DaysUntilExpiry,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Asset Maintenance Overdue Domain Event (for automated notifications)
/// </summary>
public record AssetMaintenanceOverdueDomainEvent(
    Guid Id,
    Guid AssetId,
    Guid MaintenanceId,
    AssetTag AssetTag,
    string AssetName,
    MaintenanceType MaintenanceType,
    DateTime ScheduledDate,
    int DaysOverdue,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Asset Allocation Overdue Domain Event
/// </summary>
public record AssetAllocationOverdueDomainEvent(
    Guid Id,
    Guid AssetId,
    Guid AllocationId,
    Guid AllocatedToUserId,
    AssetTag AssetTag,
    string AssetName,
    DateTime ExpectedReturnDate,
    int DaysOverdue,
    DateTime OccurredOn) : IDomainEvent;

/// <summary>
/// Asset Depreciation Calculated Domain Event
/// </summary>
public record AssetDepreciationCalculatedDomainEvent(
    Guid Id,
    Guid AssetId,
    decimal PreviousValue,
    decimal NewValue,
    decimal DepreciationAmount,
    DepreciationMethod Method,
    DateTime OccurredOn) : IDomainEvent;
