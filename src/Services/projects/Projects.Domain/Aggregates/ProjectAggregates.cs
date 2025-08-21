using TossErp.Projects.Domain.Entities;
using TossErp.Projects.Domain.Enums;
using TossErp.Projects.Domain.Events;
using TossErp.Projects.Domain.SeedWork;
using TossErp.Projects.Domain.ValueObjects;

namespace TossErp.Projects.Domain.Aggregates;

/// <summary>
/// Project aggregate root for project management
/// </summary>
public class Project : AggregateRoot
{
    public string TenantId { get; private set; }
    public ProjectCode ProjectCode { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public ProjectStatus Status { get; private set; }
    public ProjectType Type { get; private set; }
    public Priority Priority { get; private set; }
    public DateRange? Timeline { get; private set; }
    public Money Budget { get; private set; }
    public Money ActualCost { get; private set; }
    public Progress OverallProgress { get; private set; }
    public Guid ProjectManagerId { get; private set; }
    public string ProjectManagerName { get; private set; }
    public Guid? ClientId { get; private set; }
    public string? ClientName { get; private set; }
    public ClientType? ClientType { get; private set; }
    public DeliveryMethod DeliveryMethod { get; private set; }
    public BillingType BillingType { get; private set; }
    public TeamCapacity? TeamCapacity { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private readonly List<ProjectTask> _tasks;
    public IReadOnlyList<ProjectTask> Tasks => _tasks.AsReadOnly();

    private readonly List<ProjectMilestone> _milestones;
    public IReadOnlyList<ProjectMilestone> Milestones => _milestones.AsReadOnly();

    private readonly List<ResourceAllocation> _resourceAllocations;
    public IReadOnlyList<ResourceAllocation> ResourceAllocations => _resourceAllocations.AsReadOnly();

    private readonly List<TimeEntry> _timeEntries;
    public IReadOnlyList<TimeEntry> TimeEntries => _timeEntries.AsReadOnly();

    private readonly List<ProjectRisk> _risks;
    public IReadOnlyList<ProjectRisk> Risks => _risks.AsReadOnly();

    private readonly List<ProjectIssue> _issues;
    public IReadOnlyList<ProjectIssue> Issues => _issues.AsReadOnly();

    private Project()
    {
        _tasks = new List<ProjectTask>();
        _milestones = new List<ProjectMilestone>();
        _resourceAllocations = new List<ResourceAllocation>();
        _timeEntries = new List<TimeEntry>();
        _risks = new List<ProjectRisk>();
        _issues = new List<ProjectIssue>();
        TenantId = null!;
        ProjectCode = null!;
        Name = null!;
        Priority = null!;
        Budget = null!;
        ActualCost = null!;
        OverallProgress = null!;
        ProjectManagerName = null!;
        CreatedBy = null!;
    } // EF Core

    public Project(
        Guid id,
        string tenantId,
        ProjectCode projectCode,
        string name,
        ProjectType type,
        Priority priority,
        Money budget,
        Guid projectManagerId,
        string projectManagerName,
        string createdBy,
        DeliveryMethod deliveryMethod = DeliveryMethod.Agile,
        BillingType billingType = BillingType.TimeAndMaterial,
        string? description = null,
        Guid? clientId = null,
        string? clientName = null,
        ClientType? clientType = null)
    {
        Id = id;
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        ProjectCode = projectCode ?? throw new ArgumentNullException(nameof(projectCode));
        Name = name?.Trim() ?? throw new ArgumentException("Name cannot be empty");
        Description = description?.Trim();
        Status = ProjectStatus.Draft;
        Type = type;
        Priority = priority ?? throw new ArgumentNullException(nameof(priority));
        Budget = budget ?? throw new ArgumentNullException(nameof(budget));
        ActualCost = Money.Zero(budget.Currency);
        OverallProgress = Progress.Zero;
        ProjectManagerId = projectManagerId;
        ProjectManagerName = projectManagerName?.Trim() ?? throw new ArgumentException("Project manager name cannot be empty");
        ClientId = clientId;
        ClientName = clientName?.Trim();
        ClientType = clientType;
        DeliveryMethod = deliveryMethod;
        BillingType = billingType;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        CreatedAt = DateTime.UtcNow;

        _tasks = new List<ProjectTask>();
        _milestones = new List<ProjectMilestone>();
        _resourceAllocations = new List<ResourceAllocation>();
        _timeEntries = new List<TimeEntry>();
        _risks = new List<ProjectRisk>();
        _issues = new List<ProjectIssue>();

        AddDomainEvent(new ProjectCreatedEvent(Id, TenantId, ProjectCode.Value, Name, Type, Priority.Level));
    }

    public void UpdateBasicInfo(string name, string? description, Priority priority, string modifiedBy)
    {
        var oldName = Name;
        Name = name?.Trim() ?? throw new ArgumentException("Name cannot be empty");
        Description = description?.Trim();
        Priority = priority ?? throw new ArgumentNullException(nameof(priority));
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));

        if (oldName != Name)
        {
            AddDomainEvent(new ProjectUpdatedEvent(Id, TenantId, ProjectCode.Value, Name));
        }
    }

