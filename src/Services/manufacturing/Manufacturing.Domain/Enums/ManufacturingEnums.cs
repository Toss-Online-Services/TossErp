namespace Manufacturing.Domain.Enums;

public enum BomType
{
    Standard,
    Variant,
    Template,
    Service
}

public enum BomStatus
{
    Draft,
    Submitted,
    Cancelled
}

public enum WorkOrderStatus
{
    Draft,
    NotStarted,
    InProgress,
    Stopped,
    Completed,
    Cancelled
}

public enum WorkOrderPriority
{
    Low,
    Medium,
    High,
    Urgent
}

public enum OperationStatus
{
    Pending,
    InProgress,
    Completed,
    Skipped
}

public enum ProductionPlanStatus
{
    Draft,
    Submitted,
    InProduction,
    MaterialRequested,
    Completed,
    Cancelled
}

public enum WorkstationType
{
    Manual,
    SemiAutomatic,
    Automatic,
    Assembly,
    Quality
}
