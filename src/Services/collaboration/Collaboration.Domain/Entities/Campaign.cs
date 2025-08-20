using Collaboration.Domain.Common;
using Collaboration.Domain.Enums;
using Collaboration.Domain.Events;

namespace Collaboration.Domain.Entities;

/// <summary>
/// Represents a group-buy campaign where multiple participants can join to get better pricing
/// </summary>
public class Campaign : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public CampaignStatus Status { get; private set; }
    public CampaignType Type { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }
    public int MinParticipants { get; private set; }
    public int MaxParticipants { get; private set; }
    public int CurrentParticipants { get; private set; }
    public decimal TargetAmount { get; private set; }
    public decimal CurrentAmount { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public Guid CreatedBy { get; private set; }
    public Guid? SupplierId { get; private set; }
    public string SupplierName { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }
    public Guid? CancelledBy { get; private set; }

    private readonly List<CampaignParticipant> _participants = new();
    public IReadOnlyList<CampaignParticipant> Participants => _participants.AsReadOnly();

    private readonly List<CampaignAllocation> _allocations = new();
    public IReadOnlyList<CampaignAllocation> Allocations => _allocations.AsReadOnly();

    private readonly List<SupplierQuotation> _quotations = new();
    public IReadOnlyList<SupplierQuotation> Quotations => _quotations.AsReadOnly();

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Private constructor for EF Core
    private Campaign() { }

    public Campaign(
        string name,
        string description,
        CampaignType type,
        DateTime startDate,
        DateTime endDate,
        int minParticipants,
        int maxParticipants,
        decimal targetAmount,
        decimal discountPercentage,
        Guid createdBy,
        Guid tenantId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Campaign name cannot be empty", nameof(name));

        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date", nameof(startDate));

        if (minParticipants <= 0)
            throw new ArgumentException("Minimum participants must be greater than 0", nameof(minParticipants));

        if (maxParticipants < minParticipants)
            throw new ArgumentException("Maximum participants cannot be less than minimum participants", nameof(maxParticipants));

        if (targetAmount <= 0)
            throw new ArgumentException("Target amount must be greater than 0", nameof(targetAmount));

        if (discountPercentage < 0 || discountPercentage > 100)
            throw new ArgumentException("Discount percentage must be between 0 and 100", nameof(discountPercentage));

        Name = name;
        Description = description;
        Status = CampaignStatus.Draft;
        Type = type;
        StartDate = startDate;
        EndDate = endDate;
        MinParticipants = minParticipants;
        MaxParticipants = maxParticipants;
        CurrentParticipants = 0;
        TargetAmount = targetAmount;
        CurrentAmount = 0;
        DiscountPercentage = discountPercentage;
        CreatedBy = createdBy;
        TenantId = tenantId;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string description, DateTime startDate, DateTime endDate)
    {
        if (Status != CampaignStatus.Draft)
            throw new InvalidOperationException("Cannot update campaign details after it has been launched");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Campaign name cannot be empty", nameof(name));

        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date", nameof(startDate));

        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Launch()
    {
        if (Status != CampaignStatus.Draft)
            throw new InvalidOperationException("Campaign can only be launched from draft status");

        if (DateTime.UtcNow < StartDate)
            throw new InvalidOperationException("Cannot launch campaign before start date");

        Status = CampaignStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CampaignLaunchedEvent(Id, Name, CreatedBy, TenantId));
    }

    public void Pause()
    {
        if (Status != CampaignStatus.Active)
            throw new InvalidOperationException("Only active campaigns can be paused");

        Status = CampaignStatus.Paused;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Resume()
    {
        if (Status != CampaignStatus.Paused)
            throw new InvalidOperationException("Only paused campaigns can be resumed");

        Status = CampaignStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != CampaignStatus.Active)
            throw new InvalidOperationException("Only active campaigns can be completed");

        if (CurrentParticipants < MinParticipants)
            throw new InvalidOperationException("Cannot complete campaign without minimum participants");

        Status = CampaignStatus.Completed;
        ActualEndDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CampaignCompletedEvent(Id, Name, CurrentParticipants, CurrentAmount, DiscountPercentage, TenantId));
    }

    public void Cancel(string reason, Guid cancelledBy)
    {
        if (Status == CampaignStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed campaigns");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason is required", nameof(reason));

        Status = CampaignStatus.Cancelled;
        CancellationReason = reason;
        CancelledBy = cancelledBy;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddParticipant(CampaignParticipant participant)
    {
        if (Status != CampaignStatus.Active)
            throw new InvalidOperationException("Cannot add participants to non-active campaigns");

        if (CurrentParticipants >= MaxParticipants)
            throw new InvalidOperationException("Campaign has reached maximum participants");

        if (_participants.Any(p => p.UserId == participant.UserId))
            throw new InvalidOperationException("User is already a participant in this campaign");

        _participants.Add(participant);
        CurrentParticipants++;
        CurrentAmount += participant.CommittedAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveParticipant(Guid userId)
    {
        var participant = _participants.FirstOrDefault(p => p.UserId == userId);
        if (participant == null)
            throw new InvalidOperationException("User is not a participant in this campaign");

        if (Status == CampaignStatus.Completed)
            throw new InvalidOperationException("Cannot remove participants from completed campaigns");

        _participants.Remove(participant);
        CurrentParticipants--;
        CurrentAmount -= participant.CommittedAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddQuotation(SupplierQuotation quotation)
    {
        if (Status == CampaignStatus.Completed)
            throw new InvalidOperationException("Cannot add quotations to completed campaigns");

        if (_quotations.Any(q => q.SupplierId == quotation.SupplierId))
            throw new InvalidOperationException("Supplier has already submitted a quotation");

        _quotations.Add(quotation);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SelectSupplier(Guid supplierId, string supplierName)
    {
        if (Status != CampaignStatus.Active)
            throw new InvalidOperationException("Can only select supplier for active campaigns");

        if (!_quotations.Any(q => q.SupplierId == supplierId))
            throw new InvalidOperationException("Selected supplier must have submitted a quotation");

        SupplierId = supplierId;
        SupplierName = supplierName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAllocation(CampaignAllocation allocation)
    {
        if (Status != CampaignStatus.Completed)
            throw new InvalidOperationException("Can only add allocations to completed campaigns");

        if (_allocations.Any(a => a.ParticipantId == allocation.ParticipantId))
            throw new InvalidOperationException("Participant already has an allocation");

        _allocations.Add(allocation);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanJoin => Status == CampaignStatus.Active && CurrentParticipants < MaxParticipants;
    public bool IsSuccessful => Status == CampaignStatus.Completed && CurrentParticipants >= MinParticipants;
    public bool IsExpired => DateTime.UtcNow > EndDate;
    public decimal ProgressPercentage => TargetAmount > 0 ? (CurrentAmount / TargetAmount) * 100 : 0;

    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