    public void UpdateTimeline(DateRange timeline, string modifiedBy)
    {
        Timeline = timeline;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));

        AddDomainEvent(new ProjectTimelineUpdatedEvent(Id, TenantId, ProjectCode.Value, timeline));
    }

    public void UpdateBudget(Money budget, string modifiedBy)
    {
        if (budget.Currency != Budget.Currency)
            throw new InvalidOperationException($"Currency mismatch. Expected {Budget.Currency}, got {budget.Currency}");

        Budget = budget ?? throw new ArgumentNullException(nameof(budget));
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));

        AddDomainEvent(new ProjectBudgetUpdatedEvent(Id, TenantId, ProjectCode.Value, budget));
    }

    public void ChangeProjectManager(Guid projectManagerId, string projectManagerName, string modifiedBy)
    {
        var oldManagerId = ProjectManagerId;
        ProjectManagerId = projectManagerId;
        ProjectManagerName = projectManagerName?.Trim() ?? throw new ArgumentException("Project manager name cannot be empty");
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));

        AddDomainEvent(new ProjectManagerChangedEvent(Id, TenantId, ProjectCode.Value, oldManagerId, projectManagerId, projectManagerName));
    }

    public void SetTeamCapacity(TeamCapacity teamCapacity, string modifiedBy)
    {
        TeamCapacity = teamCapacity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void Start(string startedBy, DateTime? startDate = null)
    {
        if (Status != ProjectStatus.Planning)
            throw new InvalidOperationException("Only projects in planning status can be started");

        Status = ProjectStatus.Active;
        StartedAt = startDate ?? DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = startedBy ?? throw new ArgumentNullException(nameof(startedBy));

        AddDomainEvent(new ProjectStartedEvent(Id, TenantId, ProjectCode.Value, StartedAt.Value));
    }

    public void Pause(string pausedBy, string reason)
    {
        if (Status != ProjectStatus.Active)
            throw new InvalidOperationException("Only active projects can be paused");

        Status = ProjectStatus.OnHold;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = pausedBy ?? throw new ArgumentNullException(nameof(pausedBy));

        AddDomainEvent(new ProjectPausedEvent(Id, TenantId, ProjectCode.Value, reason));
    }

    public void Resume(string resumedBy)
    {
        if (Status != ProjectStatus.OnHold)
            throw new InvalidOperationException("Only paused projects can be resumed");

        Status = ProjectStatus.Active;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = resumedBy ?? throw new ArgumentNullException(nameof(resumedBy));

        AddDomainEvent(new ProjectResumedEvent(Id, TenantId, ProjectCode.Value));
    }

    public void Complete(string completedBy, DateTime? completionDate = null)
    {
        if (Status != ProjectStatus.Active)
            throw new InvalidOperationException("Only active projects can be completed");

        // Check if all critical tasks and milestones are completed
        var incompleteCriticalTasks = _tasks.Where(t => t.Priority == TaskPriority.Critical && !t.IsCompleted).ToList();
        if (incompleteCriticalTasks.Any())
            throw new InvalidOperationException($"Cannot complete project with {incompleteCriticalTasks.Count} incomplete critical tasks");

        var incompleteMilestones = _milestones.Where(m => !m.IsCompleted).ToList();
        if (incompleteMilestones.Any())
            throw new InvalidOperationException($"Cannot complete project with {incompleteMilestones.Count} incomplete milestones");

        Status = ProjectStatus.Completed;
        CompletedAt = completionDate ?? DateTime.UtcNow;
        OverallProgress = Progress.Complete;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = completedBy ?? throw new ArgumentNullException(nameof(completedBy));

        AddDomainEvent(new ProjectCompletedEvent(Id, TenantId, ProjectCode.Value, CompletedAt.Value, ActualCost, Budget));
    }

    public void Cancel(string cancelledBy, string reason)
    {
        if (Status == ProjectStatus.Completed || Status == ProjectStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel completed or already cancelled projects");

        Status = ProjectStatus.Cancelled;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = cancelledBy ?? throw new ArgumentNullException(nameof(cancelledBy));

        AddDomainEvent(new ProjectCancelledEvent(Id, TenantId, ProjectCode.Value, reason));
    }

    public void MoveTo(ProjectStatus status, string modifiedBy)
    {
        var oldStatus = Status;
        Status = status;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));

        if (status == ProjectStatus.Planning && oldStatus == ProjectStatus.Draft)
        {
            AddDomainEvent(new ProjectMovedToPlanningEvent(Id, TenantId, ProjectCode.Value));
        }
    }

    // Task Management
    public void AddTask(ProjectTask task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));
        if (task.ProjectId != Id)
            throw new InvalidOperationException("Task must belong to this project");

        _tasks.Add(task);
        RecalculateProgress();

        AddDomainEvent(new TaskAddedToProjectEvent(Id, TenantId, ProjectCode.Value, task.Id, task.Title));
    }

    public void RemoveTask(Guid taskId)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null)
        {
            _tasks.Remove(task);
            RecalculateProgress();

            AddDomainEvent(new TaskRemovedFromProjectEvent(Id, TenantId, ProjectCode.Value, taskId));
        }
    }

    // Milestone Management
    public void AddMilestone(ProjectMilestone milestone)
    {
        if (milestone == null)
            throw new ArgumentNullException(nameof(milestone));
        if (milestone.ProjectId != Id)
            throw new InvalidOperationException("Milestone must belong to this project");

        _milestones.Add(milestone);

        AddDomainEvent(new MilestoneAddedToProjectEvent(Id, TenantId, ProjectCode.Value, milestone.Id, milestone.Name));
    }

    public void RemoveMilestone(Guid milestoneId)
    {
        var milestone = _milestones.FirstOrDefault(m => m.Id == milestoneId);
        if (milestone != null)
        {
            _milestones.Remove(milestone);

            AddDomainEvent(new MilestoneRemovedFromProjectEvent(Id, TenantId, ProjectCode.Value, milestoneId));
        }
    }

    // Resource Management
    public void AllocateResource(ResourceAllocation allocation)
    {
        if (allocation == null)
            throw new ArgumentNullException(nameof(allocation));
        if (allocation.ProjectId != Id)
            throw new InvalidOperationException("Resource allocation must belong to this project");

        _resourceAllocations.Add(allocation);

        AddDomainEvent(new ResourceAllocatedToProjectEvent(Id, TenantId, ProjectCode.Value, allocation.ResourceId, allocation.ResourceName, allocation.ResourceType));
    }

    public void DeallocateResource(Guid allocationId)
    {
        var allocation = _resourceAllocations.FirstOrDefault(a => a.Id == allocationId);
        if (allocation != null)
        {
            _resourceAllocations.Remove(allocation);

            AddDomainEvent(new ResourceDeallocatedFromProjectEvent(Id, TenantId, ProjectCode.Value, allocation.ResourceId, allocation.ResourceName));
        }
    }

    // Time Tracking
    public void AddTimeEntry(TimeEntry timeEntry)
    {
        if (timeEntry == null)
            throw new ArgumentNullException(nameof(timeEntry));
        if (timeEntry.ProjectId != Id)
            throw new InvalidOperationException("Time entry must belong to this project");

        _timeEntries.Add(timeEntry);

        // Update actual cost if billable
        if (timeEntry.IsBillable && timeEntry.BillableAmount != null)
        {
            ActualCost = ActualCost.Add(timeEntry.BillableAmount);
        }

        AddDomainEvent(new TimeLoggedEvent(Id, TenantId, ProjectCode.Value, timeEntry.UserId, timeEntry.Duration, timeEntry.IsBillable));
    }

    // Risk Management
    public void AddRisk(ProjectRisk risk)
    {
        if (risk == null)
            throw new ArgumentNullException(nameof(risk));
        if (risk.ProjectId != Id)
            throw new InvalidOperationException("Risk must belong to this project");

        _risks.Add(risk);

        AddDomainEvent(new RiskIdentifiedEvent(Id, TenantId, ProjectCode.Value, risk.Id, risk.Title, risk.RiskScore.RiskLevel));
    }

    public void RemoveRisk(Guid riskId)
    {
        var risk = _risks.FirstOrDefault(r => r.Id == riskId);
        if (risk != null)
        {
            _risks.Remove(risk);
        }
    }

    // Issue Management
    public void AddIssue(ProjectIssue issue)
    {
        if (issue == null)
            throw new ArgumentNullException(nameof(issue));
        if (issue.ProjectId != Id)
            throw new InvalidOperationException("Issue must belong to this project");

        _issues.Add(issue);

        AddDomainEvent(new IssueCreatedEvent(Id, TenantId, ProjectCode.Value, issue.Id, issue.Title, issue.Severity));
    }

    public void RemoveIssue(Guid issueId)
    {
        var issue = _issues.FirstOrDefault(i => i.Id == issueId);
        if (issue != null)
        {
            _issues.Remove(issue);
        }
    }

    // Expense Tracking
    public void RecordExpense(Money amount, string description, string recordedBy)
    {
        if (amount.Currency != Budget.Currency)
            throw new InvalidOperationException($"Currency mismatch. Expected {Budget.Currency}, got {amount.Currency}");

        ActualCost = ActualCost.Add(amount);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = recordedBy ?? throw new ArgumentNullException(nameof(recordedBy));

        AddDomainEvent(new ProjectExpenseRecordedEvent(Id, TenantId, ProjectCode.Value, amount, description));

        // Check budget threshold
        var utilizationPercentage = (ActualCost.Amount / Budget.Amount) * 100;
        if (utilizationPercentage >= 90)
        {
            AddDomainEvent(new ProjectBudgetThresholdExceededEvent(Id, TenantId, ProjectCode.Value, utilizationPercentage, 90));
        }
        else if (utilizationPercentage >= 75)
        {
            AddDomainEvent(new ProjectBudgetThresholdExceededEvent(Id, TenantId, ProjectCode.Value, utilizationPercentage, 75));
        }
    }

    private void RecalculateProgress()
    {
        if (!_tasks.Any())
        {
            OverallProgress = Progress.Zero;
            return;
        }

        var completedTasks = _tasks.Count(t => t.IsCompleted);
        var totalTasks = _tasks.Count;

        OverallProgress = new Progress(completedTasks, totalTasks);

        AddDomainEvent(new ProjectProgressUpdatedEvent(Id, TenantId, ProjectCode.Value, OverallProgress));
    }

    // Calculated Properties
    public Money RemainingBudget => Budget.Subtract(ActualCost);
    public decimal BudgetUtilizationPercentage => Budget.Amount > 0 ? (ActualCost.Amount / Budget.Amount) * 100 : 0;
    public bool IsOverBudget => ActualCost.Amount > Budget.Amount;
    public bool IsActive => Status == ProjectStatus.Active;
    public bool IsCompleted => Status == ProjectStatus.Completed;
    public bool IsOnTime => Timeline?.EndDate >= DateTime.Today || IsCompleted;
    public bool IsOverdue => Timeline?.EndDate < DateTime.Today && !IsCompleted;
    public int TasksCompleted => _tasks.Count(t => t.IsCompleted);
    public int TasksInProgress => _tasks.Count(t => t.Status == TaskStatus.InProgress);
    public int TasksNotStarted => _tasks.Count(t => t.Status == TaskStatus.NotStarted);
    public int MilestonesCompleted => _milestones.Count(m => m.IsCompleted);
    public int ActiveIssues => _issues.Count(i => i.Status != IssueStatus.Closed);
    public int HighRisks => _risks.Count(r => r.RiskScore.RiskLevel == "High" || r.RiskScore.RiskLevel == "Critical");
    public Duration TotalTimeLogged => _timeEntries.Aggregate(Duration.Zero, (sum, entry) => sum.Add(entry.Duration));
    public Money TotalBillableTime => _timeEntries.Where(e => e.IsBillable && e.BillableAmount != null)
                                                 .Aggregate(Money.Zero(Budget.Currency), (sum, entry) => sum.Add(entry.BillableAmount!));
}

