using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Events;
using TossErp.Stock.Domain.Events;

namespace TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;

public class ItemTax : BaseAuditableEntity
{
    public Guid ItemId { get; private set; }
    public string TaxCode { get; private set; } = string.Empty;
    public string TaxName { get; private set; } = string.Empty;
    public decimal TaxRate { get; private set; }
    public bool IsActive { get; private set; } = true;

    private ItemTax() { } // For EF Core

    public ItemTax(Guid itemId, string taxCode, string taxName, decimal taxRate)
    {
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty.", nameof(itemId));

        if (string.IsNullOrWhiteSpace(taxCode))
            throw new ArgumentException("Tax code cannot be empty.", nameof(taxCode));

        if (string.IsNullOrWhiteSpace(taxName))
            throw new ArgumentException("Tax name cannot be empty.", nameof(taxName));

        if (taxRate < 0)
            throw new ArgumentException("Tax rate cannot be negative.", nameof(taxRate));

        ItemId = itemId;
        TaxCode = taxCode.Trim().ToUpperInvariant();
        TaxName = taxName.Trim();
        TaxRate = taxRate;

        AddDomainEvent(new ItemTaxCreatedEvent(this));
    }

    public void UpdateTaxInfo(string taxName, decimal taxRate)
    {
        if (string.IsNullOrWhiteSpace(taxName))
            throw new ArgumentException("Tax name cannot be empty.", nameof(taxName));

        if (taxRate < 0)
            throw new ArgumentException("Tax rate cannot be negative.", nameof(taxRate));

        TaxName = taxName.Trim();
        TaxRate = taxRate;
        AddDomainEvent(new ItemTaxUpdatedEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        AddDomainEvent(new ItemTaxDeactivatedEvent(this));
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        AddDomainEvent(new ItemTaxActivatedEvent(this));
    }

    public void Delete()
    {
        AddDomainEvent(new ItemTaxDeletedEvent(this));
    }
} 
