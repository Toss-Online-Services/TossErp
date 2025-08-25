using Crm.Domain.Common;

namespace Crm.Domain.Entities;

public class Customer : Entity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public DateTime DateOfBirth { get; private set; }
    public CustomerStatus Status { get; private set; }
    public CustomerSegment Segment { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastPurchaseDate { get; private set; }
    public decimal TotalSpent { get; private set; }
    public int PurchaseCount { get; private set; }
    public int LoyaltyPoints { get; private set; }
    
    private readonly List<CustomerInteraction> _interactions = new();
    public IReadOnlyCollection<CustomerInteraction> Interactions => _interactions.AsReadOnly();

    private readonly List<LoyaltyTransaction> _loyaltyTransactions = new();
    public IReadOnlyCollection<LoyaltyTransaction> LoyaltyTransactions => _loyaltyTransactions.AsReadOnly();

    // Constructor for new customers
    public Customer(string firstName, string lastName, string email, string phone, string address, DateTime dateOfBirth)
    {
        Id = Guid.NewGuid();
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        DateOfBirth = dateOfBirth;
        Status = CustomerStatus.Active;
        Segment = CustomerSegment.Regular;
        CreatedAt = DateTime.UtcNow;
        TotalSpent = 0;
        PurchaseCount = 0;
        LoyaltyPoints = 0;
    }

    // Constructor for existing customers (from database)
    private Customer() { }

    public void UpdateProfile(string firstName, string lastName, string email, string phone, string address)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }

    public void RecordPurchase(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Purchase amount must be positive", nameof(amount));

        TotalSpent += amount;
        PurchaseCount++;
        LastPurchaseDate = DateTime.UtcNow;
        
        // Award loyalty points (1 point per R1 spent)
        var pointsEarned = (int)amount;
        AddLoyaltyPoints(pointsEarned);
        
        // Update segment based on total spent
        UpdateSegment();
    }

    public void AddLoyaltyPoints(int points)
    {
        if (points <= 0)
            throw new ArgumentException("Points must be positive", nameof(points));

        LoyaltyPoints += points;
        _loyaltyTransactions.Add(new LoyaltyTransaction(Id, points, LoyaltyTransactionType.Earned, "Purchase reward"));
    }

    public void RedeemLoyaltyPoints(int points, string reason)
    {
        if (points <= 0)
            throw new ArgumentException("Points must be positive", nameof(points));
        
        if (points > LoyaltyPoints)
            throw new InvalidOperationException("Insufficient loyalty points");

        LoyaltyPoints -= points;
        _loyaltyTransactions.Add(new LoyaltyTransaction(Id, -points, LoyaltyTransactionType.Redeemed, reason));
    }

    public void AddInteraction(CustomerInteraction interaction)
    {
        if (interaction == null)
            throw new ArgumentNullException(nameof(interaction));

        _interactions.Add(interaction);
    }

    public void UpdateStatus(CustomerStatus status)
    {
        Status = status;
    }

    private void UpdateSegment()
    {
        Segment = TotalSpent switch
        {
            >= 10000 => CustomerSegment.Premium,
            >= 5000 => CustomerSegment.Gold,
            >= 1000 => CustomerSegment.Silver,
            _ => CustomerSegment.Regular
        };
    }

    public string FullName => $"{FirstName} {LastName}";
    
    public bool IsLapsed => LastPurchaseDate.HasValue && 
                           DateTime.UtcNow - LastPurchaseDate.Value > TimeSpan.FromDays(90);
    
    public bool IsHighValue => TotalSpent >= 5000;
}

public enum CustomerStatus
{
    Active,
    Inactive,
    Suspended,
    Deleted
}

public enum CustomerSegment
{
    Regular,
    Silver,
    Gold,
    Premium
}
