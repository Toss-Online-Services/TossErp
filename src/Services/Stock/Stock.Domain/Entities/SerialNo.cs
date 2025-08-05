using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Events;

namespace TossErp.Stock.Domain.Entities;

public class SerialNo : BaseAuditableEntity
{
    public Guid ItemId { get; private set; }
    public string SerialNumber { get; private set; } = string.Empty;
    public string? BatchNo { get; private set; }
    public DateTime? ManufacturingDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DateTime? WarrantyExpiryDate { get; private set; }
    public string? Location { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Inventory management properties
    public string Warehouse { get; private set; } = string.Empty;
    public string Bin { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal TransferQuantity { get; private set; }
    public decimal ConsumedQuantity { get; private set; }
    public decimal DispatchedQuantity { get; private set; }
    public decimal ReturnedQuantity { get; private set; }
    public decimal ScrappedQuantity { get; private set; }
    public string Description { get; private set; } = string.Empty;

    private SerialNo() { } // For EF Core

    public SerialNo(Guid itemId, string serialNumber, string? batchNo = null, 
        DateTime? manufacturingDate = null, DateTime? expiryDate = null, string? location = null,
        decimal quantity = 1)
    {
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty.", nameof(itemId));

        if (string.IsNullOrWhiteSpace(serialNumber))
            throw new ArgumentException("Serial number cannot be empty.", nameof(serialNumber));

        ItemId = itemId;
        SerialNumber = serialNumber.Trim().ToUpperInvariant();
        BatchNo = batchNo?.Trim();
        ManufacturingDate = manufacturingDate;
        ExpiryDate = expiryDate;
        Location = location?.Trim();
        Quantity = quantity;

        AddDomainEvent(new SerialNoCreatedEvent(this));
    }

    public void UpdateBatchNo(string? batchNo)
    {
        BatchNo = batchNo?.Trim();
        AddDomainEvent(new SerialNoBatchNoUpdatedEvent(this));
    }

    public void UpdateDates(DateTime? manufacturingDate, DateTime? expiryDate, DateTime? warrantyExpiryDate)
    {
        ManufacturingDate = manufacturingDate;
        ExpiryDate = expiryDate;
        WarrantyExpiryDate = warrantyExpiryDate;
        AddDomainEvent(new SerialNoDatesUpdatedEvent(this));
    }

    public void UpdateLocation(string? location)
    {
        Location = location?.Trim();
        AddDomainEvent(new SerialNoLocationUpdatedEvent(this));
    }

    public void UpdateWarehouse(string warehouse)
    {
        Warehouse = warehouse ?? string.Empty;
        AddDomainEvent(new SerialNoWarehouseUpdatedEvent(this));
    }

    public void UpdateBin(string bin)
    {
        Bin = bin ?? string.Empty;
        AddDomainEvent(new SerialNoBinUpdatedEvent(this));
    }

    public void UpdateDescription(string description)
    {
        Description = description ?? string.Empty;
        AddDomainEvent(new SerialNoDescriptionUpdatedEvent(this));
    }

    // Inventory management methods
    public void UpdateQuantity(decimal quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

        Quantity = quantity;
        AddDomainEvent(new SerialNoQuantityUpdatedEvent(this));
    }

    public void AddTransferQuantity(decimal transferQty)
    {
        if (transferQty < 0)
            throw new ArgumentException("Transfer quantity cannot be negative", nameof(transferQty));

        TransferQuantity += transferQty;
        AddDomainEvent(new SerialNoTransferQuantityUpdatedEvent(this));
    }

    public void AddConsumedQuantity(decimal consumedQty)
    {
        if (consumedQty < 0)
            throw new ArgumentException("Consumed quantity cannot be negative", nameof(consumedQty));

        ConsumedQuantity += consumedQty;
        AddDomainEvent(new SerialNoConsumedQuantityUpdatedEvent(this));
    }

    public void AddDispatchedQuantity(decimal dispatchedQty)
    {
        if (dispatchedQty < 0)
            throw new ArgumentException("Dispatched quantity cannot be negative", nameof(dispatchedQty));

        DispatchedQuantity += dispatchedQty;
        AddDomainEvent(new SerialNoDispatchedQuantityUpdatedEvent(this));
    }

    public void AddReturnedQuantity(decimal returnedQty)
    {
        if (returnedQty < 0)
            throw new ArgumentException("Returned quantity cannot be negative", nameof(returnedQty));

        ReturnedQuantity += returnedQty;
        AddDomainEvent(new SerialNoReturnedQuantityUpdatedEvent(this));
    }

    public void AddScrappedQuantity(decimal scrappedQty)
    {
        if (scrappedQty < 0)
            throw new ArgumentException("Scrapped quantity cannot be negative", nameof(scrappedQty));

        ScrappedQuantity += scrappedQty;
        AddDomainEvent(new SerialNoScrappedQuantityUpdatedEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        AddDomainEvent(new SerialNoDeactivatedEvent(this));
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        AddDomainEvent(new SerialNoActivatedEvent(this));
    }

    public void Delete()
    {
        AddDomainEvent(new SerialNoDeletedEvent(this));
    }

    public bool IsExpired()
    {
        return ExpiryDate.HasValue && DateTime.UtcNow > ExpiryDate.Value;
    }

    public bool IsExpiringSoon(int daysThreshold = 30)
    {
        if (!ExpiryDate.HasValue) return false;
        
        var daysUntilExpiry = (ExpiryDate.Value - DateTime.UtcNow).Days;
        return daysUntilExpiry <= daysThreshold && daysUntilExpiry >= 0;
    }

    public bool IsWarrantyExpired()
    {
        return WarrantyExpiryDate.HasValue && DateTime.UtcNow > WarrantyExpiryDate.Value;
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
