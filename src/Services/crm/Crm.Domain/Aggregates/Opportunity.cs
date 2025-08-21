using TossErp.CRM.Domain.Entities;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.Events;
using TossErp.CRM.Domain.SeedWork;
using TossErp.CRM.Domain.ValueObjects;

namespace TossErp.CRM.Domain.Aggregates;

/// <summary>
/// Opportunity aggregate representing sales opportunities in the pipeline
/// Handles opportunity lifecycle, stage progression, and revenue forecasting
/// </summary>
public class Opportunity : AggregateRoot
{
    private readonly List<Activity> _activities;
    private readonly List<Note> _notes;
    private readonly List<Communication> _communications;
    private readonly List<Document> _documents;

    // Core properties
    public Guid TenantId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid? LeadId { get; private set; } // Source lead if converted

    // Opportunity details
    public OpportunityStage Stage { get; private set; }
    public OpportunityType Type { get; private set; }
    public OpportunityValue Value { get; private set; }
    public OpportunityPriority Priority { get; private set; }
    public LeadSource? Source { get; private set; }

    // Timeline
    public DateTime ExpectedCloseDate { get; private set; }
    public DateTime? ActualCloseDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    // Assignment and ownership
    public string? AssignedTo { get; private set; }
    public string? SalesTeam { get; private set; }

    // Tracking
    public DateTime? LastActivityDate { get; private set; }
    public int ContactAttempts { get; private set; }
    public DateTime? NextFollowUp { get; private set; }

    // Closure details
    public string? WinReason { get; private set; }
    public string? LossReason { get; private set; }
    public string? CompetitorName { get; private set; }
    public Money? ActualValue { get; private set; }

    // Campaign tracking
    public Guid? CampaignId { get; private set; }
    public string? CampaignName { get; private set; }

    // Collections
    public IReadOnlyList<Activity> Activities => _activities.AsReadOnly();
    public IReadOnlyList<Note> Notes => _notes.AsReadOnly();
    public IReadOnlyList<Communication> Communications => _communications.AsReadOnly();
    public IReadOnlyList<Document> Documents => _documents.AsReadOnly();

    private Opportunity() 
    { 
        _activities = new List<Activity>();
        _notes = new List<Note>();
        _communications = new List<Communication>();
        _documents = new List<Document>();
        Name = null!;
        Value = null!;
        CreatedBy = null!;
    } // EF Core

    public Opportunity(
        Guid id,
        Guid tenantId,
        string name,
        Guid customerId,
        OpportunityValue value,
        DateTime expectedCloseDate,
        string createdBy,
        string? description = null,
        OpportunityType type = OpportunityType.NewBusiness,
        OpportunityPriority priority = OpportunityPriority.Medium,
        LeadSource? source = null,
        Guid? leadId = null,
        Guid? campaignId = null,
        string? campaignName = null)
    {
        Id = id;
        TenantId = tenantId;
        Name = name?.Trim() ?? throw new ArgumentException("Opportunity name cannot be empty");
        Description = description?.Trim();
        CustomerId = customerId;
        LeadId = leadId;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Stage = OpportunityStage.Prospecting;
        Type = type;
        Priority = priority;
        Source = source;
        ExpectedCloseDate = expectedCloseDate;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        ContactAttempts = 0;
        CampaignId = campaignId;
        CampaignName = campaignName?.Trim();

        _activities = new List<Activity>();
        _notes = new List<Note>();
        _communications = new List<Communication>();
        _documents = new List<Document>();

        AddDomainEvent(new OpportunityCreatedEvent(tenantId, id, customerId, name, value, createdBy));
    }

    #region Opportunity Management

    public void UpdateBasicInfo(
        string name,
        string modifiedBy,
        string? description = null,
        OpportunityType? type = null,
        OpportunityPriority? priority = null)
    {
        Name = name?.Trim() ?? throw new ArgumentException("Opportunity name cannot be empty");
        Description = description?.Trim();
        
        if (type.HasValue)
            Type = type.Value;
        
        if (priority.HasValue)
            Priority = priority.Value;

        UpdateModificationInfo(modifiedBy);
    }

    public void UpdateValue(OpportunityValue newValue, string updatedBy, string? reason = null)
    {
        if (Stage == OpportunityStage.ClosedWon || Stage == OpportunityStage.ClosedLost)
            throw new InvalidOperationException("Cannot update value of closed opportunity");

        var previousValue = Value;
        Value = newValue ?? throw new ArgumentNullException(nameof(newValue));
        UpdateModificationInfo(updatedBy);

        AddDomainEvent(new OpportunityValueUpdatedEvent(TenantId, Id, previousValue, newValue, updatedBy, reason));
    }

    public void UpdateExpectedCloseDate(DateTime newExpectedCloseDate, string updatedBy)
    {
        if (Stage == OpportunityStage.ClosedWon || Stage == OpportunityStage.ClosedLost)
            throw new InvalidOperationException("Cannot update close date of closed opportunity");

        ExpectedCloseDate = newExpectedCloseDate;
        UpdateModificationInfo(updatedBy);
    }

