using TossErp.CRM.Domain.Entities;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.Events;
using TossErp.CRM.Domain.SeedWork;
using TossErp.CRM.Domain.ValueObjects;

namespace TossErp.CRM.Domain.Aggregates;

/// <summary>
/// Lead aggregate representing potential customers in the sales pipeline
/// Handles lead qualification, scoring, and conversion tracking
/// </summary>
public class Lead : AggregateRoot
{
    private readonly List<Activity> _activities;
    private readonly List<Note> _notes;
    private readonly List<Communication> _communications;

    // Core properties
    public Guid TenantId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Company { get; private set; }
    public string? JobTitle { get; private set; }
    public EmailAddress Email { get; private set; }
    public PhoneNumber? Phone { get; private set; }
    public Address? Address { get; private set; }

    // Lead tracking
    public LeadStatus Status { get; private set; }
    public LeadSource Source { get; private set; }
    public LeadScore Score { get; private set; }
    public string? Industry { get; private set; }
    public int? CompanySize { get; private set; }
    public Money? EstimatedValue { get; private set; }
    
    // Enhanced qualification and pipeline tracking
    public LeadQualificationCriteria? QualificationCriteria { get; private set; }
    public string PipelineStageName { get; private set; } = "New";
    public int Priority { get; private set; } = 3; // 1=High, 2=Medium, 3=Low
    public DateTime? ExpectedCloseDate { get; private set; }
    public string? LossReason { get; private set; }

    // Assignment and tracking
    public string? AssignedTo { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public DateTime? LastContactedAt { get; private set; }
    public DateTime? QualifiedAt { get; private set; }
    public string? QualifiedBy { get; private set; }

    // Conversion tracking
    public DateTime? ConvertedAt { get; private set; }
    public Guid? ConvertedCustomerId { get; private set; }
    public Guid? ConvertedOpportunityId { get; private set; }
    public string? ConvertedBy { get; private set; }

    // Campaign tracking
    public Guid? CampaignId { get; private set; }
    public string? CampaignName { get; private set; }

    // Additional tracking properties
    public string? WebsiteUrl { get; private set; }
    public string? Remarks { get; private set; }
    public DateTime? NextFollowUp { get; private set; }
    public int ContactAttempts { get; private set; } = 0;
    public DateTime? LastActivityDate { get; private set; }

    // Soft delete support
    public bool IsDeleted { get; private set; } = false;
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    // Collections
    public IReadOnlyList<Activity> Activities => _activities.AsReadOnly();
    public IReadOnlyList<Note> Notes => _notes.AsReadOnly();
    public IReadOnlyList<Communication> Communications => _communications.AsReadOnly();

    private Lead() 
    { 
        _activities = new List<Activity>();
        _notes = new List<Note>();
        _communications = new List<Communication>();
        FirstName = null!;
        LastName = null!;
        Company = null!;
        Email = null!;
        Score = null!;
        CreatedBy = null!;
    } // EF Core

    public Lead(
        Guid id,
        Guid tenantId,
        string firstName,
        string lastName,
        string company,
        EmailAddress email,
        LeadSource source,
        string createdBy,
        string? jobTitle = null,
        PhoneNumber? phone = null,
        string? industry = null,
        int? companySize = null,
        Guid? campaignId = null,
        string? campaignName = null)
    {
        Id = id;
        TenantId = tenantId;
        FirstName = firstName?.Trim() ?? throw new ArgumentException("First name cannot be empty");
        LastName = lastName?.Trim() ?? throw new ArgumentException("Last name cannot be empty");
        Company = company?.Trim() ?? throw new ArgumentException("Company cannot be empty");
        JobTitle = jobTitle?.Trim();
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone;
        Source = source;
        Status = LeadStatus.New;
        Score = new LeadScore(CalculateInitialScore(source, industry, companySize));
        Industry = industry?.Trim();
        CompanySize = companySize;
        PipelineStageName = "New";
        Priority = 3; // Default to Low priority
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        CampaignId = campaignId;
        CampaignName = campaignName?.Trim();

        _activities = new List<Activity>();
        _notes = new List<Note>();
        _communications = new List<Communication>();

        AddDomainEvent(new LeadCreatedEvent(tenantId, id, FullName, company, source, Score, createdBy));
    }

    public string FullName => $"{FirstName} {LastName}";

    #region Lead Management

    public void UpdateBasicInfo(
        string firstName,
        string lastName,
        string company,
        string modifiedBy,
        string? jobTitle = null,
        string? industry = null,
        int? companySize = null)
    {
        FirstName = firstName?.Trim() ?? throw new ArgumentException("First name cannot be empty");
        LastName = lastName?.Trim() ?? throw new ArgumentException("Last name cannot be empty");
        Company = company?.Trim() ?? throw new ArgumentException("Company cannot be empty");
        JobTitle = jobTitle?.Trim();
        Industry = industry?.Trim();
        CompanySize = companySize;
        
        UpdateModificationInfo(modifiedBy);
    }

    public void UpdateContactInfo(EmailAddress email, PhoneNumber? phone, string modifiedBy)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone;
        UpdateModificationInfo(modifiedBy);
    }

