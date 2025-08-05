using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Events;
using TossErp.Stock.Domain.Events;

namespace TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;

public class ItemManufacturer : BaseAuditableEntity
{
    public Guid ItemId { get; private set; }
    public string ManufacturerName { get; private set; } = string.Empty;
    public string? ManufacturerPartNumber { get; private set; }
    public string? CountryOfOrigin { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; } = true;

    private ItemManufacturer() { } // For EF Core

    public ItemManufacturer(Guid itemId, string manufacturerName, string? manufacturerPartNumber = null, 
        string? countryOfOrigin = null, bool isDefault = false)
    {
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty.", nameof(itemId));

        if (string.IsNullOrWhiteSpace(manufacturerName))
            throw new ArgumentException("Manufacturer name cannot be empty.", nameof(manufacturerName));

        ItemId = itemId;
        ManufacturerName = manufacturerName.Trim();
        ManufacturerPartNumber = manufacturerPartNumber?.Trim();
        CountryOfOrigin = countryOfOrigin?.Trim();
        IsDefault = isDefault;

        AddDomainEvent(new ItemManufacturerCreatedEvent(this));
    }

    public void UpdateManufacturerInfo(string manufacturerName, string? manufacturerPartNumber = null, 
        string? countryOfOrigin = null)
    {
        if (string.IsNullOrWhiteSpace(manufacturerName))
            throw new ArgumentException("Manufacturer name cannot be empty.", nameof(manufacturerName));

        ManufacturerName = manufacturerName.Trim();
        ManufacturerPartNumber = manufacturerPartNumber?.Trim();
        CountryOfOrigin = countryOfOrigin?.Trim();
        AddDomainEvent(new ItemManufacturerUpdatedEvent(this));
    }

    public void SetAsDefault()
    {
        if (IsDefault) return;

        IsDefault = true;
        AddDomainEvent(new ItemManufacturerDefaultSetEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        AddDomainEvent(new ItemManufacturerDeactivatedEvent(this));
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        AddDomainEvent(new ItemManufacturerActivatedEvent(this));
    }

    public void Delete()
    {
        AddDomainEvent(new ItemManufacturerDeletedEvent(this));
    }
} 
