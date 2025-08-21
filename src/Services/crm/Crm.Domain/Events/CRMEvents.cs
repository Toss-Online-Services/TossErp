using MediatR;
using TossErp.CRM.Domain.Enums;
using TossErp.CRM.Domain.ValueObjects;

namespace TossErp.CRM.Domain.Events;

/// <summary>
/// Base class for CRM domain events
/// </summary>
public abstract class CRMDomainEvent : INotification
{
    public Guid TenantId { get; }
    public DateTime OccurredOn { get; }

    protected CRMDomainEvent(Guid tenantId)
    {
        TenantId = tenantId;
        OccurredOn = DateTime.UtcNow;
    }
}

#region Customer Events

/// <summary>
/// Event raised when a new customer is created
/// </summary>
public class CustomerCreatedEvent : CRMDomainEvent
{
    public Guid CustomerId { get; }
    public string CustomerName { get; }
    public CustomerType CustomerType { get; }
    public string CreatedBy { get; }

    public CustomerCreatedEvent(Guid tenantId, Guid customerId, string customerName, CustomerType customerType, string createdBy)
        : base(tenantId)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        CustomerType = customerType;
        CreatedBy = createdBy;
    }
}

/// <summary>
/// Event raised when customer status changes
/// </summary>
public class CustomerStatusChangedEvent : CRMDomainEvent
{
    public Guid CustomerId { get; }
    public CustomerStatus PreviousStatus { get; }
    public CustomerStatus NewStatus { get; }
    public string ChangedBy { get; }
    public string? Reason { get; }

    public CustomerStatusChangedEvent(Guid tenantId, Guid customerId, CustomerStatus previousStatus, CustomerStatus newStatus, string changedBy, string? reason = null)
        : base(tenantId)
    {
        CustomerId = customerId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        ChangedBy = changedBy;
        Reason = reason;
    }
}

/// <summary>
/// Event raised when customer is upgraded to a higher tier
/// </summary>
public class CustomerUpgradedEvent : CRMDomainEvent
{
    public Guid CustomerId { get; }
    public CustomerTier PreviousTier { get; }
    public CustomerTier NewTier { get; }
    public Money? NewAnnualValue { get; }
    public string UpgradedBy { get; }

    public CustomerUpgradedEvent(Guid tenantId, Guid customerId, CustomerTier previousTier, CustomerTier newTier, string upgradedBy, Money? newAnnualValue = null)
        : base(tenantId)
    {
        CustomerId = customerId;
        PreviousTier = previousTier;
        NewTier = newTier;
        NewAnnualValue = newAnnualValue;
        UpgradedBy = upgradedBy;
    }
}

/// <summary>
/// Event raised when customer subscription is renewed
/// </summary>
public class CustomerSubscriptionRenewedEvent : CRMDomainEvent
{
    public Guid CustomerId { get; }
    public DateTime NewExpiryDate { get; }
    public Money RenewalAmount { get; }
    public string RenewedBy { get; }

    public CustomerSubscriptionRenewedEvent(Guid tenantId, Guid customerId, DateTime newExpiryDate, Money renewalAmount, string renewedBy)
        : base(tenantId)
    {
        CustomerId = customerId;
        NewExpiryDate = newExpiryDate;
        RenewalAmount = renewalAmount;
        RenewedBy = renewedBy;
    }
}

#endregion

#region Lead Events

/// <summary>
/// Event raised when a new lead is created
/// </summary>
public class LeadCreatedEvent : CRMDomainEvent
{
    public Guid LeadId { get; }
    public string LeadName { get; }
    public string Company { get; }
    public LeadSource Source { get; }
    public LeadScore InitialScore { get; }
    public string CreatedBy { get; }

    public LeadCreatedEvent(Guid tenantId, Guid leadId, string leadName, string company, LeadSource source, LeadScore initialScore, string createdBy)
        : base(tenantId)
    {
        LeadId = leadId;
        LeadName = leadName;
        Company = company;
        Source = source;
        InitialScore = initialScore;
        CreatedBy = createdBy;
    }
}

/// <summary>
/// Event raised when lead status changes
/// </summary>
public class LeadStatusChangedEvent : CRMDomainEvent
{
    public Guid LeadId { get; }
    public LeadStatus PreviousStatus { get; }
    public LeadStatus NewStatus { get; }
    public string ChangedBy { get; }
    public string? Reason { get; }

    public LeadStatusChangedEvent(Guid tenantId, Guid leadId, LeadStatus previousStatus, LeadStatus newStatus, string changedBy, string? reason = null)
        : base(tenantId)
    {
        LeadId = leadId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        ChangedBy = changedBy;
        Reason = reason;
    }
}

