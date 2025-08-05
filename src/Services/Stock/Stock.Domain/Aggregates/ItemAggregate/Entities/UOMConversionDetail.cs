using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Domain.Aggregates.ItemAggregate.Entities;

public class UOMConversionDetail : BaseAuditableEntity
{
    public Guid ItemId { get; private set; }
    public string FromUOM { get; private set; } = string.Empty;
    public string ToUOM { get; private set; } = string.Empty;
    public decimal Factor { get; private set; }
    public bool IsActive { get; private set; } = true;

    private UOMConversionDetail() { }

    public UOMConversionDetail(Guid itemId, string fromUOM, string toUOM, decimal factor)
    {
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty.", nameof(itemId));
        if (string.IsNullOrWhiteSpace(fromUOM))
            throw new ArgumentException("From UOM cannot be empty.", nameof(fromUOM));
        if (string.IsNullOrWhiteSpace(toUOM))
            throw new ArgumentException("To UOM cannot be empty.", nameof(toUOM));
        if (factor <= 0)
            throw new ArgumentException("Factor must be positive.", nameof(factor));
        ItemId = itemId;
        FromUOM = fromUOM.Trim();
        ToUOM = toUOM.Trim();
        Factor = factor;
        AddDomainEvent(new TossErp.Stock.Domain.Aggregates.ItemAggregate.Events.UOMConversionDetailCreatedEvent(this));
    }

    public void UpdateFactor(decimal factor)
    {
        if (factor <= 0)
            throw new ArgumentException("Factor must be positive.", nameof(factor));
        Factor = factor;
        AddDomainEvent(new UOMConversionDetailUpdatedEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        AddDomainEvent(new UOMConversionDetailDeactivatedEvent(this));
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        AddDomainEvent(new UOMConversionDetailActivatedEvent(this));
    }

    public void Delete()
    {
        AddDomainEvent(new UOMConversionDetailDeletedEvent(this));
    }
} 