    public void UpdateAddress(Address address, string modifiedBy)
    {
        Address = address;
        UpdateModificationInfo(modifiedBy);
    }

    public void UpdateEstimatedValue(Money estimatedValue, string modifiedBy)
    {
        EstimatedValue = estimatedValue;
        UpdateModificationInfo(modifiedBy);
    }

    #endregion

    #region Status Management

    public void ChangeStatus(LeadStatus newStatus, string changedBy, string? reason = null)
    {
        if (Status == newStatus)
            return;

        ValidateStatusTransition(Status, newStatus);

        var previousStatus = Status;
        Status = newStatus;
        UpdateModificationInfo(changedBy);

        // Special handling for qualification
        if (newStatus == LeadStatus.Qualified && previousStatus != LeadStatus.Qualified)
        {
            QualifiedAt = DateTime.UtcNow;
            QualifiedBy = changedBy;
            AddDomainEvent(new LeadQualifiedEvent(TenantId, Id, Score, changedBy));
        }

        AddDomainEvent(new LeadStatusChangedEvent(TenantId, Id, previousStatus, newStatus, changedBy, reason));
    }

    private void ValidateStatusTransition(LeadStatus fromStatus, LeadStatus toStatus)
    {
        // Business rules for status transitions
        switch (fromStatus)
        {
            case LeadStatus.Converted:
                throw new InvalidOperationException("Cannot change status of converted lead");
            
            case LeadStatus.Disqualified:
                if (toStatus != LeadStatus.New && toStatus != LeadStatus.Open)
                    throw new InvalidOperationException("Disqualified leads can only be reopened");
                break;
            
            case LeadStatus.New:
                if (toStatus == LeadStatus.Converted)
                    throw new InvalidOperationException("New leads must be qualified before conversion");
                break;
        }
    }

    public void Qualify(string qualifiedBy, string? qualificationNotes = null)
    {
        if (Status == LeadStatus.Converted)
            throw new InvalidOperationException("Cannot qualify converted lead");

        ChangeStatus(LeadStatus.Qualified, qualifiedBy, qualificationNotes);
        
        // Increase score for qualification
        Score = Score.Increase(20);
    }

    public void Disqualify(string disqualifiedBy, string reason)
    {
        if (Status == LeadStatus.Converted)
            throw new InvalidOperationException("Cannot disqualify converted lead");

        ChangeStatus(LeadStatus.Disqualified, disqualifiedBy, reason);
        
        // Decrease score for disqualification
        Score = Score.Decrease(30);
    }

    #endregion

    #region Enhanced Lead Qualification

    public void SetQualificationCriteria(
        bool hasBudget,
        bool hasAuthority,
        bool hasNeed,
        bool hasTimeline,
        string evaluatedBy,
        string? notes = null)
    {
        QualificationCriteria = new LeadQualificationCriteria(
            hasBudget, hasAuthority, hasNeed, hasTimeline, evaluatedBy, notes);

        // Update score based on qualification
        IncreaseScore(QualificationCriteria.ScoreWeight, evaluatedBy, "BANT qualification updated");

        // Auto-qualify if fully qualified
        if (QualificationCriteria.IsQualified && Status != LeadStatus.Qualified)
        {
            Qualify(evaluatedBy, "Auto-qualified based on BANT criteria");
        }

        UpdateModificationInfo(evaluatedBy);
    }