    #endregion

    #region Stage Management

    public void AdvanceStage(OpportunityStage newStage, string changedBy, string? reason = null)
    {
        if (Stage == newStage)
            return;

        ValidateStageTransition(Stage, newStage);

        var previousStage = Stage;
        var previousProbability = Value.Probability;
        
        Stage = newStage;
        
        // Update probability based on stage
        var newProbability = GetStandardProbabilityForStage(newStage);
        Value = new OpportunityValue(Value.EstimatedValue, newProbability);
        
        UpdateModificationInfo(changedBy);

        AddDomainEvent(new OpportunityStageChangedEvent(
            TenantId, Id, previousStage, newStage, newProbability, changedBy, previousProbability));
    }

    private void ValidateStageTransition(OpportunityStage fromStage, OpportunityStage toStage)
    {
        // Business rules for stage transitions
        if (fromStage == OpportunityStage.ClosedWon || fromStage == OpportunityStage.ClosedLost)
            throw new InvalidOperationException("Cannot change stage of closed opportunity");

        // Ensure logical progression (can go backward for corrections)
        if (toStage == OpportunityStage.ClosedWon || toStage == OpportunityStage.ClosedLost)
        {
            if (fromStage == OpportunityStage.Prospecting)
                throw new InvalidOperationException("Cannot close opportunity directly from prospecting stage");
        }
    }

    private decimal GetStandardProbabilityForStage(OpportunityStage stage)
    {
        return stage switch
        {
            OpportunityStage.Prospecting => 10,
            OpportunityStage.Qualification => 25,
            OpportunityStage.NeedsAnalysis => 40,
            OpportunityStage.Proposal => 60,
            OpportunityStage.Negotiation => 80,
            OpportunityStage.ClosedWon => 100,
            OpportunityStage.ClosedLost => 0,
            _ => 10
        };
    }

    #endregion

    #region Assignment Management

    public void Assign(string assignedTo, string assignedBy, string? salesTeam = null)
    {
        AssignedTo = assignedTo?.Trim() ?? throw new ArgumentNullException(nameof(assignedTo));
        SalesTeam = salesTeam?.Trim();
        UpdateModificationInfo(assignedBy);
    }

    public void Unassign(string unassignedBy)
    {
        AssignedTo = null;
        SalesTeam = null;
        UpdateModificationInfo(unassignedBy);
    }

    #endregion

    #region Closure Management

    public void Win(Money actualValue, string wonBy, string? winReason = null)
    {
        if (Stage == OpportunityStage.ClosedWon)
            throw new InvalidOperationException("Opportunity is already won");

        if (Stage == OpportunityStage.ClosedLost)
            throw new InvalidOperationException("Cannot win a lost opportunity");

        Stage = OpportunityStage.ClosedWon;
        ActualCloseDate = DateTime.UtcNow;
        ActualValue = actualValue ?? throw new ArgumentNullException(nameof(actualValue));
        WinReason = winReason?.Trim();
        Value = new OpportunityValue(Value.EstimatedValue, 100);
        UpdateModificationInfo(wonBy);

        AddDomainEvent(new OpportunityWonEvent(TenantId, Id, CustomerId, actualValue, wonBy, winReason));
    }

    public void Lose(string lostBy, string lossReason, string? competitorName = null)
    {
        if (Stage == OpportunityStage.ClosedLost)
            throw new InvalidOperationException("Opportunity is already lost");

        if (Stage == OpportunityStage.ClosedWon)
            throw new InvalidOperationException("Cannot lose a won opportunity");

        Stage = OpportunityStage.ClosedLost;
        ActualCloseDate = DateTime.UtcNow;
        LossReason = lossReason?.Trim() ?? throw new ArgumentException("Loss reason is required");
        CompetitorName = competitorName?.Trim();
        Value = new OpportunityValue(Value.EstimatedValue, 0);
        UpdateModificationInfo(lostBy);

        AddDomainEvent(new OpportunityLostEvent(TenantId, Id, CustomerId, lostBy, lossReason, competitorName));
    }

    public void Reopen(string reopenedBy, string reason)
    {
        if (Stage != OpportunityStage.ClosedLost)
            throw new InvalidOperationException("Can only reopen lost opportunities");

        Stage = OpportunityStage.Prospecting;
        ActualCloseDate = null;
        LossReason = null;
        CompetitorName = null;
        ActualValue = null;
        Value = new OpportunityValue(Value.EstimatedValue, GetStandardProbabilityForStage(OpportunityStage.Prospecting));
        UpdateModificationInfo(reopenedBy);

        // Could add domain event for reopening if needed
    }

    #endregion

    #region Activity Management

    public void ScheduleActivity(Activity activity)
    {
        if (activity == null)
            throw new ArgumentNullException(nameof(activity));

        _activities.Add(activity);
        RecordActivity();

        AddDomainEvent(new ActivityScheduledEvent(
            TenantId, 
            activity.Id, 
            activity.Type, 
            activity.Subject, 
            activity.ScheduledAt, 
            activity.CreatedBy, 
            activity.AssignedTo));
    }

