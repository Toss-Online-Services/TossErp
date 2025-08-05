using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Events;

namespace TossErp.Stock.Domain.Entities;

public class ItemCustomer : BaseAuditableEntity
{
    public Guid ItemId { get; private set; }
    public string Customer { get; private set; } = string.Empty;
    public string? CustomerItemCode { get; private set; }
    public string? CustomerItemName { get; private set; }
    public bool IsDefaultCustomer { get; private set; }
    public bool IsPreferredCustomer { get; private set; }
    public decimal? RefRate { get; private set; }
    public string? RefUOM { get; private set; }
    public decimal? ConversionFactor { get; private set; }
    public bool IsDisabled { get; private set; }

    private ItemCustomer() { } // For EF Core

    public ItemCustomer(Guid itemId, string customer)
    {
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty.", nameof(itemId));

        if (string.IsNullOrWhiteSpace(customer))
            throw new ArgumentException("Customer cannot be empty.", nameof(customer));

        ItemId = itemId;
        Customer = customer.Trim();
        IsDefaultCustomer = false;
        IsPreferredCustomer = false;
        IsDisabled = false;

        AddDomainEvent(new ItemCustomerCreatedEvent(this));
    }

    public void UpdateInfo(string? customerItemCode, string? customerItemName)
    {
        CustomerItemCode = customerItemCode?.Trim();
        CustomerItemName = customerItemName?.Trim();
        AddDomainEvent(new ItemCustomerUpdatedEvent(this));
    }

    public void SetAsDefaultCustomer()
    {
        IsDefaultCustomer = true;
        AddDomainEvent(new ItemCustomerDefaultSetEvent(this));
    }

    public void SetAsPreferredCustomer()
    {
        IsPreferredCustomer = true;
        AddDomainEvent(new ItemCustomerPreferredSetEvent(this));
    }

    public void UpdatePricing(decimal? refRate)
    {
        RefRate = refRate;
        AddDomainEvent(new ItemCustomerPricingUpdatedEvent(this));
    }

    public void UpdateUOMSettings(string? refUOM, decimal? conversionFactor)
    {
        RefUOM = refUOM?.Trim();
        ConversionFactor = conversionFactor;
        AddDomainEvent(new ItemCustomerUOMUpdatedEvent(this));
    }

    public void Disable()
    {
        IsDisabled = true;
        AddDomainEvent(new ItemCustomerDisabledEvent(this));
    }

    public void Enable()
    {
        IsDisabled = false;
        AddDomainEvent(new ItemCustomerEnabledEvent(this));
    }

    public bool IsDefault()
    {
        return IsDefaultCustomer;
    }

    public bool IsPreferred()
    {
        return IsPreferredCustomer;
    }

    public bool IsDisabledCustomer()
    {
        return IsDisabled;
    }
} 
