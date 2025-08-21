using Collaboration.Domain.Common;
using Collaboration.Domain.Enums;
using Collaboration.Domain.Events;

namespace Collaboration.Domain.Entities;

/// <summary>
/// Represents a participant in a group-buy campaign
/// </summary>
public class CampaignParticipant : Entity
{
    public Guid CampaignId { get; private set; }
    public Guid UserId { get; private set; }
    public string UserName { get; private set; }
    public string UserEmail { get; private set; }
    public ParticipantStatus Status { get; private set; }
    public decimal CommittedAmount { get; private set; }
    public decimal? ActualAmount { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public DateTime? LeftAt { get; private set; }
    public string? LeaveReason { get; private set; }
    public DateTime? PaymentReceivedAt { get; private set; }
    public decimal? PaymentAmount { get; private set; }
    public string? PaymentReference { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Navigation properties
    public Campaign Campaign { get; private set; } = null!;

    // Private constructor for EF Core
    private CampaignParticipant() 
    { 
        UserName = string.Empty;
        UserEmail = string.Empty;
    }

    public CampaignParticipant(
        Guid campaignId,
        Guid userId,
        string userName,
        string userEmail,
        decimal committedAmount,
        Guid tenantId)
    {
        if (campaignId == Guid.Empty)
            throw new ArgumentException("Campaign ID cannot be empty", nameof(campaignId));

        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User name cannot be empty", nameof(userName));

        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("User email cannot be empty", nameof(userEmail));

        if (committedAmount <= 0)
            throw new ArgumentException("Committed amount must be greater than 0", nameof(committedAmount));

        CampaignId = campaignId;
        UserId = userId;
        UserName = userName;
        UserEmail = userEmail;
        Status = ParticipantStatus.Joined;
        CommittedAmount = committedAmount;
        JoinedAt = DateTime.UtcNow;
        TenantId = tenantId;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new ParticipantJoinedEvent(campaignId, Id, userId, userName, committedAmount, tenantId));
    }

    public void UpdateCommittedAmount(decimal newAmount)
    {
        if (newAmount <= 0)
            throw new ArgumentException("Committed amount must be greater than 0", nameof(newAmount));

        if (Status != ParticipantStatus.Joined)
            throw new InvalidOperationException("Cannot update committed amount for non-joined participants");

        CommittedAmount = newAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Leave(string reason)
    {
        if (Status != ParticipantStatus.Joined)
            throw new InvalidOperationException("Only joined participants can leave");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Leave reason is required", nameof(reason));

        Status = ParticipantStatus.Left;
        LeaveReason = reason;
        LeftAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Rejoin()
    {
        if (Status != ParticipantStatus.Left)
            throw new InvalidOperationException("Only left participants can rejoin");

        Status = ParticipantStatus.Joined;
        LeftAt = null;
        LeaveReason = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmPayment(decimal amount, string reference)
    {
        if (Status != ParticipantStatus.Joined)
            throw new InvalidOperationException("Only joined participants can confirm payment");

        if (amount <= 0)
            throw new ArgumentException("Payment amount must be greater than 0", nameof(amount));

        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException("Payment reference is required", nameof(reference));

        PaymentReceivedAt = DateTime.UtcNow;
        PaymentAmount = amount;
        PaymentReference = reference;
        Status = ParticipantStatus.PaymentConfirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActualAmount(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Actual amount cannot be negative", nameof(amount));

        ActualAmount = amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasConfirmedPayment => PaymentReceivedAt.HasValue;
    public bool IsActive => Status == ParticipantStatus.Joined || Status == ParticipantStatus.PaymentConfirmed;
    public bool CanLeave => Status == ParticipantStatus.Joined;
    public bool CanUpdateAmount => Status == ParticipantStatus.Joined;

    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
