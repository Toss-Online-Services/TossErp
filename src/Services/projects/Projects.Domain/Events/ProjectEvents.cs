using MediatR;
using TossErp.Projects.Domain.Enums;
using TossErp.Projects.Domain.ValueObjects;

namespace TossErp.Projects.Domain.Events;

// Project Lifecycle Events
public record ProjectCreatedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    string ProjectName,
    ProjectType ProjectType,
    ProjectPriority Priority) : INotification;

public record ProjectUpdatedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    string NewProjectName) : INotification;

public record ProjectStartedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    DateTime StartedAt) : INotification;

public record ProjectCompletedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    DateTime CompletedAt,
    Money ActualCost,
    Money Budget) : INotification;

public record ProjectCancelledEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    string Reason) : INotification;

public record ProjectPausedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    string Reason) : INotification;

public record ProjectResumedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode) : INotification;

public record ProjectMovedToPlanningEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode) : INotification;

// Project Management Events
public record ProjectTimelineUpdatedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    DateRange NewTimeline) : INotification;

public record ProjectBudgetUpdatedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Money NewBudget) : INotification;

public record ProjectManagerChangedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid OldManagerId,
    Guid NewManagerId,
    string NewManagerName) : INotification;

public record ProjectProgressUpdatedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Progress NewProgress) : INotification;

public record ProjectExpenseRecordedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Money ExpenseAmount,
    string Description) : INotification;

public record ProjectBudgetThresholdExceededEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    decimal CurrentUtilization,
    decimal ThresholdPercentage) : INotification;

// Task Events
public record TaskCreatedEvent(
    Guid TaskId,
    Guid ProjectId,
    string TenantId,
    string TaskNumber,
    string Title,
    TaskPriority Priority,
    TaskType TaskType) : INotification;

public record TaskAssignedEvent(
    Guid TaskId,
    Guid ProjectId,
    string TenantId,
    Guid AssigneeId,
    string AssigneeName) : INotification;

public record TaskStatusChangedEvent(
    Guid TaskId,
    Guid ProjectId,
    string TenantId,
    TaskStatus OldStatus,
    TaskStatus NewStatus) : INotification;

public record TaskCompletedEvent(
    Guid TaskId,
    Guid ProjectId,
    string TenantId,
    string TaskTitle,
    DateTime CompletedAt) : INotification;

public record TaskProgressUpdatedEvent(
    Guid TaskId,
    Guid ProjectId,
    string TenantId,
    Progress NewProgress) : INotification;

public record TaskOverdueEvent(
    Guid TaskId,
    Guid ProjectId,
    string TenantId,
    string TaskTitle,
    DateTime DueDate) : INotification;

public record TaskAddedToProjectEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid TaskId,
    string TaskTitle) : INotification;

public record TaskRemovedFromProjectEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid TaskId) : INotification;

public record TaskCommentAddedEvent(
    Guid TaskId,
    Guid ProjectId,
    string TenantId,
    Guid CommentId,
    string AuthorName) : INotification;

// Milestone Events
public record MilestoneCreatedEvent(
    Guid MilestoneId,
    Guid ProjectId,
    string TenantId,
    string MilestoneName,
    MilestoneType Type,
    DateTime PlannedDate) : INotification;

public record MilestoneCompletedEvent(
    Guid MilestoneId,
    Guid ProjectId,
    string TenantId,
    string MilestoneName,
    DateTime CompletedAt,
    bool OnTime) : INotification;

public record MilestoneDelayedEvent(
    Guid MilestoneId,
    Guid ProjectId,
    string TenantId,
    string MilestoneName,
    DateTime OriginalDate,
    DateTime NewDate) : INotification;

public record MilestoneAddedToProjectEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid MilestoneId,
    string MilestoneName) : INotification;

public record MilestoneRemovedFromProjectEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid MilestoneId) : INotification;

// Resource Events
public record ResourceAllocatedToProjectEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid ResourceId,
    string ResourceName,
    ResourceType ResourceType) : INotification;

public record ResourceDeallocatedFromProjectEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid ResourceId,
    string ResourceName) : INotification;

public record ResourceAllocationUpdatedEvent(
    Guid AllocationId,
    Guid ProjectId,
    string TenantId,
    Guid ResourceId,
    decimal OldPercentage,
    decimal NewPercentage) : INotification;

public record ResourceOverallocatedEvent(
    Guid ResourceId,
    string TenantId,
    string ResourceName,
    decimal TotalAllocation,
    List<Guid> ProjectIds) : INotification;

// Time Tracking Events
public record TimeLoggedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid UserId,
    Duration Duration,
    bool IsBillable) : INotification;

public record TimeEntrySubmittedEvent(
    Guid TimeEntryId,
    Guid ProjectId,
    string TenantId,
    Guid UserId,
    Duration Duration,
    DateTime Date) : INotification;

public record TimeEntryApprovedEvent(
    Guid TimeEntryId,
    Guid ProjectId,
    string TenantId,
    Guid UserId,
    Duration Duration,
    string ApprovedBy) : INotification;

public record TimeEntryRejectedEvent(
    Guid TimeEntryId,
    Guid ProjectId,
    string TenantId,
    Guid UserId,
    string RejectedBy,
    string Reason) : INotification;