    public void CompleteActivity(Guid activityId, string completedBy, string? outcome = null, string? nextAction = null)
    {
        var activity = _activities.FirstOrDefault(a => a.Id == activityId);
        if (activity == null)
            throw new ArgumentException("Activity not found", nameof(activityId));

        activity.Complete(outcome, nextAction);
        RecordActivity();
        ContactAttempts++;

        AddDomainEvent(new ActivityCompletedEvent(
            TenantId, 
            activityId, 
            activity.Type, 
            completedBy, 
            outcome, 
            nextAction));
    }

    public void ScheduleFollowUp(DateTime followUpDate, string scheduledBy)
    {
        NextFollowUp = followUpDate;
        UpdateModificationInfo(scheduledBy);
    }

    #endregion

    #region Communication Management

    public void AddCommunication(Communication communication)
    {
        if (communication == null)
            throw new ArgumentNullException(nameof(communication));

        _communications.Add(communication);
        RecordActivity();
        ContactAttempts++;
    }

    #endregion

    #region Document Management

    public void AddDocument(Document document)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));

        _documents.Add(document);
        RecordActivity();
    }

    public IEnumerable<Document> GetActiveDocuments()
    {
        return _documents.Where(d => d.IsActive);
    }

    public IEnumerable<Document> GetProposalDocuments()
    {
        return _documents.Where(d => d.IsActive && d.Type == DocumentType.Proposal);
    }

    public IEnumerable<Document> GetContractDocuments()
    {
        return _documents.Where(d => d.IsActive && d.Type == DocumentType.Contract);
    }

    #endregion

    #region Notes Management

    public void AddNote(Note note)
    {
        if (note == null)
            throw new ArgumentNullException(nameof(note));

        _notes.Add(note);
        RecordActivity();
    }

    #endregion

    #region Campaign Tracking

    public void AssignToCampaign(Guid campaignId, string campaignName, string assignedBy)
    {
        CampaignId = campaignId;
        CampaignName = campaignName?.Trim();
        UpdateModificationInfo(assignedBy);
    }

    public void RemoveFromCampaign(string removedBy)
    {
        CampaignId = null;
        CampaignName = null;
        UpdateModificationInfo(removedBy);
    }

    #endregion

    #region Business Logic Properties

    public bool IsOpen => Stage != OpportunityStage.ClosedWon && Stage != OpportunityStage.ClosedLost;
    public bool IsClosed => Stage == OpportunityStage.ClosedWon || Stage == OpportunityStage.ClosedLost;
    public bool IsWon => Stage == OpportunityStage.ClosedWon;
    public bool IsLost => Stage == OpportunityStage.ClosedLost;

    public bool IsOverdue => IsOpen && ExpectedCloseDate < DateTime.UtcNow.Date;
    public bool IsClosingSoon => IsOpen && ExpectedCloseDate <= DateTime.UtcNow.Date.AddDays(7);

    public TimeSpan? TimeSinceLastActivity => LastActivityDate.HasValue ? 
        DateTime.UtcNow - LastActivityDate.Value : null;

    public bool IsStale => TimeSinceLastActivity.HasValue && TimeSinceLastActivity.Value.TotalDays > 14;

    public bool IsHighPriority => Priority == OpportunityPriority.High || 
                                 Value.EstimatedValue.Amount >= 50000 ||
                                 IsClosingSoon;

    public int DaysToClose => (ExpectedCloseDate.Date - DateTime.UtcNow.Date).Days;

    public decimal WeightedValue => Value.WeightedValue.Amount;

    #endregion

    #region Business Logic Helpers

    private void UpdateModificationInfo(string modifiedBy)
    {
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    private void RecordActivity()
    {
        LastActivityDate = DateTime.UtcNow;
    }

    public IEnumerable<Activity> GetOverdueActivities()
    {
        return _activities.Where(a => a.IsOverdue);
    }

    public IEnumerable<Activity> GetUpcomingActivities(int days = 7)
    {
        var cutoff = DateTime.UtcNow.AddDays(days);
        return _activities.Where(a => a.Status == ActivityStatus.Scheduled && 
                                    a.ScheduledAt <= cutoff && 
                                    a.ScheduledAt >= DateTime.UtcNow)
                         .OrderBy(a => a.ScheduledAt);
    }

    public IEnumerable<Communication> GetRecentCommunications(int days = 30)
    {
        var cutoff = DateTime.UtcNow.AddDays(-days);
        return _communications.Where(c => c.CommunicatedAt >= cutoff)
                            .OrderByDescending(c => c.CommunicatedAt);
    }

    public Activity? GetNextScheduledActivity()
    {
        return _activities.Where(a => a.Status == ActivityStatus.Scheduled && 
                                    a.ScheduledAt >= DateTime.UtcNow)
                         .OrderBy(a => a.ScheduledAt)
                         .FirstOrDefault();
    }

    #endregion
}
