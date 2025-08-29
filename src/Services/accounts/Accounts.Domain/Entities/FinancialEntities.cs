using TossErp.Accounts.Domain.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;
using TossErp.Accounts.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// Expense entity for tracking business expenses
/// </summary>
[Table("Expenses")]
public class Expense : AggregateRoot
{
    [Required]
    [StringLength(200)]
    public string Description { get; private set; } = string.Empty;

    public Money Amount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public DateOnly ExpenseDate { get; private set; }

    public Guid? VendorId { get; private set; }

    public Guid? ChartOfAccountId { get; private set; }

    [StringLength(50)]
    public string? ReferenceNumber { get; private set; }

    [StringLength(20)]
    public string? TaxCode { get; private set; }

    public decimal TaxRate { get; private set; } = 0;

    public Money TaxAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public ExpenseStatus Status { get; private set; } = ExpenseStatus.Draft;

    [StringLength(500)]
    public string? Notes { get; private set; }

    // Navigation properties
    public virtual Vendor? Vendor { get; private set; }
    public virtual ChartOfAccount? ChartOfAccount { get; private set; }

    private Expense() : base() { }

    public Expense(
        Guid id,
        string tenantId,
        string description,
        Money amount,
        DateOnly expenseDate,
        Guid? vendorId = null,
        Guid? chartOfAccountId = null,
        string? referenceNumber = null,
        string? taxCode = null,
        decimal taxRate = 0,
        string? notes = null,
        string? createdBy = null) : base(id, tenantId)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Amount = amount;
        ExpenseDate = expenseDate;
        VendorId = vendorId;
        ChartOfAccountId = chartOfAccountId;
        ReferenceNumber = referenceNumber;
        TaxCode = taxCode;
        TaxRate = taxRate;
        Notes = notes;
        CalculateTax();
        CreatedBy = createdBy;
    }

    public static Expense Create(
        string tenantId,
        string description,
        Money amount,
        DateOnly expenseDate,
        Guid? vendorId = null,
        Guid? chartOfAccountId = null,
        string? referenceNumber = null,
        string? taxCode = null,
        decimal taxRate = 0,
        string? notes = null,
        string? createdBy = null)
    {
        return new Expense(
            Guid.NewGuid(),
            tenantId,
            description,
            amount,
            expenseDate,
            vendorId,
            chartOfAccountId,
            referenceNumber,
            taxCode,
            taxRate,
            notes,
            createdBy);
    }

    private void CalculateTax()
    {
        TaxAmount = Amount.Multiply(TaxRate);
    }

    public void UpdateAmount(Money amount, string? updatedBy = null)
    {
        Amount = amount;
        CalculateTax();
        MarkAsUpdated(updatedBy);
    }

    public void Approve(string? updatedBy = null)
    {
        Status = ExpenseStatus.Approved;
        MarkAsUpdated(updatedBy);
    }

    public void Reject(string? updatedBy = null)
    {
        Status = ExpenseStatus.Rejected;
        MarkAsUpdated(updatedBy);
    }
}

/// <summary>
/// Financial Period entity for managing accounting periods
/// </summary>
[Table("FinancialPeriods")]
public class FinancialPeriod : AggregateRoot
{
    [Required]
    [StringLength(100)]
    public string Name { get; private set; } = string.Empty;

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

    public bool IsClosed { get; private set; } = false;

    public bool IsActive { get; private set; } = true;

    [StringLength(500)]
    public string? Description { get; private set; }

    private FinancialPeriod() : base() { }

    public FinancialPeriod(
        Guid id,
        string tenantId,
        string name,
        DateOnly startDate,
        DateOnly endDate,
        string? description = null,
        string? createdBy = null) : base(id, tenantId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
        CreatedBy = createdBy;

        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date");
    }

    public static FinancialPeriod Create(
        string tenantId,
        string name,
        DateOnly startDate,
        DateOnly endDate,
        string? description = null,
        string? createdBy = null)
    {
        return new FinancialPeriod(
            Guid.NewGuid(),
            tenantId,
            name,
            startDate,
            endDate,
            description,
            createdBy);
    }

    public void Close(string? updatedBy = null)
    {
        IsClosed = true;
        MarkAsUpdated(updatedBy);
    }

    public void Open(string? updatedBy = null)
    {
        IsClosed = false;
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

/// <summary>
/// Budget Entry entity for budgeting
/// </summary>
[Table("BudgetEntries")]
public class BudgetEntry : Entity
{
    public Guid FinancialPeriodId { get; private set; }

    public Guid ChartOfAccountId { get; private set; }

    public Money BudgetedAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money ActualAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money VarianceAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    [StringLength(500)]
    public string? Notes { get; private set; }

    // Navigation properties
    public virtual FinancialPeriod? FinancialPeriod { get; private set; }
    public virtual ChartOfAccount? ChartOfAccount { get; private set; }

    private BudgetEntry() : base() { }

    public BudgetEntry(
        Guid id,
        string tenantId,
        Guid financialPeriodId,
        Guid chartOfAccountId,
        Money budgetedAmount,
        string? notes = null,
        string? createdBy = null) : base(id, tenantId)
    {
        FinancialPeriodId = financialPeriodId;
        ChartOfAccountId = chartOfAccountId;
        BudgetedAmount = budgetedAmount;
        Notes = notes;
        CalculateVariance();
        CreatedBy = createdBy;
    }

    public static BudgetEntry Create(
        string tenantId,
        Guid financialPeriodId,
        Guid chartOfAccountId,
        Money budgetedAmount,
        string? notes = null,
        string? createdBy = null)
    {
        return new BudgetEntry(
            Guid.NewGuid(),
            tenantId,
            financialPeriodId,
            chartOfAccountId,
            budgetedAmount,
            notes,
            createdBy);
    }

    public void UpdateActualAmount(Money actualAmount, string? updatedBy = null)
    {
        ActualAmount = actualAmount;
        CalculateVariance();
        MarkAsUpdated(updatedBy);
    }

    private void CalculateVariance()
    {
        VarianceAmount = BudgetedAmount.Subtract(ActualAmount);
    }
}