/// <summary>
/// Event raised when lead is qualified
/// </summary>
public class LeadQualifiedEvent : CRMDomainEvent
{
    public Guid LeadId { get; }
    public LeadScore QualificationScore { get; }
    public string QualifiedBy { get; }
    public string? QualificationNotes { get; }

    public LeadQualifiedEvent(Guid tenantId, Guid leadId, LeadScore qualificationScore, string qualifiedBy, string? qualificationNotes = null)
        : base(tenantId)
    {
        LeadId = leadId;
        QualificationScore = qualificationScore;
        QualifiedBy = qualifiedBy;
        QualificationNotes = qualificationNotes;
    }
}

/// <summary>
/// Event raised when lead is converted to customer
/// </summary>
public class LeadConvertedEvent : CRMDomainEvent
{
    public Guid LeadId { get; }
    public Guid CustomerId { get; }
    public Guid? OpportunityId { get; }
    public string ConvertedBy { get; }
    public DateTime ConvertedAt { get; }

    public LeadConvertedEvent(Guid tenantId, Guid leadId, Guid customerId, string convertedBy, Guid? opportunityId = null)
        : base(tenantId)
    {
        LeadId = leadId;
        CustomerId = customerId;
        OpportunityId = opportunityId;
        ConvertedBy = convertedBy;
        ConvertedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Event raised when lead is assigned to a sales representative
/// </summary>
public class LeadAssignedEvent : CRMDomainEvent
{
    public Guid LeadId { get; }
    public string AssignedTo { get; }
    public string? PreviousAssignee { get; }
    public string AssignedBy { get; }

    public LeadAssignedEvent(Guid tenantId, Guid leadId, string assignedTo, string assignedBy, string? previousAssignee = null)
        : base(tenantId)
    {
        LeadId = leadId;
        AssignedTo = assignedTo;
        PreviousAssignee = previousAssignee;
        AssignedBy = assignedBy;
    }
}

#endregion

#region Opportunity Events

/// <summary>
/// Event raised when a new opportunity is created
/// </summary>
public class OpportunityCreatedEvent : CRMDomainEvent
{
    public Guid OpportunityId { get; }
    public Guid CustomerId { get; }
    public string OpportunityName { get; }
    public OpportunityValue EstimatedValue { get; }
    public string CreatedBy { get; }

    public OpportunityCreatedEvent(Guid tenantId, Guid opportunityId, Guid customerId, string opportunityName, OpportunityValue estimatedValue, string createdBy)
        : base(tenantId)
    {
        OpportunityId = opportunityId;
        CustomerId = customerId;
        OpportunityName = opportunityName;
        EstimatedValue = estimatedValue;
        CreatedBy = createdBy;
    }
}

/// <summary>
/// Event raised when opportunity stage changes
/// </summary>
public class OpportunityStageChangedEvent : CRMDomainEvent
{
    public Guid OpportunityId { get; }
    public OpportunityStage PreviousStage { get; }
    public OpportunityStage NewStage { get; }
    public decimal? PreviousProbability { get; }
    public decimal NewProbability { get; }
    public string ChangedBy { get; }

    public OpportunityStageChangedEvent(Guid tenantId, Guid opportunityId, OpportunityStage previousStage, OpportunityStage newStage, decimal newProbability, string changedBy, decimal? previousProbability = null)
        : base(tenantId)
    {
        OpportunityId = opportunityId;
        PreviousStage = previousStage;
        NewStage = newStage;
        PreviousProbability = previousProbability;
        NewProbability = newProbability;
        ChangedBy = changedBy;
    }
}

/// <summary>
/// Event raised when opportunity is won
/// </summary>
public class OpportunityWonEvent : CRMDomainEvent
{
    public Guid OpportunityId { get; }
    public Guid CustomerId { get; }
    public Money ActualValue { get; }
    public DateTime ClosedAt { get; }
    public string WonBy { get; }
    public string? WinReason { get; }

    public OpportunityWonEvent(Guid tenantId, Guid opportunityId, Guid customerId, Money actualValue, string wonBy, string? winReason = null)
        : base(tenantId)
    {
        OpportunityId = opportunityId;
        CustomerId = customerId;
        ActualValue = actualValue;
        ClosedAt = DateTime.UtcNow;
        WonBy = wonBy;
        WinReason = winReason;
    }
}

/// <summary>
/// Event raised when opportunity is lost
/// </summary>
public class OpportunityLostEvent : CRMDomainEvent
{
    public Guid OpportunityId { get; }
    public Guid CustomerId { get; }
    public DateTime ClosedAt { get; }
    public string LostBy { get; }
    public string LossReason { get; }
    public string? CompetitorName { get; }

    public OpportunityLostEvent(Guid tenantId, Guid opportunityId, Guid customerId, string lostBy, string lossReason, string? competitorName = null)
        : base(tenantId)
    {
        OpportunityId = opportunityId;
        CustomerId = customerId;
        ClosedAt = DateTime.UtcNow;
        LostBy = lostBy;
        LossReason = lossReason;
        CompetitorName = competitorName;
    }
}

/// <summary>
/// Event raised when opportunity value is updated
/// </summary>
public class OpportunityValueUpdatedEvent : CRMDomainEvent
{
    public Guid OpportunityId { get; }
    public OpportunityValue PreviousValue { get; }
    public OpportunityValue NewValue { get; }
    public string UpdatedBy { get; }
    public string? UpdateReason { get; }

    public OpportunityValueUpdatedEvent(Guid tenantId, Guid opportunityId, OpportunityValue previousValue, OpportunityValue newValue, string updatedBy, string? updateReason = null)
        : base(tenantId)
    {
        OpportunityId = opportunityId;
        PreviousValue = previousValue;
        NewValue = newValue;
        UpdatedBy = updatedBy;
        UpdateReason = updateReason;
    }
}

#endregion

#region Activity Events

/// <summary>
/// Event raised when an activity is scheduled
/// </summary>
public class ActivityScheduledEvent : CRMDomainEvent
{
    public Guid ActivityId { get; }
    public ActivityType Type { get; }
    public string Subject { get; }
    public DateTime ScheduledAt { get; }
    public string ScheduledBy { get; }
    public string? AssignedTo { get; }

    public ActivityScheduledEvent(Guid tenantId, Guid activityId, ActivityType type, string subject, DateTime scheduledAt, string scheduledBy, string? assignedTo = null)
        : base(tenantId)
    {
        ActivityId = activityId;
        Type = type;
        Subject = subject;
        ScheduledAt = scheduledAt;
        ScheduledBy = scheduledBy;
        AssignedTo = assignedTo;
    }
}

/// <summary>
/// Event raised when an activity is completed
/// </summary>
public class ActivityCompletedEvent : CRMDomainEvent
{
    public Guid ActivityId { get; }
    public ActivityType Type { get; }
    public DateTime CompletedAt { get; }
    public string CompletedBy { get; }
    public string? Outcome { get; }
    public string? NextAction { get; }

    public ActivityCompletedEvent(Guid tenantId, Guid activityId, ActivityType type, string completedBy, string? outcome = null, string? nextAction = null)
        : base(tenantId)
    {
        ActivityId = activityId;
        Type = type;
        CompletedAt = DateTime.UtcNow;
        CompletedBy = completedBy;
        Outcome = outcome;
        NextAction = nextAction;
    }
}

/// <summary>
/// Event raised when an activity becomes overdue
/// </summary>
public class ActivityOverdueEvent : CRMDomainEvent
{
    public Guid ActivityId { get; }
    public ActivityType Type { get; }
    public string Subject { get; }
    public DateTime ScheduledAt { get; }
    public string? AssignedTo { get; }
    public TimeSpan OverdueDuration { get; }

    public ActivityOverdueEvent(Guid tenantId, Guid activityId, ActivityType type, string subject, DateTime scheduledAt, string? assignedTo = null)
        : base(tenantId)
    {
        ActivityId = activityId;
        Type = type;
        Subject = subject;
        ScheduledAt = scheduledAt;
        AssignedTo = assignedTo;
        OverdueDuration = DateTime.UtcNow - scheduledAt;
    }
}

#endregion

#region Campaign Events

/// <summary>
/// Event raised when a marketing campaign is launched
/// </summary>
public class CampaignLaunchedEvent : CRMDomainEvent
{
    public Guid CampaignId { get; }
    public string CampaignName { get; }
    public CampaignType Type { get; }
    public DateTime LaunchDate { get; }
    public string LaunchedBy { get; }
    public int TargetAudience { get; }

    public CampaignLaunchedEvent(Guid tenantId, Guid campaignId, string campaignName, CampaignType type, string launchedBy, int targetAudience)
        : base(tenantId)
    {
        CampaignId = campaignId;
        CampaignName = campaignName;
        Type = type;
        LaunchDate = DateTime.UtcNow;
        LaunchedBy = launchedBy;
        TargetAudience = targetAudience;
    }
}

/// <summary>
/// Event raised when a lead responds to a campaign
/// </summary>
public class CampaignResponseEvent : CRMDomainEvent
{
    public Guid CampaignId { get; }
    public Guid LeadId { get; }
    public CampaignResponseType ResponseType { get; }
    public DateTime ResponseDate { get; }

    public CampaignResponseEvent(Guid tenantId, Guid campaignId, Guid leadId, CampaignResponseType responseType)
        : base(tenantId)
    {
        CampaignId = campaignId;
        LeadId = leadId;
        ResponseType = responseType;
        ResponseDate = DateTime.UtcNow;
    }
}

#endregion
