using TossErp.Stock.Domain.Events;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;

namespace TossErp.Stock.Domain.Entities;

public class Batch : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public ItemAggregate Item { get; private set; } = null!;
    public string ItemCode { get; private set; } = string.Empty;
    public DateTime? ExpiryDate { get; private set; }
    public DateTime? ManufacturingDate { get; private set; }
    public DateTime? WarrantyExpiryDate { get; private set; }
    public string? Supplier { get; private set; }
    public string? ReferenceDocumentType { get; private set; }
    public string? ReferenceDocumentNo { get; private set; }
    public string? ReferenceDocumentDetailNo { get; private set; }
    public string? Description { get; private set; }
    public string? Remarks { get; private set; }
    public bool IsDisabled { get; private set; }

    // Inventory management properties
    public decimal Quantity { get; private set; }
    public decimal TransferQuantity { get; private set; }
    public decimal ConsumedQuantity { get; private set; }
    public decimal DispatchedQuantity { get; private set; }
    public decimal ReturnedQuantity { get; private set; }
    public decimal ScrappedQuantity { get; private set; }
    public decimal RetainSample { get; private set; }
    public decimal RetainSampleQuantity { get; private set; }
    public string RetainSampleUOM { get; private set; } = string.Empty;
    public decimal RetainSampleUOMQuantity { get; private set; }
    public string RetainSampleWarehouse { get; private set; } = string.Empty;
    public string RetainSampleBin { get; private set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<StockLedgerEntry> StockLedgerEntries { get; private set; } = new List<StockLedgerEntry>();

    private Batch() { } // For EF Core

    public Batch(string name, ItemAggregate item, decimal quantity = 0)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Item = item ?? throw new ArgumentNullException(nameof(item));
        ItemCode = item.ItemCode.Value;
        Quantity = quantity;
        IsDisabled = false;

        AddDomainEvent(new BatchCreatedEvent(this));
    }

    public void UpdateExpiryDate(DateTime? expiryDate)
    {
        ExpiryDate = expiryDate;
        AddDomainEvent(new BatchExpiryDateUpdatedEvent(this));
    }

    public void UpdateManufacturingDate(DateTime? manufacturingDate)
    {
        ManufacturingDate = manufacturingDate;
        AddDomainEvent(new BatchManufacturingDateUpdatedEvent(this));
    }

    public void UpdateWarrantyExpiryDate(DateTime? warrantyExpiryDate)
    {
        WarrantyExpiryDate = warrantyExpiryDate;
        AddDomainEvent(new BatchWarrantyExpiryDateUpdatedEvent(this));
    }

    public void UpdateSupplier(string? supplier)
    {
        Supplier = supplier;
        AddDomainEvent(new BatchSupplierUpdatedEvent(this));
    }

    public void UpdateReferenceDocument(string? referenceDocumentType, string? referenceDocumentNo, string? referenceDocumentDetailNo)
    {
        ReferenceDocumentType = referenceDocumentType;
        ReferenceDocumentNo = referenceDocumentNo;
        ReferenceDocumentDetailNo = referenceDocumentDetailNo;
        AddDomainEvent(new BatchReferenceDocumentUpdatedEvent(this));
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
        AddDomainEvent(new BatchDescriptionUpdatedEvent(this));
    }

    public void UpdateRemarks(string? remarks)
    {
        Remarks = remarks;
        AddDomainEvent(new BatchRemarksUpdatedEvent(this));
    }

    // Inventory management methods
    public void UpdateQuantity(decimal quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

        Quantity = quantity;
        AddDomainEvent(new BatchQuantityUpdatedEvent(this));
    }

    public void AddTransferQuantity(decimal transferQty)
    {
        if (transferQty < 0)
            throw new ArgumentException("Transfer quantity cannot be negative", nameof(transferQty));

        TransferQuantity += transferQty;
        AddDomainEvent(new BatchTransferQuantityUpdatedEvent(this));
    }

    public void AddConsumedQuantity(decimal consumedQty)
    {
        if (consumedQty < 0)
            throw new ArgumentException("Consumed quantity cannot be negative", nameof(consumedQty));

        ConsumedQuantity += consumedQty;
        AddDomainEvent(new BatchConsumedQuantityUpdatedEvent(this));
    }

    public void AddDispatchedQuantity(decimal dispatchedQty)
    {
        if (dispatchedQty < 0)
            throw new ArgumentException("Dispatched quantity cannot be negative", nameof(dispatchedQty));

        DispatchedQuantity += dispatchedQty;
        AddDomainEvent(new BatchDispatchedQuantityUpdatedEvent(this));
    }

    public void AddReturnedQuantity(decimal returnedQty)
    {
        if (returnedQty < 0)
            throw new ArgumentException("Returned quantity cannot be negative", nameof(returnedQty));

        ReturnedQuantity += returnedQty;
        AddDomainEvent(new BatchReturnedQuantityUpdatedEvent(this));
    }

    public void AddScrappedQuantity(decimal scrappedQty)
    {
        if (scrappedQty < 0)
            throw new ArgumentException("Scrapped quantity cannot be negative", nameof(scrappedQty));

        ScrappedQuantity += scrappedQty;
        AddDomainEvent(new BatchScrappedQuantityUpdatedEvent(this));
    }

    public void UpdateRetainSample(decimal retainSample, decimal retainSampleQty, string retainSampleUOM, 
        decimal retainSampleUOMQty, string retainSampleWarehouse, string retainSampleBin)
    {
        RetainSample = retainSample;
        RetainSampleQuantity = retainSampleQty;
        RetainSampleUOM = retainSampleUOM ?? string.Empty;
        RetainSampleUOMQuantity = retainSampleUOMQty;
        RetainSampleWarehouse = retainSampleWarehouse ?? string.Empty;
        RetainSampleBin = retainSampleBin ?? string.Empty;
        
        AddDomainEvent(new BatchRetainSampleUpdatedEvent(this));
    }

    public void Disable()
    {
        IsDisabled = true;
        AddDomainEvent(new BatchDisabledEvent(this));
    }

    public void Enable()
    {
        IsDisabled = false;
        AddDomainEvent(new BatchEnabledEvent(this));
    }

    public void AddStockLedgerEntry(StockLedgerEntry entry)
    {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry));

        StockLedgerEntries.Add(entry);
        AddDomainEvent(new BatchStockLedgerEntryAddedEvent(this, entry));
    }

    public void RemoveStockLedgerEntry(StockLedgerEntry entry)
    {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry));

        if (StockLedgerEntries.Remove(entry))
        {
            AddDomainEvent(new BatchStockLedgerEntryRemovedEvent(this, entry));
        }
    }

    public bool HasExpiryDate()
    {
        return ExpiryDate.HasValue;
    }

    public bool IsExpired()
    {
        return ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.UtcNow;
    }

    public bool IsExpiringSoon(int daysThreshold = 30)
    {
        if (!ExpiryDate.HasValue)
            return false;

        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
        return ExpiryDate.Value <= thresholdDate && ExpiryDate.Value > DateTime.UtcNow;
    }

    public bool HasManufacturingDate()
    {
        return ManufacturingDate.HasValue;
    }

    public bool HasSupplier()
    {
        return !string.IsNullOrEmpty(Supplier);
    }

    public bool HasReferenceDocument()
    {
        return !string.IsNullOrEmpty(ReferenceDocumentType) && !string.IsNullOrEmpty(ReferenceDocumentNo);
    }

    public bool IsDisabledBatch()
    {
        return IsDisabled;
    }

    public string GetReferenceDocumentReference()
    {
        if (string.IsNullOrEmpty(ReferenceDocumentType) || string.IsNullOrEmpty(ReferenceDocumentNo))
            return string.Empty;

        return $"{ReferenceDocumentType}-{ReferenceDocumentNo}";
    }

    public int GetAgeInDays()
    {
        if (!ManufacturingDate.HasValue)
            return 0;

        return (int)(DateTime.UtcNow - ManufacturingDate.Value).TotalDays;
    }

    public int GetDaysUntilExpiry()
    {
        if (!ExpiryDate.HasValue)
            return int.MaxValue;

        return (int)(ExpiryDate.Value - DateTime.UtcNow).TotalDays;
    }

    public decimal GetAvailableQuantity()
    {
        return Quantity - ConsumedQuantity - DispatchedQuantity - ScrappedQuantity + ReturnedQuantity;
    }

    public bool HasAvailableQuantity()
    {
        return GetAvailableQuantity() > 0;
    }
} 