    public void UpdatePriority(int priority, string updatedBy)
    {
        if (priority < 1 || priority > 5)
            throw new ArgumentOutOfRangeException(nameof(priority), "Priority must be between 1 (highest) and 5 (lowest)");

        Priority = priority;
        UpdateModificationInfo(updatedBy);
    }

    public void SetExpectedCloseDate(DateTime expectedCloseDate, string updatedBy)
    {
        if (expectedCloseDate < DateTime.UtcNow.Date)
            throw new ArgumentException("Expected close date cannot be in the past");

        ExpectedCloseDate = expectedCloseDate;
        UpdateModificationInfo(updatedBy);
    }

    public void AdvancePipelineStage(string advancedBy, string? reason = null)
    {
        var currentStage = PipelineStageName;
        var nextStage = currentStage switch
        {
            "New" => "Contacted",
            "Contacted" => "Qualified",
            "Qualified" => "Proposal",
            "Proposal" => "Negotiation",
            "Negotiation" => "Won",
            _ => currentStage
        };

        if (nextStage != currentStage)
        {
            MoveToPipelineStage(nextStage, advancedBy, reason);
        }
    }

    public void MoveToPipelineStage(string stageName, string movedBy, string? reason = null)
    {
        var previousStage = PipelineStageName;
        PipelineStageName = stageName;
        UpdateModificationInfo(movedBy);

        // Update status based on stage
        if (stageName == "Won" && Status != LeadStatus.Converted)
        {
            // Don't automatically convert, but allow manual conversion
        }
        else if (stageName == "Lost")
        {
            Disqualify(movedBy, reason ?? "Moved to Lost stage");
            LossReason = reason;
        }
        else if (stageName == "Qualified" && Status == LeadStatus.Open)
        {
            Qualify(movedBy, reason);
        }

        AddDomainEvent(new LeadPipelineStageChangedEvent(
            TenantId, Id, previousStage, stageName, movedBy, reason));
    }

    public string PriorityText => Priority switch
    {
        1 => "Critical",
        2 => "High", 
        3 => "Medium",
        4 => "Low",
        5 => "Very Low",
        _ => "Unknown"
    };

    public bool IsHighPriority => Priority <= 2;
    public bool IsQualificationComplete => QualificationCriteria?.IsQualified == true;
    public bool HasQualificationData => QualificationCriteria != null;
    public string QualificationStatus => QualificationCriteria?.QualificationLevel ?? "Not Evaluated";

    #endregion

    #region Assignment Management

    public void Assign(string assignedTo, string assignedBy)
    {
        var previousAssignee = AssignedTo;
        AssignedTo = assignedTo?.Trim() ?? throw new ArgumentNullException(nameof(assignedTo));
        UpdateModificationInfo(assignedBy);

        AddDomainEvent(new LeadAssignedEvent(TenantId, Id, assignedTo, assignedBy, previousAssignee));
    }

    public void Unassign(string unassignedBy)
    {
        AssignedTo = null;
        UpdateModificationInfo(unassignedBy);
    }

    #endregion

    #region Lead Scoring

    public void UpdateScore(int newScore, string updatedBy)
    {
        Score = new LeadScore(newScore);
        UpdateModificationInfo(updatedBy);
    }

    public void IncreaseScore(int points, string updatedBy, string? reason = null)
    {
        Score = Score.Increase(points);
        UpdateModificationInfo(updatedBy);
        
        // Auto-qualify if score becomes high enough
        if (Score.IsQualified && Status == LeadStatus.Open)
        {
            ChangeStatus(LeadStatus.Qualified, updatedBy, $"Auto-qualified due to high score: {reason}");
        }
    }

    public void DecreaseScore(int points, string updatedBy, string? reason = null)
    {
        Score = Score.Decrease(points);
        UpdateModificationInfo(updatedBy);
    }

