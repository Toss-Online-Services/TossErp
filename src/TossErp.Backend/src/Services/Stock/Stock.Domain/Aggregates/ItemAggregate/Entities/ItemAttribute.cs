using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Events;

namespace TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;

public class ItemAttribute : BaseAuditableEntity
{
    public Guid ItemId { get; private set; }
    public string AttributeName { get; private set; } = string.Empty;
    public string? AttributeValue { get; private set; }
    public string? AttributeType { get; private set; }
    public bool IsActive { get; private set; } = true;

    private ItemAttribute() { } // For EF Core

    public ItemAttribute(Guid itemId, string attributeName, string? attributeValue = null, string? attributeType = null)
    {
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty.", nameof(itemId));

        if (string.IsNullOrWhiteSpace(attributeName))
            throw new ArgumentException("Attribute name cannot be empty.", nameof(attributeName));

        ItemId = itemId;
        AttributeName = attributeName.Trim();
        AttributeValue = attributeValue?.Trim();
        AttributeType = attributeType?.Trim();

        AddDomainEvent(new ItemAttributeCreatedEvent(this));
    }

    public void UpdateValue(string? attributeValue)
    {
        AttributeValue = attributeValue?.Trim();
        AddDomainEvent(new ItemAttributeUpdatedEvent(this));
    }

    public void UpdateType(string? attributeType)
    {
        AttributeType = attributeType?.Trim();
        AddDomainEvent(new ItemAttributeTypeUpdatedEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        AddDomainEvent(new ItemAttributeDeactivatedEvent(this));
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        AddDomainEvent(new ItemAttributeActivatedEvent(this));
    }

    public void Delete()
    {
        AddDomainEvent(new ItemAttributeDeletedEvent(this));
    }
} 
