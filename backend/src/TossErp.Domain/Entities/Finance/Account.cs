using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Finance;

public enum AccountType
{
    Asset,
    Liability,
    Equity,
    Revenue,
    Expense
}

public class Account : BaseEntity
{
    public string AccountCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public string? Description { get; set; }
    
    // Hierarchy
    public int? ParentAccountId { get; set; }
    public virtual Account? ParentAccount { get; set; }
    public virtual ICollection<Account> SubAccounts { get; set; } = new List<Account>();
    
    // Balance tracking
    public decimal CurrentBalance { get; set; }
    public string Currency { get; set; } = "ZAR";
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool IsSystem { get; set; } // System accounts cannot be deleted
    
    // Business logic
    public void Debit(decimal amount)
    {
        // Asset and Expense accounts increase with debits
        if (Type == AccountType.Asset || Type == AccountType.Expense)
            CurrentBalance += amount;
        else
            CurrentBalance -= amount;
            
        AddDomainEvent(new AccountDebitedEvent(Id, AccountCode, amount));
    }
    
    public void Credit(decimal amount)
    {
        // Liability, Equity, and Revenue accounts increase with credits
        if (Type == AccountType.Liability || Type == AccountType.Equity || Type == AccountType.Revenue)
            CurrentBalance += amount;
        else
            CurrentBalance -= amount;
            
        AddDomainEvent(new AccountCreditedEvent(Id, AccountCode, amount));
    }
}

// Domain Events
public class AccountDebitedEvent : DomainEvent
{
    public int AccountId { get; }
    public string AccountCode { get; }
    public decimal Amount { get; }
    
    public AccountDebitedEvent(int accountId, string accountCode, decimal amount)
    {
        AccountId = accountId;
        AccountCode = accountCode;
        Amount = amount;
    }
}

public class AccountCreditedEvent : DomainEvent
{
    public int AccountId { get; }
    public string AccountCode { get; }
    public decimal Amount { get; }
    
    public AccountCreditedEvent(int accountId, string accountCode, decimal amount)
    {
        AccountId = accountId;
        AccountCode = accountCode;
        Amount = amount;
    }
}

