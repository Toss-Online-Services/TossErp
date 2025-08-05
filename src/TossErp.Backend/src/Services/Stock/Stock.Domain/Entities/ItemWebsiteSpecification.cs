using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Events;

namespace TossErp.Stock.Domain.Entities;

public class ItemWebsiteSpecification : BaseAuditableEntity
{
    public Guid ItemId { get; private set; }
    public string SpecName { get; private set; } = string.Empty;
    public string? SpecValue { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    private ItemWebsiteSpecification() { }

    public ItemWebsiteSpecification(Guid itemId, string specName, string? specValue = null, int displayOrder = 0)
    {
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty.", nameof(itemId));
        if (string.IsNullOrWhiteSpace(specName))
            throw new ArgumentException("Spec name cannot be empty.", nameof(specName));
        ItemId = itemId;
        SpecName = specName.Trim();
        SpecValue = specValue?.Trim();
        DisplayOrder = displayOrder;
        AddDomainEvent(new ItemWebsiteSpecificationCreatedEvent(this));
    }

    public void UpdateValue(string? specValue)
    {
        SpecValue = specValue?.Trim();
        AddDomainEvent(new ItemWebsiteSpecificationUpdatedEvent(this));
    }

    public void UpdateDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
        AddDomainEvent(new ItemWebsiteSpecificationDisplayOrderUpdatedEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        AddDomainEvent(new ItemWebsiteSpecificationDeactivatedEvent(this));
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        AddDomainEvent(new ItemWebsiteSpecificationActivatedEvent(this));
    }

    public void Delete()
    {
        AddDomainEvent(new ItemWebsiteSpecificationDeletedEvent(this));
    }
} 

