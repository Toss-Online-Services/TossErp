using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Events;

namespace TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;

public class ItemBarcode : BaseAuditableEntity
{
    public Guid ItemId { get; private set; }
    public string Barcode { get; private set; } = string.Empty;
    public string? BarcodeType { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; } = true;

    private ItemBarcode() { } // For EF Core

    public ItemBarcode(Guid itemId, string barcode, string? barcodeType = null, bool isDefault = false)
    {
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty.", nameof(itemId));

        if (string.IsNullOrWhiteSpace(barcode))
            throw new ArgumentException("Barcode cannot be empty.", nameof(barcode));

        ItemId = itemId;
        Barcode = barcode.Trim();
        BarcodeType = barcodeType?.Trim();
        IsDefault = isDefault;

        AddDomainEvent(new ItemBarcodeCreatedEvent(this));
    }

    public void UpdateBarcode(string barcode, string? barcodeType = null)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            throw new ArgumentException("Barcode cannot be empty.", nameof(barcode));

        Barcode = barcode.Trim();
        BarcodeType = barcodeType?.Trim();
        AddDomainEvent(new ItemBarcodeUpdatedEvent(this));
    }

    public void SetAsDefault()
    {
        if (IsDefault) return;

        IsDefault = true;
        AddDomainEvent(new ItemBarcodeDefaultSetEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        AddDomainEvent(new ItemBarcodeDeactivatedEvent(this));
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        AddDomainEvent(new ItemBarcodeActivatedEvent(this));
    }

    public void Delete()
    {
        AddDomainEvent(new ItemBarcodeDeletedEvent(this));
    }
} 
