using TossErp.Stock.Domain.Events;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;

namespace TossErp.Stock.Domain.Entities;

public class StockEntryAdditionalCost : BaseEntity
{
    public StockEntryAggregate StockEntry { get; private set; } = null!;
    public string ExpenseAccount { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Amount { get; private set; }
    public string? Project { get; private set; }
    public string? CostCenter { get; private set; }
    public bool IsDisabled { get; private set; }

    private StockEntryAdditionalCost() { } // For EF Core

    public StockEntryAdditionalCost(StockEntryAggregate stockEntry, string expenseAccount, decimal amount)
    {
        StockEntry = stockEntry ?? throw new ArgumentNullException(nameof(stockEntry));
        ExpenseAccount = expenseAccount ?? throw new ArgumentNullException(nameof(expenseAccount));
        Amount = amount >= 0 ? amount : throw new ArgumentException("Amount cannot be negative", nameof(amount));
        IsDisabled = false;

        AddDomainEvent(new StockEntryAdditionalCostCreatedEvent(this));
    }

    public void UpdateInfo(string? description)
    {
        Description = description;
        AddDomainEvent(new StockEntryAdditionalCostUpdatedEvent(this));
    }

    public void UpdateAmount(decimal amount)
    {
        Amount = amount >= 0 ? amount : throw new ArgumentException("Amount cannot be negative", nameof(amount));
        AddDomainEvent(new StockEntryAdditionalCostAmountUpdatedEvent(this));
    }

    public void UpdateAccountSettings(string? project, string? costCenter)
    {
        Project = project;
        CostCenter = costCenter;
        AddDomainEvent(new StockEntryAdditionalCostAccountSettingsUpdatedEvent(this));
    }

    public void Disable()
    {
        IsDisabled = true;
        AddDomainEvent(new StockEntryAdditionalCostDisabledEvent(this));
    }

    public void Enable()
    {
        IsDisabled = false;
        AddDomainEvent(new StockEntryAdditionalCostEnabledEvent(this));
    }

    public bool IsDisabledCost()
    {
        return IsDisabled;
    }
} 
