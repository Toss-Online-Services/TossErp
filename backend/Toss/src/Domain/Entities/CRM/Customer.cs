using Toss.Domain.Common;
using Toss.Domain.Entities;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Entities.Stores;
using Toss.Domain.ValueObjects;

namespace Toss.Domain.Entities.CRM;

public class Customer : BaseAuditableEntity, IBusinessScopedEntity
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    public int? StoreId { get; set; }
    public Store? Store { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    public PhoneNumber? Phone { get; set; }
    public string? Email { get; set; }
    
    public int? AddressId { get; set; }
    public Address? Address { get; set; }
    
    public bool IsActive { get; set; } = true;
    public bool AllowsMarketing { get; set; }
    public decimal CreditLimit { get; set; }
    public string? Tags { get; set; }
    
    // Customer insights
    public decimal TotalPurchaseAmount { get; set; }
    public int TotalPurchaseCount { get; set; }
    public DateTimeOffset? LastPurchaseDate { get; set; }
    public DateTimeOffset? FirstPurchaseDate { get; set; }
    
    // Aliases for handlers
    public decimal TotalPurchases
    {
        get => TotalPurchaseAmount;
        set => TotalPurchaseAmount = value;
    }
    
    public string? Notes { get; set; }
    
    // Relationships
    public ICollection<Sale> Sales { get; private set; } = new List<Sale>();
    public ICollection<CustomerPurchase> PurchaseHistory { get; private set; } = new List<CustomerPurchase>();
    public ICollection<CustomerInteraction> Interactions { get; private set; } = new List<CustomerInteraction>();
    
    // Alias for handlers
    public ICollection<CustomerPurchase> Purchases => PurchaseHistory;
}

