using TossErp.Domain.Common;
using TossErp.Domain.Events.Collaboration;

namespace TossErp.Domain.Entities.Collaboration;

/// <summary>
/// Community credit pool for shared financing
/// </summary>
public class CreditPool : BaseEntity
{
    public string PoolNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CreditPoolStatus Status { get; set; } = CreditPoolStatus.Forming;
    
    // Administrator
    public int AdministratorId { get; set; }
    public string AdministratorName { get; set; } = string.Empty;
    
    // Fund Details
    public decimal TotalFund { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal AvailableAmount => TotalFund - AllocatedAmount;
    public decimal OutstandingAmount { get; set; }
    public decimal RepaidAmount { get; set; }
    
    // Terms
    public decimal InterestRate { get; set; } // Annual percentage
    public int DefaultTermMonths { get; set; }
    public int MaximumTermMonths { get; set; }
    public decimal MaximumLoanAmount { get; set; }
    public decimal MinimumLoanAmount { get; set; }
    
    // Membership
    public int TotalMembers { get; set; }
    public int ActiveBorrowers { get; set; }
    public decimal MembershipFee { get; set; }
    public decimal? MinimumContribution { get; set; }
    
    // Risk Management
    public decimal DefaultRate { get; set; } // Percentage
    public decimal? ReserveRequirement { get; set; } // Percentage
    public decimal ReserveFund { get; set; }
    
    // Performance
    public decimal TotalLoansIssued { get; set; }
    public int LoanCount { get; set; }
    public decimal TotalInterestEarned { get; set; }
    public decimal AverageRecoveryRate { get; set; }
    
    // Rules
    public string? EligibilityCriteria { get; set; } // JSON
    public string? LendingRules { get; set; }
    public string? CollateralRequirements { get; set; }
    
    // Metadata
    public DateTime? LaunchDate { get; set; }
    public string? Notes { get; set; }
    
    // Navigation Properties
    public ICollection<CreditPoolMember> Members { get; set; } = new List<CreditPoolMember>();
    public ICollection<CreditAllocation> Allocations { get; set; } = new List<CreditAllocation>();
    
    // Business Methods
    public void Activate()
    {
        if (TotalFund == 0)
            throw new InvalidOperationException("Credit pool must have funds");
        
        Status = CreditPoolStatus.Active;
        LaunchDate = DateTime.UtcNow;
        AddDomainEvent(new CreditPoolCreated(Id, Name, TotalFund));
    }
    
    public void AllocateCredit(int borrowerId, decimal amount)
    {
        if (Status != CreditPoolStatus.Active)
            throw new InvalidOperationException("Credit pool must be active");
        
        if (amount > AvailableAmount)
            throw new InvalidOperationException($"Insufficient funds. Available: {AvailableAmount}");
        
        AllocatedAmount += amount;
        OutstandingAmount += amount;
        ActiveBorrowers++;
    }
}

public enum CreditPoolStatus
{
    Forming,
    Active,
    Suspended,
    Closed
}

