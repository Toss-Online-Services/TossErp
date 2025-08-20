using Collaboration.Domain.Common;
using Collaboration.Domain.Enums;

namespace Collaboration.Domain.Entities;

/// <summary>
/// Represents the allocation of campaign benefits to participants after completion
/// </summary>
public class CampaignAllocation : Entity
{
    public Guid CampaignId { get; private set; }
    public Guid ParticipantId { get; private set; }
    public AllocationType Type { get; private set; }
    public decimal AllocatedAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal FinalAmount { get; private set; }
    public int AllocatedQuantity { get; private set; }
    public string? ProductDetails { get; private set; }
    public string? Notes { get; private set; }
    public AllocationStatus Status { get; private set; }
    public DateTime AllocatedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? SettledAt { get; private set; }
    public Guid AllocatedBy { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public Campaign Campaign { get; private set; } = null!;
    public CampaignParticipant Participant { get; private set; } = null!;

    // Private constructor for EF Core
    private CampaignAllocation() { }

    public CampaignAllocation(
        Guid campaignId,
        Guid participantId,
        AllocationType type,
        decimal allocatedAmount,
        decimal discountAmount,
        int allocatedQuantity,
        string? productDetails,
        Guid allocatedBy,
        Guid tenantId)
    {
        if (campaignId == Guid.Empty)
            throw new ArgumentException("Campaign ID cannot be empty", nameof(campaignId));

        if (participantId == Guid.Empty)
            throw new ArgumentException("Participant ID cannot be empty", nameof(participantId));

        if (allocatedAmount < 0)
            throw new ArgumentException("Allocated amount cannot be negative", nameof(allocatedAmount));

        if (discountAmount < 0)
            throw new ArgumentException("Discount amount cannot be negative", nameof(discountAmount));

        if (allocatedQuantity <= 0)
            throw new ArgumentException("Allocated quantity must be greater than 0", nameof(allocatedQuantity));

        CampaignId = campaignId;
        ParticipantId = participantId;
        Type = type;
        AllocatedAmount = allocatedAmount;
        DiscountAmount = discountAmount;
        FinalAmount = allocatedAmount - discountAmount;
        AllocatedQuantity = allocatedQuantity;
        ProductDetails = productDetails;
        Status = AllocationStatus.Allocated;
        AllocatedAt = DateTime.UtcNow;
        AllocatedBy = allocatedBy;
        TenantId = tenantId;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateAllocation(decimal newAmount, decimal newDiscount, int newQuantity, string? productDetails)
    {
        if (Status != AllocationStatus.Allocated)
            throw new InvalidOperationException("Cannot update allocation after it has been delivered or settled");

        if (newAmount < 0)
            throw new ArgumentException("Allocated amount cannot be negative", nameof(newAmount));

        if (newDiscount < 0)
            throw new ArgumentException("Discount amount cannot be negative", nameof(newDiscount));

        if (newQuantity <= 0)
            throw new ArgumentException("Allocated quantity must be greater than 0", nameof(newQuantity));

        AllocatedAmount = newAmount;
        DiscountAmount = newDiscount;
        FinalAmount = newAmount - newDiscount;
        AllocatedQuantity = newQuantity;
        ProductDetails = productDetails;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDelivered()
    {
        if (Status != AllocationStatus.Allocated)
            throw new InvalidOperationException("Allocation must be in allocated status to mark as delivered");

        Status = AllocationStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsSettled()
    {
        if (Status != AllocationStatus.Delivered)
            throw new ArgumentException("Allocation must be delivered before it can be settled");

        Status = AllocationStatus.Settled;
        SettledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddNotes(string notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
            throw new ArgumentException("Notes cannot be empty", nameof(notes));

        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsDelivered => Status == AllocationStatus.Delivered;
    public bool IsSettled => Status == AllocationStatus.Settled;
    public bool CanUpdate => Status == AllocationStatus.Allocated;
    public bool CanDeliver => Status == AllocationStatus.Allocated;
    public bool CanSettle => Status == AllocationStatus.Delivered;
    public decimal TotalValue => AllocatedAmount * AllocatedQuantity;
    public decimal TotalDiscount => DiscountAmount * AllocatedQuantity;
    public decimal TotalFinalValue => FinalAmount * AllocatedQuantity;
}
