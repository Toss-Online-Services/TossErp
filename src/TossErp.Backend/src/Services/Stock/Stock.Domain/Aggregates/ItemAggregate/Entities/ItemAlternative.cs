using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Events;

namespace TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;

public class ItemAlternative : BaseAuditableEntity
{
    public Guid ItemId { get; private set; }
    public Guid AlternativeItemId { get; private set; }
    public string? ConversionFactor { get; private set; }
    public bool IsPreferred { get; private set; }
    public bool IsActive { get; private set; } = true;

    private ItemAlternative() { } // For EF Core

    public ItemAlternative(Guid itemId, Guid alternativeItemId, string? conversionFactor = null, bool isPreferred = false)
    {
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty.", nameof(itemId));

        if (alternativeItemId == Guid.Empty)
            throw new ArgumentException("Alternative item ID cannot be empty.", nameof(alternativeItemId));

        if (itemId == alternativeItemId)
            throw new ArgumentException("Item cannot be its own alternative.", nameof(alternativeItemId));

        ItemId = itemId;
        AlternativeItemId = alternativeItemId;
        ConversionFactor = conversionFactor?.Trim();
        IsPreferred = isPreferred;

        AddDomainEvent(new ItemAlternativeCreatedEvent(this));
    }

    public void UpdateConversionFactor(string? conversionFactor)
    {
        ConversionFactor = conversionFactor?.Trim();
        AddDomainEvent(new ItemAlternativeUpdatedEvent(this));
    }

    public void SetAsPreferred()
    {
        if (IsPreferred) return;

        IsPreferred = true;
        AddDomainEvent(new ItemAlternativePreferredSetEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        AddDomainEvent(new ItemAlternativeDeactivatedEvent(this));
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        AddDomainEvent(new ItemAlternativeActivatedEvent(this));
    }

    public void Delete()
    {
        AddDomainEvent(new ItemAlternativeDeletedEvent(this));
    }
} 