    private int CalculateInitialScore(LeadSource source, string? industry, int? companySize)
    {
        int score = 30; // Base score

        // Source-based scoring
        score += source switch
        {
            LeadSource.Referral => 25,
            LeadSource.Website => 15,
            LeadSource.SocialMedia => 10,
            LeadSource.Email => 10,
            LeadSource.Phone => 15,
            LeadSource.TradeShow => 20,
            LeadSource.Advertisement => 5,
            LeadSource.DirectMail => 5,
            LeadSource.Partner => 20,
            _ => 0
        };

        // Industry-based scoring (can be customized based on business focus)
        if (!string.IsNullOrEmpty(industry))
        {
            score += industry.ToLowerInvariant() switch
            {
                var i when i.Contains("technology") || i.Contains("software") => 15,
                var i when i.Contains("healthcare") || i.Contains("medical") => 10,
                var i when i.Contains("finance") || i.Contains("banking") => 10,
                var i when i.Contains("manufacturing") => 10,
                _ => 5
            };
        }

        // Company size-based scoring
        if (companySize.HasValue)
        {
            score += companySize.Value switch
            {
                >= 1000 => 20,
                >= 500 => 15,
                >= 100 => 10,
                >= 50 => 5,
                _ => 0
            };
        }

        return Math.Min(100, Math.Max(0, score));
    }

    #endregion

    #region Activity Management

    public void ScheduleActivity(Activity activity)
    {
        if (activity == null)
            throw new ArgumentNullException(nameof(activity));

        _activities.Add(activity);
        RecordContact();

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
        RecordContact();

        // Increase score for completed activities
        IncreaseScore(5, completedBy, "Activity completed");

        AddDomainEvent(new ActivityCompletedEvent(
            TenantId, 
            activityId, 
            activity.Type, 
            completedBy, 
            outcome, 
            nextAction));
    }

    #endregion

    #region Communication Management

    public void AddCommunication(Communication communication)
    {
        if (communication == null)
            throw new ArgumentNullException(nameof(communication));

        _communications.Add(communication);
        RecordContact();

        // Increase score for communications
        var scoreIncrease = communication.Type switch
        {
            CommunicationType.Email => 3,
            CommunicationType.Phone => 5,
            CommunicationType.Meeting => 10,
            CommunicationType.VideoCall => 8,
            _ => 2
        };

        IncreaseScore(scoreIncrease, communication.CreatedBy, $"Communication: {communication.Type}");
    }

    #endregion

    #region Notes Management

    public void AddNote(Note note)
    {
        if (note == null)
            throw new ArgumentNullException(nameof(note));

        _notes.Add(note);
        RecordContact();
    }

    #endregion

    #region Conversion

    public void ConvertToCustomer(Guid customerId, string convertedBy, Guid? opportunityId = null)
    {
        if (Status == LeadStatus.Converted)
            throw new InvalidOperationException("Lead is already converted");

        if (Status != LeadStatus.Qualified)
            throw new InvalidOperationException("Lead must be qualified before conversion");

        Status = LeadStatus.Converted;
        ConvertedAt = DateTime.UtcNow;
        ConvertedCustomerId = customerId;
        ConvertedOpportunityId = opportunityId;
        ConvertedBy = convertedBy;
        UpdateModificationInfo(convertedBy);

        AddDomainEvent(new LeadConvertedEvent(TenantId, Id, customerId, convertedBy, opportunityId));
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

    #region Business Logic Helpers

    private void UpdateModificationInfo(string modifiedBy)
    {
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
    }

    private void RecordContact()
    {
        LastContactedAt = DateTime.UtcNow;
    }

    public bool IsQualified => Status == LeadStatus.Qualified;
    public bool IsConverted => Status == LeadStatus.Converted;
    public bool IsActive => Status != LeadStatus.Converted && Status != LeadStatus.Disqualified;

    public int DaysInPipeline => (int)(DateTime.UtcNow - CreatedAt).TotalDays;
    public int? DaysSinceLastContact => LastContactedAt.HasValue ? 
        (int)(DateTime.UtcNow - LastContactedAt.Value).TotalDays : null;

    public TimeSpan? TimeSinceLastContact => LastContactedAt.HasValue ? 
        DateTime.UtcNow - LastContactedAt.Value : null;

    public bool IsStale => TimeSinceLastContact.HasValue && TimeSinceLastContact.Value.TotalDays > 30;

    public bool IsHot => Score.IsHot;
    public bool IsCold => Score.IsCold;

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

    public void Delete(string deletedBy)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Lead is already deleted");

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy ?? throw new ArgumentNullException(nameof(deletedBy));
        
        AddDomainEvent(new LeadDeletedEvent(TenantId, Id, FullName, deletedBy));
    }

    #endregion
}
