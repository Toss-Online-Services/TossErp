using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate.Events;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;

public class ItemVariantAttribute : BaseEntity
{
    [NotMapped]
    public ItemVariant ItemVariant { get; private set; } = null!;
    public string Attribute { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public bool IsDisabled { get; private set; }

    private ItemVariantAttribute() { } // For EF Core

    public ItemVariantAttribute(ItemVariant itemVariant, string attribute, string value)
    {
        ItemVariant = itemVariant ?? throw new ArgumentNullException(nameof(itemVariant));
        Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        IsDisabled = false;

        AddDomainEvent(new ItemVariantAttributeCreatedEvent(this));
    }

    public void UpdateValue(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        AddDomainEvent(new ItemVariantAttributeValueUpdatedEvent(this));
    }

    public void Disable()
    {
        IsDisabled = true;
        AddDomainEvent(new ItemVariantAttributeDisabledEvent(this));
    }

    public void Enable()
    {
        IsDisabled = false;
        AddDomainEvent(new ItemVariantAttributeEnabledEvent(this));
    }

    public bool IsDisabledAttribute()
    {
        return IsDisabled;
    }
} 
