using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Sales;

public enum CustomerType
{
    Individual,
    Business
}

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public CustomerType Type { get; set; } = CustomerType.Individual;
    
    // Contact information
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AlternatePhone { get; set; }
    
    // Address
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; } = "South Africa";
    
    // Business details (for business customers)
    public string? CompanyName { get; set; }
    public string? TaxNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    
    // Financial
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public int PaymentTermsDays { get; set; } = 30;
    
    // Loyalty
    public int LoyaltyPoints { get; set; }
    public string? LoyaltyTier { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool IsVip { get; set; }
    
    // Preferences
    public string? PreferredPaymentMethod { get; set; }
    public string? PreferredLanguage { get; set; } = "English";
    public bool ReceiveMarketing { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    
    // Business logic
    public void AddLoyaltyPoints(int points)
    {
        if (points < 0)
            throw new ArgumentException("Points must be positive", nameof(points));
        
        LoyaltyPoints += points;
        AddDomainEvent(new LoyaltyPointsAddedEvent(Id, Name, points));
    }
    
    public void RedeemLoyaltyPoints(int points)
    {
        if (points > LoyaltyPoints)
            throw new InvalidOperationException($"Cannot redeem {points} points. Only {LoyaltyPoints} available.");
        
        LoyaltyPoints -= points;
        AddDomainEvent(new LoyaltyPointsRedeemedEvent(Id, Name, points));
    }
}

// Domain Events
public class LoyaltyPointsAddedEvent : DomainEvent
{
    public int CustomerId { get; }
    public string CustomerName { get; }
    public int Points { get; }
    
    public LoyaltyPointsAddedEvent(int customerId, string customerName, int points)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        Points = points;
    }
}

public class LoyaltyPointsRedeemedEvent : DomainEvent
{
    public int CustomerId { get; }
    public string CustomerName { get; }
    public int Points { get; }
    
    public LoyaltyPointsRedeemedEvent(int customerId, string customerName, int points)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        Points = points;
    }
}

