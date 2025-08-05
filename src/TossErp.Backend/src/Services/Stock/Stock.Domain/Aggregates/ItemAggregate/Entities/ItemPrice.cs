using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;

/// <summary>
/// Item Price - Child entity of Item Aggregate
/// Represents pricing information for an item in different price lists and currencies
/// </summary>
public class ItemPrice : Entity
{
    public string PriceList { get; private set; } = string.Empty;
    public string Currency { get; private set; } = string.Empty;
    public decimal Rate { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public bool IsActive { get; private set; }

    protected ItemPrice() { } // For EF Core

    public ItemPrice(string priceList, string currency, decimal rate, DateTime validFrom, DateTime? validTo = null)
    {
        if (string.IsNullOrWhiteSpace(priceList))
            throw new ArgumentException("Price list cannot be empty", nameof(priceList));
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));
        if (rate < 0)
            throw new ArgumentException("Rate cannot be negative", nameof(rate));

        PriceList = priceList.Trim();
        Currency = currency.Trim().ToUpperInvariant();
        Rate = rate;
        ValidFrom = validFrom;
        ValidTo = validTo;
        IsActive = true;
    }

    public void UpdateRate(decimal newRate)
    {
        if (newRate < 0)
            throw new ArgumentException("Rate cannot be negative", nameof(newRate));

        Rate = newRate;
    }

    public void UpdateValidityPeriod(DateTime validFrom, DateTime? validTo = null)
    {
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public bool IsValid(DateTime? asOfDate = null)
    {
        var checkDate = asOfDate ?? DateTime.UtcNow;
        return IsActive && 
               checkDate >= ValidFrom && 
               (!ValidTo.HasValue || checkDate <= ValidTo.Value);
    }

    public bool IsExpired(DateTime? asOfDate = null)
    {
        var checkDate = asOfDate ?? DateTime.UtcNow;
        return ValidTo.HasValue && checkDate > ValidTo.Value;
    }
} 
