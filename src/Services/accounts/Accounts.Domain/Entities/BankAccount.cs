using TossErp.Shared.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;
using TossErp.Accounts.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// Bank Account entity for managing bank accounts
/// </summary>
[Table("BankAccounts")]
public class BankAccount : AggregateRoot
{
    public override Guid Id { get; protected set; }
    public override DateTime CreatedAt { get; protected set; }
    public override string CreatedBy { get; protected set; }
    
    [Required]
    [StringLength(100)]
    public string AccountName { get; private set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string AccountNumber { get; private set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string BankName { get; private set; } = string.Empty;

    [StringLength(20)]
    public string? BranchCode { get; private set; }

    [StringLength(20)]
    public string? SwiftCode { get; private set; }

    public Money CurrentBalance { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money AvailableBalance { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public bool IsActive { get; private set; } = true;

    public bool IsDefault { get; private set; } = false;

    [StringLength(500)]
    public string? Description { get; private set; }

    private BankAccount() : base() { }

    public BankAccount(
        Guid id,
        string tenantId,
        string accountName,
        string accountNumber,
        string bankName,
        string? branchCode = null,
        string? swiftCode = null,
        string? description = null,
        string? ModifiedBy = null)
    {
        Id = id;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = createdBy ?? "system";
        AccountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
        AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
        BankName = bankName ?? throw new ArgumentNullException(nameof(bankName));
        BranchCode = branchCode;
        SwiftCode = swiftCode;
        Description = description;
    }

    public static BankAccount Create(
        string tenantId,
        string accountName,
        string accountNumber,
        string bankName,
        string? branchCode = null,
        string? swiftCode = null,
        string? description = null,
        string? ModifiedBy = null)
    {
        return new BankAccount(
            Guid.NewGuid(),
            tenantId,
            accountName,
            accountNumber,
            bankName,
            branchCode,
            swiftCode,
            description,
            createdBy);
    }

    public void UpdateBalance(Money newBalance, string? updatedBy = null)
    {
        CurrentBalance = newBalance;
        AvailableBalance = newBalance; // Simplified for now
        MarkAsUpdated(updatedBy);
    }

    public void Activate(string? updatedBy = null)
    {
        IsActive = true;
        MarkAsUpdated(updatedBy);
    }

    public void Deactivate(string? updatedBy = null)
    {
        IsActive = false;
        MarkAsUpdated(updatedBy);
    }

    public void SetAsDefault(string? updatedBy = null)
    {
        IsDefault = true;
        MarkAsUpdated(updatedBy);
    }

    public void UnsetAsDefault(string? updatedBy = null)
    {
        IsDefault = false;
        MarkAsUpdated(updatedBy);
    }
}