/// <summary>
/// Project Risk aggregate for risk management
/// </summary>
public class ProjectRisk : AggregateRoot
{
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public RiskScore RiskScore { get; private set; }
    public RiskStatus Status { get; private set; }
    public string? MitigationPlan { get; private set; }
    public string? ContingencyPlan { get; private set; }
    public Guid? OwnerId { get; private set; }
    public string? OwnerName { get; private set; }
    public DateTime IdentifiedDate { get; private set; }
    public DateTime? TargetResolutionDate { get; private set; }
    public DateTime? ActualResolutionDate { get; private set; }
    public string IdentifiedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    private ProjectRisk()
    {
        Title = null!;
        Description = null!;
        RiskScore = null!;
        IdentifiedBy = null!;
    } // EF Core

    public ProjectRisk(
        Guid id,
        Guid projectId,
        string title,
        string description,
        RiskScore riskScore,
        string identifiedBy,
        DateTime? identifiedDate = null,
        Guid? ownerId = null,
        string? ownerName = null)
    {
        Id = id;
        ProjectId = projectId;
        Title = title?.Trim() ?? throw new ArgumentException("Title cannot be empty");
        Description = description?.Trim() ?? throw new ArgumentException("Description cannot be empty");
        RiskScore = riskScore ?? throw new ArgumentNullException(nameof(riskScore));
        Status = RiskStatus.Identified;
        OwnerId = ownerId;
        OwnerName = ownerName?.Trim();
        IdentifiedDate = identifiedDate?.Date ?? DateTime.UtcNow.Date;
        IdentifiedBy = identifiedBy ?? throw new ArgumentNullException(nameof(identifiedBy));
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string description, string modifiedBy)
    {
        Title = title?.Trim() ?? throw new ArgumentException("Title cannot be empty");
        Description = description?.Trim() ?? throw new ArgumentException("Description cannot be empty");
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void UpdateRiskScore(RiskScore riskScore, string modifiedBy)
    {
        RiskScore = riskScore ?? throw new ArgumentNullException(nameof(riskScore));
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void AssignOwner(Guid ownerId, string ownerName, string modifiedBy)
    {
        OwnerId = ownerId;
        OwnerName = ownerName?.Trim() ?? throw new ArgumentException("Owner name cannot be empty");
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void SetMitigationPlan(string mitigationPlan, string modifiedBy)
    {
        MitigationPlan = mitigationPlan?.Trim();
        Status = RiskStatus.PlanningResponse;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void SetContingencyPlan(string contingencyPlan, string modifiedBy)
    {
        ContingencyPlan = contingencyPlan?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void StartMonitoring(string modifiedBy)
    {
        Status = RiskStatus.Monitoring;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void Close(string modifiedBy, DateTime? resolutionDate = null)
    {
        Status = RiskStatus.Closed;
        ActualResolutionDate = resolutionDate?.Date ?? DateTime.UtcNow.Date;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void SetTargetResolutionDate(DateTime targetDate, string modifiedBy)
    {
        TargetResolutionDate = targetDate.Date;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public bool IsHigh => RiskScore.RiskLevel == "High" || RiskScore.RiskLevel == "Critical";
    public bool IsResolved => Status == RiskStatus.Closed;
    public bool IsOverdue => TargetResolutionDate < DateTime.Today && !IsResolved;
    public bool HasMitigationPlan => !string.IsNullOrWhiteSpace(MitigationPlan);
    public bool HasContingencyPlan => !string.IsNullOrWhiteSpace(ContingencyPlan);
}

/// <summary>
/// Project Issue aggregate for issue tracking
/// </summary>
public class ProjectIssue : AggregateRoot
{
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public IssueType Type { get; private set; }
    public IssueSeverity Severity { get; private set; }
    public IssueStatus Status { get; private set; }
    public Priority Priority { get; private set; }
    public Guid? AssigneeId { get; private set; }
    public string? AssigneeName { get; private set; }
    public string? Resolution { get; private set; }
    public DateTime ReportedDate { get; private set; }
    public DateTime? TargetResolutionDate { get; private set; }
    public DateTime? ActualResolutionDate { get; private set; }
    public string ReportedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    private ProjectIssue()
    {
        Title = null!;
        Description = null!;
        Priority = null!;
        ReportedBy = null!;
    } // EF Core

    public ProjectIssue(
        Guid id,
        Guid projectId,
        string title,
        string description,
        IssueType type,
        IssueSeverity severity,
        Priority priority,
        string reportedBy,
        DateTime? reportedDate = null,
        Guid? assigneeId = null,
        string? assigneeName = null)
    {
        Id = id;
        ProjectId = projectId;
        Title = title?.Trim() ?? throw new ArgumentException("Title cannot be empty");
        Description = description?.Trim() ?? throw new ArgumentException("Description cannot be empty");
        Type = type;
        Severity = severity;
        Priority = priority ?? throw new ArgumentNullException(nameof(priority));
        Status = IssueStatus.Open;
        AssigneeId = assigneeId;
        AssigneeName = assigneeName?.Trim();
        ReportedDate = reportedDate?.Date ?? DateTime.UtcNow.Date;
        ReportedBy = reportedBy ?? throw new ArgumentNullException(nameof(reportedBy));
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string description, string modifiedBy)
    {
        Title = title?.Trim() ?? throw new ArgumentException("Title cannot be empty");
        Description = description?.Trim() ?? throw new ArgumentException("Description cannot be empty");
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void UpdateSeverity(IssueSeverity severity, string modifiedBy)
    {
        Severity = severity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void UpdatePriority(Priority priority, string modifiedBy)
    {
        Priority = priority ?? throw new ArgumentNullException(nameof(priority));
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void Assign(Guid assigneeId, string assigneeName, string modifiedBy)
    {
        AssigneeId = assigneeId;
        AssigneeName = assigneeName?.Trim() ?? throw new ArgumentException("Assignee name cannot be empty");
        Status = IssueStatus.InProgress;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void Unassign(string modifiedBy)
    {
        AssigneeId = null;
        AssigneeName = null;
        Status = IssueStatus.Open;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void Resolve(string resolution, string modifiedBy, DateTime? resolutionDate = null)
    {
        if (Status == IssueStatus.Closed)
            throw new InvalidOperationException("Cannot resolve a closed issue");

        Resolution = resolution?.Trim() ?? throw new ArgumentException("Resolution cannot be empty");
        Status = IssueStatus.Resolved;
        ActualResolutionDate = resolutionDate?.Date ?? DateTime.UtcNow.Date;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void Close(string modifiedBy)
    {
        if (Status != IssueStatus.Resolved)
            throw new InvalidOperationException("Only resolved issues can be closed");

        Status = IssueStatus.Closed;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void Reopen(string modifiedBy, string reason)
    {
        if (Status != IssueStatus.Resolved && Status != IssueStatus.Closed)
            throw new InvalidOperationException("Only resolved or closed issues can be reopened");

        Status = IssueStatus.Reopened;
        Resolution = null;
        ActualResolutionDate = null;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public void SetTargetResolutionDate(DateTime targetDate, string modifiedBy)
    {
        TargetResolutionDate = targetDate.Date;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    public bool IsOpen => Status == IssueStatus.Open || Status == IssueStatus.InProgress || Status == IssueStatus.Reopened;
    public bool IsResolved => Status == IssueStatus.Resolved || Status == IssueStatus.Closed;
    public bool IsOverdue => TargetResolutionDate < DateTime.Today && !IsResolved;
    public bool IsAssigned => AssigneeId.HasValue;
    public bool IsCritical => Severity == IssueSeverity.Critical || Severity == IssueSeverity.Blocker;
}
