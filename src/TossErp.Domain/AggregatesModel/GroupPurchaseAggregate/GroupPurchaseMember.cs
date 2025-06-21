using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.GroupPurchaseAggregate;

public class GroupPurchaseMember : Entity
{
    public Guid GroupPurchaseId { get; private set; }
    public Guid UserId { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Status { get; private set; } = string.Empty; // "pending", "confirmed", "paid", "cancelled"
    public DateTime JoinedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? Notes { get; private set; }
    public string? PaymentMethod { get; private set; }
    public string? PaymentReference { get; private set; }

    protected GroupPurchaseMember() 
    {
        Status = "pending";
        JoinedAt = DateTime.UtcNow;
    }

    public GroupPurchaseMember(Guid groupPurchaseId, Guid userId, int quantity, string? notes = null)
    {
        Id = Guid.NewGuid();
        GroupPurchaseId = groupPurchaseId;
        UserId = userId;
        Quantity = quantity;
        Status = "pending";
        JoinedAt = DateTime.UtcNow;
        Notes = notes;
        TotalAmount = 0; // Will be calculated when group purchase is confirmed
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }

    public void Confirm(decimal unitPrice)
    {
        Status = "confirmed";
        ConfirmedAt = DateTime.UtcNow;
        TotalAmount = Quantity * unitPrice;
    }

    public void MarkAsPaid(string paymentMethod, string paymentReference)
    {
        Status = "paid";
        PaidAt = DateTime.UtcNow;
        PaymentMethod = paymentMethod;
        PaymentReference = paymentReference;
    }

    public void Cancel()
    {
        Status = "cancelled";
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes;
    }
} 
