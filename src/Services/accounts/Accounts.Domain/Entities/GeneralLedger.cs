using TossErp.Shared.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;
using TossErp.Accounts.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// General Ledger entity for maintaining account balances
/// </summary>
[Table("GeneralLedger")]
public class GeneralLedger : AggregateRoot
{
    public override Guid Id { get; protected set; }
    public override DateTime CreatedAt { get; protected set; }
    public override string CreatedBy { get; protected set; }

    public string TenantId { get; private set; } = string.Empty;

    public Guid ChartOfAccountId { get; private set; }

    public DateOnly PeriodDate { get; private set; }

    public Money DebitBalance { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money CreditBalance { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money NetBalance { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money OpeningBalance { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money ClosingBalance { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    // Navigation properties
    public virtual ChartOfAccount? ChartOfAccount { get; private set; }

    private GeneralLedger() : base() { }

    public GeneralLedger(
        Guid id,
        string tenantId,
        Guid chartOfAccountId,
        DateOnly periodDate,
        Money openingBalance,
        string? ModifiedBy = null)
    {
        ChartOfAccountId = chartOfAccountId;
        PeriodDate = periodDate;
        OpeningBalance = openingBalance;
        ClosingBalance = openingBalance;
        NetBalance = openingBalance;
        ModifiedBy = createdBy;
    }

    public static GeneralLedger Create(
        string tenantId,
        Guid chartOfAccountId,
        DateOnly periodDate,
        Money openingBalance,
        string? ModifiedBy = null)
    {
        return new GeneralLedger(
            Guid.NewGuid(),
            tenantId,
            chartOfAccountId,
            periodDate,
            openingBalance,
            createdBy);
    }

    public void AddDebitTransaction(Money amount, string? updatedBy = null)
    {
        DebitBalance = DebitBalance.Add(amount);
        CalculateNetBalance();
        MarkAsUpdated(updatedBy);
    }

    public void AddCreditTransaction(Money amount, string? updatedBy = null)
    {
        CreditBalance = CreditBalance.Add(amount);
        CalculateNetBalance();
        MarkAsUpdated(updatedBy);
    }

    private void CalculateNetBalance()
    {
        NetBalance = DebitBalance.Subtract(CreditBalance);
        ClosingBalance = OpeningBalance.Add(NetBalance);
    }

    public void CloseAccount(string? updatedBy = null)
    {
        CalculateNetBalance();
        MarkAsUpdated(updatedBy);
    }
}
