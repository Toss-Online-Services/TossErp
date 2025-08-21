namespace TossErp.Projects.Domain.Enums;

/// <summary>
/// Project status enumeration
/// </summary>
public enum ProjectStatus
{
    Draft,
    Planning,
    Active,
    OnHold,
    Completed,
    Cancelled
}

/// <summary>
/// Project priority levels
/// </summary>
public enum ProjectPriority
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Project types for categorization
/// </summary>
public enum ProjectType
{
    Internal,
    External,
    Product,
    Service,
    Research,
    Maintenance,
    Support,
    Marketing,
    Training
}

/// <summary>
/// Task status enumeration
/// </summary>
public enum TaskStatus
{
    NotStarted,
    InProgress,
    OnHold,
    Completed,
    Cancelled,
    Blocked
}

/// <summary>
/// Task priority levels
/// </summary>
public enum TaskPriority
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Task types for categorization
/// </summary>
public enum TaskType
{
    Development,
    Testing,
    Documentation,
    Review,
    Meeting,
    Research,
    Deployment,
    Bug,
    Feature,
    Maintenance
}

/// <summary>
/// Time tracking entry types
/// </summary>
public enum TimeEntryType
{
    Work,
    Meeting,
    Travel,
    Training,
    Break,
    Overtime,
    Vacation,
    Sick
}

/// <summary>
/// Time tracking status
/// </summary>
public enum TimeEntryStatus
{
    Draft,
    Submitted,
    Approved,
    Rejected,
    Billed
}

/// <summary>
/// Project milestone status
/// </summary>
public enum MilestoneStatus
{
    Planned,
    InProgress,
    Completed,
    Delayed,
    Cancelled
}

/// <summary>
/// Milestone types
/// </summary>
public enum MilestoneType
{
    Planning,
    Development,
    Testing,
    Deployment,
    Review,
    Delivery,
    Payment,
    Approval
}

/// <summary>
/// Resource allocation status
/// </summary>
public enum AllocationStatus
{
    Planned,
    Confirmed,
    Active,
    Completed,
    Cancelled
}

/// <summary>
/// Resource types for project allocation
/// </summary>
public enum ResourceType
{
    Human,
    Equipment,
    Software,
    Material,
    Budget,
    Facility
}

/// <summary>
/// Project budget status
/// </summary>
public enum BudgetStatus
{
    Draft,
    Approved,
    Active,
    Exceeded,
    Completed
}

/// <summary>
/// Expense categories for project costs
/// </summary>
public enum ExpenseCategory
{
    Labor,
    Materials,
    Equipment,
    Travel,
    Software,
    Training,
    Marketing,
    Legal,
    Consulting,
    Other
}

/// <summary>
/// Project communication types
/// </summary>
public enum CommunicationType
{
    Email,
    Meeting,
    Call,
    Chat,
    Document,
    Note,
    Announcement
}

/// <summary>
/// Document types for project management
/// </summary>
public enum DocumentType
{
    Requirement,
    Specification,
    Design,
    TestPlan,
    UserGuide,
    TechnicalDoc,
    Contract,
    Proposal,
    Report,
    Presentation
}

/// <summary>
/// Issue severity levels
/// </summary>
public enum IssueSeverity
{
    Low,
    Medium,
    High,
    Critical,
    Blocker
}

/// <summary>
/// Issue types
/// </summary>
public enum IssueType
{
    Bug,
    Risk,
    Change,
    Quality,
    Schedule,
    Budget,
    Resource,
    Technical,
    Business
}

/// <summary>
/// Issue status
/// </summary>
public enum IssueStatus
{
    Open,
    InProgress,
    Resolved,
    Closed,
    Reopened
}

/// <summary>
/// Risk probability levels
/// </summary>
public enum RiskProbability
{
    VeryLow,
    Low,
    Medium,
    High,
    VeryHigh
}

/// <summary>
/// Risk impact levels
/// </summary>
public enum RiskImpact
{
    VeryLow,
    Low,
    Medium,
    High,
    VeryHigh
}

/// <summary>
/// Risk status
/// </summary>
public enum RiskStatus
{
    Identified,
    Analyzing,
    PlanningResponse,
    Monitoring,
    Closed
}

/// <summary>
/// Project phase types
/// </summary>
public enum ProjectPhase
{
    Initiation,
    Planning,
    Execution,
    Monitoring,
    Closure
}

/// <summary>
/// Quality assurance status
/// </summary>
public enum QualityStatus
{
    NotStarted,
    InProgress,
    Passed,
    Failed,
    RequiresReview
}

/// <summary>
/// Approval status for project deliverables
/// </summary>
public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected,
    RequiresChanges,
    Cancelled
}

/// <summary>
/// Project template categories
/// </summary>
public enum TemplateCategory
{
    Software,
    Marketing,
    Construction,
    Research,
    Event,
    Product,
    Service,
    Training,
    General
}

/// <summary>
/// Billing types for project work
/// </summary>
public enum BillingType
{
    FixedPrice,
    TimeAndMaterial,
    Hourly,
    Monthly,
    Milestone,
    NonBillable
}

/// <summary>
/// Client types for external projects
/// </summary>
public enum ClientType
{
    Corporate,
    Government,
    NonProfit,
    Individual,
    Partner,
    Internal
}

/// <summary>
/// Project delivery methods
/// </summary>
public enum DeliveryMethod
{
    Waterfall,
    Agile,
    Scrum,
    Kanban,
    Hybrid,
    Custom
}

/// <summary>
/// Team member roles in projects
/// </summary>
public enum ProjectRole
{
    ProjectManager,
    TeamLead,
    Developer,
    Designer,
    Analyst,
    Tester,
    Architect,
    Consultant,
    Stakeholder,
    Sponsor,
    Client,
    Vendor
}

/// <summary>
/// Notification types for project events
/// </summary>
public enum NotificationType
{
    TaskAssignment,
    TaskCompletion,
    MilestoneReached,
    DeadlineApproaching,
    IssueCreated,
    BudgetAlert,
    StatusChange,
    Approval,
    Comment,
    Document
}
