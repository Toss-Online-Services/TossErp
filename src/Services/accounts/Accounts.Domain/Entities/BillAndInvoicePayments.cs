using TossErp.Shared.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;
using TossErp.Accounts.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// Bill Line entity for bill line items
/// </summary>
[Table("BillLines")]
public class BillLine : Entity
{
    public Guid BillId { get; private set; }

    [Required]
    [StringLength(200)]
    public string Description { get; private set; } = string.Empty;

    public decimal Quantity { get; private set; }

    public Money UnitCost { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money LineTotal { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Guid? ChartOfAccountId { get; private set; }

    [StringLength(20)]
    public string? TaxCode { get; private set; }

    public decimal TaxRate { get; private set; } = 0;

    public Money TaxAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    // Navigation properties
    public virtual Bill? Bill { get; private set; }
    public virtual ChartOfAccount? ChartOfAccount { get; private set; }

    private BillLine() : base() { }

    public BillLine(
        Guid id,
        string tenantId,
        Guid billId,
        string description,
        decimal quantity,
        Money unitCost,
        Guid? chartOfAccountId = null,
        string? taxCode = null,
        decimal taxRate = 0,
        string? createdBy = null) : base(id, tenantId)
    {
        BillId = billId;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Quantity = quantity;
        UnitCost = unitCost;
        ChartOfAccountId = chartOfAccountId;
        TaxCode = taxCode;
        TaxRate = taxRate;
        CalculateAmounts();
        CreatedBy = createdBy;
    }

    public static BillLine Create(
        string tenantId,
        Guid billId,
        string description,
        decimal quantity,
        Money unitCost,
        Guid? chartOfAccountId = null,
        string? taxCode = null,
        decimal taxRate = 0,
        string? createdBy = null)
    {
        return new BillLine(
            Guid.NewGuid(),
            tenantId,
            billId,
            description,
            quantity,
            unitCost,
            chartOfAccountId,
            taxCode,
            taxRate,
            createdBy);
    }

    private void CalculateAmounts()
    {
        LineTotal = UnitCost.Multiply(Quantity);
        TaxAmount = LineTotal.Multiply(TaxRate);
    }

    public void UpdateQuantityAndCost(decimal quantity, Money unitCost, string? updatedBy = null)
    {
        Quantity = quantity;
        UnitCost = unitCost;
        CalculateAmounts();
        MarkAsUpdated(updatedBy);
    }
}

/// <summary>
/// Bill Payment entity for bill payments
/// </summary>
[Table("BillPayments")]
public class BillPayment : Entity
{
    public Guid BillId { get; private set; }

    public Guid PaymentId { get; private set; }

    public Money AmountAllocated { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public DateTime AllocationDate { get; private set; }

    [StringLength(500)]
    public string? Notes { get; private set; }

    // Navigation properties
    public virtual Bill? Bill { get; private set; }
    public virtual Payment? Payment { get; private set; }

    private BillPayment() : base() { }

    public BillPayment(
        Guid id,
        string tenantId,
        Guid billId,
        Guid paymentId,
        Money amountAllocated,
        DateTime allocationDate,
        string? notes = null,
        string? createdBy = null) : base(id, tenantId)
    {
        BillId = billId;
        PaymentId = paymentId;
        AmountAllocated = amountAllocated;
        AllocationDate = allocationDate;
        Notes = notes;
        CreatedBy = createdBy;
    }

    public static BillPayment Create(
        string tenantId,
        Guid billId,
        Guid paymentId,
        Money amountAllocated,
        DateTime allocationDate,
        string? notes = null,
        string? createdBy = null)
    {
        return new BillPayment(
            Guid.NewGuid(),
            tenantId,
            billId,
            paymentId,
            amountAllocated,
            allocationDate,
            notes,
            createdBy);
    }
}

/// <summary>
/// Invoice Payment entity for invoice payments
/// </summary>
[Table("InvoicePayments")]
public class InvoicePayment : Entity
{
    public Guid InvoiceId { get; private set; }

    public Guid PaymentId { get; private set; }

    public Money AmountAllocated { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public DateTime AllocationDate { get; private set; }

    [StringLength(500)]
    public string? Notes { get; private set; }

    // Navigation properties
    public virtual Invoice? Invoice { get; private set; }
    public virtual Payment? Payment { get; private set; }

    private InvoicePayment() : base() { }

    public InvoicePayment(
        Guid id,
        string tenantId,
        Guid invoiceId,
        Guid paymentId,
        Money amountAllocated,
        DateTime allocationDate,
        string? notes = null,
        string? createdBy = null) : base(id, tenantId)
    {
        InvoiceId = invoiceId;
        PaymentId = paymentId;
        AmountAllocated = amountAllocated;
        AllocationDate = allocationDate;
        Notes = notes;
        CreatedBy = createdBy;
    }

    public static InvoicePayment Create(
        string tenantId,
        Guid invoiceId,
        Guid paymentId,
        Money amountAllocated,
        DateTime allocationDate,
        string? notes = null,
        string? createdBy = null)
    {
        return new InvoicePayment(
            Guid.NewGuid(),
            tenantId,
            invoiceId,
            paymentId,
            amountAllocated,
            allocationDate,
            notes,
            createdBy);
    }
}
