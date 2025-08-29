using TossErp.Shared.SeedWork;
using TossErp.Accounts.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// Chart of Accounts entity for the general ledger structure
/// </summary>
[Table("ChartOfAccounts")]
public class ChartOfAccount : AggregateRoot
{
    // Implement abstract properties from Entity<Guid>
    public override Guid Id { get; protected set; }
    public override DateTime CreatedAt { get; protected set; }
    public override string CreatedBy { get; protected set; } = string.Empty;

    // Additional audit properties
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }
    public Guid TenantId { get; private set; }

    [Required]
    [StringLength(20)]
    public string AccountCode { get; private set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string AccountName { get; private set; } = string.Empty;

    public AccountType AccountType { get; private set; }

    [StringLength(500)]
    public string? Description { get; private set; }

    public Guid? ParentAccountId { get; private set; }

    public bool IsActive { get; private set; } = true;

    public bool IsSystem { get; private set; } = false;

    public int Level { get; private set; } = 1;

    [StringLength(20)]
    public string? TaxCode { get; private set; }

    public bool AllowPostings { get; private set; } = true;

    // Navigation properties
    public virtual ChartOfAccount? ParentAccount { get; private set; }
    public virtual ICollection<ChartOfAccount> ChildAccounts { get; private set; } = new List<ChartOfAccount>();

    private ChartOfAccount() { }

    public ChartOfAccount(
        Guid id,
        Guid tenantId,
        string accountCode,
        string accountName,
        AccountType accountType,
        string? description = null,
        Guid? parentAccountId = null,
        string? createdBy = null)
    {
        Id = id;
        TenantId = tenantId;
        AccountCode = accountCode ?? throw new ArgumentNullException(nameof(accountCode));
        AccountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
        AccountType = accountType;
        Description = description;
        ParentAccountId = parentAccountId;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? string.Empty;
    }

    public static ChartOfAccount Create(
        Guid tenantId,
        string accountCode,
        string accountName,
        AccountType accountType,
        string? description = null,
        Guid? parentAccountId = null,
        string? createdBy = null)
    {
        return new ChartOfAccount(
            Guid.NewGuid(),
            tenantId,
            accountCode,
            accountName,
            accountType,
            description,
            parentAccountId,
            createdBy);
    }

    private void MarkAsUpdated(string? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateBasicInfo(
        string accountName,
        string? description = null,
        string? updatedBy = null)
    {
        AccountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
        Description = description;
        MarkAsUpdated(updatedBy);
    }

    public void SetParent(Guid? parentAccountId, string? updatedBy = null)
    {
        ParentAccountId = parentAccountId;
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
}
