using TossErp.Domain.SeedWork;
using TossErp.Domain.Events;

namespace TossErp.Domain.AggregatesModel.GroupPurchaseAggregate;

public class GroupPurchase : Entity, IAggregateRoot
{
    public string GroupNumber { get; private set; } = string.Empty;
    public Guid BusinessId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public decimal GroupPrice { get; private set; } // Discounted price when buying in bulk
    public int MinimumQuantity { get; private set; }
    public int TargetQuantity { get; private set; }
    public int CurrentQuantity { get; private set; }
    public string Status { get; private set; } = string.Empty; // "open", "active", "completed", "cancelled"
    public DateTime ExpiryDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public string? Description { get; private set; }
    public string? DeliveryLocation { get; private set; }
    public DateTime? DeliveryDate { get; private set; }

    private readonly List<GroupPurchaseMember> _members;
    public IReadOnlyCollection<GroupPurchaseMember> Members => _members.AsReadOnly();

    protected GroupPurchase()
    {
        _members = new List<GroupPurchaseMember>();
        GroupNumber = string.Empty;
        ProductName = string.Empty;
        Status = "open";
        CreatedAt = DateTime.UtcNow;
    }

    public GroupPurchase(Guid businessId, Guid productId, string productName, decimal unitPrice, 
                        decimal groupPrice, int minimumQuantity, int targetQuantity, DateTime expiryDate,
                        Guid createdBy, string? description = null, string? deliveryLocation = null) : this()
    {
        Id = Guid.NewGuid();
        GroupNumber = GenerateGroupNumber();
        BusinessId = businessId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        GroupPrice = groupPrice;
        MinimumQuantity = minimumQuantity;
        TargetQuantity = targetQuantity;
        CurrentQuantity = 0;
        Status = "open";
        ExpiryDate = expiryDate;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        Description = description;
        DeliveryLocation = deliveryLocation;

        AddDomainEvent(new GroupPurchaseCreatedDomainEvent(this));
    }

    public void AddMember(Guid userId, int quantity, string? notes = null)
    {
        if (Status != "open" && Status != "active")
            throw new InvalidOperationException("Cannot add members to a closed group purchase");

        if (CurrentQuantity + quantity > TargetQuantity)
            throw new InvalidOperationException("Adding this quantity would exceed the target quantity");

        var member = new GroupPurchaseMember(Id, userId, quantity, notes);
        _members.Add(member);
        CurrentQuantity += quantity;

        if (Status == "open" && CurrentQuantity >= MinimumQuantity)
        {
            Status = "active";
            AddDomainEvent(new GroupPurchaseActivatedDomainEvent(this));
        }

        if (CurrentQuantity >= TargetQuantity)
        {
            CompleteGroupPurchase();
        }

        AddDomainEvent(new GroupPurchaseMemberAddedDomainEvent(this, member));
    }

    public void RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member != null)
        {
            CurrentQuantity -= member.Quantity;
            _members.Remove(member);

            if (Status == "active" && CurrentQuantity < MinimumQuantity)
            {
                Status = "open";
            }

            AddDomainEvent(new GroupPurchaseMemberRemovedDomainEvent(this, member));
        }
    }

    public void UpdateMemberQuantity(Guid userId, int newQuantity)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member != null)
        {
            var quantityDifference = newQuantity - member.Quantity;
            
            if (CurrentQuantity + quantityDifference > TargetQuantity)
                throw new InvalidOperationException("This quantity would exceed the target quantity");

            member.UpdateQuantity(newQuantity);
            CurrentQuantity += quantityDifference;

            if (CurrentQuantity >= TargetQuantity)
            {
                CompleteGroupPurchase();
            }
        }
    }

    public void SetDeliveryDetails(string deliveryLocation, DateTime deliveryDate)
    {
        DeliveryLocation = deliveryLocation;
        DeliveryDate = deliveryDate;
    }

    public void CancelGroupPurchase(string reason = "")
    {
        Status = "cancelled";
        AddDomainEvent(new GroupPurchaseCancelledDomainEvent(this, reason));
    }

    private void CompleteGroupPurchase()
    {
        Status = "completed";
        CompletedAt = DateTime.UtcNow;
        AddDomainEvent(new GroupPurchaseCompletedDomainEvent(this));
    }

    private string GenerateGroupNumber()
    {
        return $"GP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiryDate;
    
    public bool CanJoin() => Status == "open" || (Status == "active" && CurrentQuantity < TargetQuantity);
    
    public decimal GetTotalSavings() => (UnitPrice - GroupPrice) * CurrentQuantity;
    
    public int GetRemainingQuantity() => TargetQuantity - CurrentQuantity;
    
    public double GetProgressPercentage() => (double)CurrentQuantity / TargetQuantity * 100;
} 
