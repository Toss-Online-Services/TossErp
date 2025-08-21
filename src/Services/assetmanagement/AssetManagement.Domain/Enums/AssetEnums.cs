namespace TossErp.AssetManagement.Domain.Enums;

/// <summary>
/// Asset status in its lifecycle
/// </summary>
public enum AssetStatus
{
    Draft = 0,
    Available = 1,
    InUse = 2,
    UnderMaintenance = 3,
    OutOfOrder = 4,
    Disposed = 5,
    Lost = 6,
    Stolen = 7
}

/// <summary>
/// Asset condition assessment
/// </summary>
public enum AssetCondition
{
    New = 1,
    Excellent = 2,
    Good = 3,
    Fair = 4,
    Poor = 5,
    Damaged = 6,
    BeyondRepair = 7
}

/// <summary>
/// Asset category classification
/// </summary>
public enum AssetCategory
{
    ITEquipment = 1,
    Furniture = 2,
    Machinery = 3,
    Vehicle = 4,
    Building = 5,
    Land = 6,
    Software = 7,
    Intellectual = 8,
    Financial = 9,
    Other = 10
}

/// <summary>
/// Asset ownership type
/// </summary>
public enum AssetOwnership
{
    Owned = 1,
    Leased = 2,
    Rented = 3,
    Borrowed = 4,
    Consignment = 5
}

/// <summary>
/// Depreciation calculation method
/// </summary>
public enum DepreciationMethod
{
    StraightLine = 1,
    DecliningBalance = 2,
    DoubleDecliningBalance = 3,
    SumOfYearsDigits = 4,
    UnitsOfProduction = 5,
    NoDepreciation = 6
}

/// <summary>
/// Maintenance type classification
/// </summary>
public enum MaintenanceType
{
    Preventive = 1,
    Corrective = 2,
    Emergency = 3,
    Predictive = 4,
    Routine = 5,
    Inspection = 6,
    Calibration = 7,
    Overhaul = 8
}

/// <summary>
/// Maintenance priority level
/// </summary>
public enum MaintenancePriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4,
    Emergency = 5
}

/// <summary>
/// Maintenance work order status
/// </summary>
public enum MaintenanceStatus
{
    Draft = 0,
    Scheduled = 1,
    InProgress = 2,
    OnHold = 3,
    Completed = 4,
    Cancelled = 5,
    Rejected = 6
}

/// <summary>
/// Asset location type
/// </summary>
public enum LocationType
{
    Building = 1,
    Room = 2,
    Floor = 3,
    Department = 4,
    Warehouse = 5,
    ExternalSite = 6,
    Vehicle = 7,
    Storage = 8,
    InTransit = 9
}

/// <summary>
/// Asset movement type
/// </summary>
public enum AssetMovementType
{
    Assignment = 1,
    Transfer = 2,
    Return = 3,
    Checkout = 4,
    Checkin = 5,
    Deployment = 6,
    Recall = 7,
    Disposal = 8
}

/// <summary>
/// Asset allocation status
/// </summary>
public enum AllocationStatus
{
    Available = 1,
    Allocated = 2,
    Reserved = 3,
    CheckedOut = 4,
    InTransit = 5,
    ReturnPending = 6
}
