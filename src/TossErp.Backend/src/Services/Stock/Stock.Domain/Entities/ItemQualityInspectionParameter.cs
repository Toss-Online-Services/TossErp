using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Events;

namespace TossErp.Stock.Domain.Entities;

public class ItemQualityInspectionParameter : BaseAuditableEntity
{
    public Guid ItemId { get; private set; }
    public string ParameterName { get; private set; } = string.Empty;
    public string? Value { get; private set; }
    public string? Result { get; private set; }
    public bool IsActive { get; private set; } = true;

    private ItemQualityInspectionParameter() { }

    public ItemQualityInspectionParameter(Guid itemId, string parameterName, string? value = null, string? result = null)
    {
        if (itemId == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty.", nameof(itemId));
        if (string.IsNullOrWhiteSpace(parameterName))
            throw new ArgumentException("Parameter name cannot be empty.", nameof(parameterName));
        ItemId = itemId;
        ParameterName = parameterName.Trim();
        Value = value?.Trim();
        Result = result?.Trim();
        AddDomainEvent(new ItemQualityInspectionParameterCreatedEvent(this));
    }

    public void UpdateValue(string? value)
    {
        Value = value?.Trim();
        AddDomainEvent(new ItemQualityInspectionParameterUpdatedEvent(this));
    }

    public void UpdateResult(string? result)
    {
        Result = result?.Trim();
        AddDomainEvent(new ItemQualityInspectionParameterResultUpdatedEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        AddDomainEvent(new ItemQualityInspectionParameterDeactivatedEvent(this));
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        AddDomainEvent(new ItemQualityInspectionParameterActivatedEvent(this));
    }

    public void Delete()
    {
        AddDomainEvent(new ItemQualityInspectionParameterDeletedEvent(this));
    }
} 
