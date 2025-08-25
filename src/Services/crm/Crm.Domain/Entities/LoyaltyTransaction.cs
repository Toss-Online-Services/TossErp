using Crm.Domain.Common;

namespace Crm.Domain.Entities;

public class LoyaltyTransaction : Entity
{
    public Guid CustomerId { get; private set; }
    public int Points { get; private set; }
    public LoyaltyTransactionType Type { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? RelatedOrderId { get; private set; }
    public DateTime? ExpiryDate { get; private set; }

    public LoyaltyTransaction(Guid customerId, int points, LoyaltyTransactionType type, string description, Guid? relatedOrderId = null)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Points = points;
        Type = type;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        CreatedAt = DateTime.UtcNow;
        RelatedOrderId = relatedOrderId;
        
        // Set expiry date for earned points (2 years from creation)
        if (type == LoyaltyTransactionType.Earned && points > 0)
        {
            ExpiryDate = CreatedAt.AddYears(2);
        }
    }

    private LoyaltyTransaction() 
    {
        Description = string.Empty;
    }

    public bool IsExpired => ExpiryDate.HasValue && DateTime.UtcNow > ExpiryDate.Value;
    
    public bool IsEarned => Type == LoyaltyTransactionType.Earned;
    
    public bool IsRedeemed => Type == LoyaltyTransactionType.Redeemed;
}

public enum LoyaltyTransactionType
{
    Earned,
    Redeemed,
    Expired,
    Bonus,
    Adjustment
}