public record OvertimeLoggedEvent(
    Guid ProjectId,
    string TenantId,
    Guid UserId,
    Duration OvertimeDuration,
    DateTime Date) : INotification;

// Risk Management Events
public record RiskIdentifiedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid RiskId,
    string RiskTitle,
    string RiskLevel) : INotification;

public record RiskMitigationPlanCreatedEvent(
    Guid RiskId,
    Guid ProjectId,
    string TenantId,
    string RiskTitle,
    string MitigationPlan) : INotification;

public record RiskStatusChangedEvent(
    Guid RiskId,
    Guid ProjectId,
    string TenantId,
    RiskStatus OldStatus,
    RiskStatus NewStatus) : INotification;

public record RiskResolvedEvent(
    Guid RiskId,
    Guid ProjectId,
    string TenantId,
    string RiskTitle,
    DateTime ResolvedAt) : INotification;

public record HighRiskIdentifiedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid RiskId,
    string RiskTitle,
    string RiskLevel) : INotification;

// Issue Management Events
public record IssueCreatedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid IssueId,
    string IssueTitle,
    IssueSeverity Severity) : INotification;

public record IssueAssignedEvent(
    Guid IssueId,
    Guid ProjectId,
    string TenantId,
    Guid AssigneeId,
    string AssigneeName) : INotification;

public record IssueResolvedEvent(
    Guid IssueId,
    Guid ProjectId,
    string TenantId,
    string IssueTitle,
    string Resolution,
    DateTime ResolvedAt) : INotification;

public record IssueSeverityChangedEvent(
    Guid IssueId,
    Guid ProjectId,
    string TenantId,
    IssueSeverity OldSeverity,
    IssueSeverity NewSeverity) : INotification;

public record CriticalIssueCreatedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid IssueId,
    string IssueTitle) : INotification;

public record IssueReopenedEvent(
    Guid IssueId,
    Guid ProjectId,
    string TenantId,
    string IssueTitle,
    string Reason) : INotification;

// Communication Events
public record ProjectCommunicationSentEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    CommunicationType Type,
    string Subject,
    List<string> Recipients) : INotification;

public record ProjectMeetingScheduledEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    DateTime MeetingDate,
    string Subject,
    List<Guid> Attendees) : INotification;

public record ProjectStatusReportGeneratedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    DateTime ReportDate,
    Progress OverallProgress) : INotification;

// Budget and Financial Events
public record ProjectBudgetExceededEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Money Budget,
    Money ActualCost,
    decimal OverrunPercentage) : INotification;

public record ProjectInvoiceGeneratedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid InvoiceId,
    Money Amount,
    BillingType BillingType) : INotification;

public record ProjectPaymentReceivedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Money PaymentAmount,
    DateTime PaymentDate) : INotification;

// Quality Assurance Events
public record QualityCheckScheduledEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    DateTime ScheduledDate,
    string CheckType) : INotification;

public record QualityCheckCompletedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    QualityStatus Result,
    DateTime CompletedDate) : INotification;

public record DeliverableApprovedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    string DeliverableName,
    string ApprovedBy,
    DateTime ApprovedDate) : INotification;

public record DeliverableRejectedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    string DeliverableName,
    string RejectedBy,
    string Reason) : INotification;

// Team and Collaboration Events
public record TeamMemberAddedToProjectEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid UserId,
    string UserName,
    ProjectRole Role) : INotification;

public record TeamMemberRemovedFromProjectEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid UserId,
    string UserName) : INotification;

public record ProjectRoleChangedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid UserId,
    ProjectRole OldRole,
    ProjectRole NewRole) : INotification;

// Document and File Events
public record ProjectDocumentUploadedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    string DocumentName,
    DocumentType DocumentType,
    string UploadedBy) : INotification;

public record ProjectDocumentApprovedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    string DocumentName,
    string ApprovedBy) : INotification;

public record ProjectDocumentVersionUpdatedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    string DocumentName,
    string Version,
    string UpdatedBy) : INotification;

// Template and Standard Events
public record ProjectCreatedFromTemplateEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid TemplateId,
    string TemplateName) : INotification;

public record ProjectTemplateCreatedEvent(
    Guid TemplateId,
    string TenantId,
    string TemplateName,
    TemplateCategory Category,
    Guid SourceProjectId) : INotification;

// Notification Events
public record ProjectNotificationSentEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    NotificationType Type,
    List<Guid> Recipients,
    string Message) : INotification;

public record ProjectDeadlineApproachingEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    DateTime Deadline,
    int DaysRemaining) : INotification;

public record ProjectMilestoneApproachingEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Guid MilestoneId,
    string MilestoneName,
    DateTime PlannedDate,
    int DaysRemaining) : INotification;

// Reporting Events
public record ProjectReportGeneratedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    string ReportType,
    DateTime GeneratedAt,
    string GeneratedBy) : INotification;

public record ProjectKPICalculatedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Dictionary<string, decimal> KPIs,
    DateTime CalculatedAt) : INotification;

public record ProjectDashboardUpdatedEvent(
    Guid ProjectId,
    string TenantId,
    string ProjectCode,
    Progress OverallProgress,
    decimal BudgetUtilization,
    int OpenIssues,
    DateTime UpdatedAt) : INotification;
