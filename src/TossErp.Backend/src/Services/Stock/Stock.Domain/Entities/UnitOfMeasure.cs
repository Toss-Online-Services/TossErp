using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Events;

namespace TossErp.Stock.Domain.Entities;

public class UnitOfMeasure : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsBaseUnit { get; private set; }
    public bool IsActive { get; private set; } = true;

    private UnitOfMeasure() { } // For EF Core

    public UnitOfMeasure(string code, string name, string? description = null, bool isBaseUnit = false)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = description?.Trim();
        IsBaseUnit = isBaseUnit;

        AddDomainEvent(new UnitOfMeasureCreatedEvent(this));
    }

    public void Update(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();

        AddDomainEvent(new UnitOfMeasureUpdatedEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        AddDomainEvent(new UnitOfMeasureDeactivatedEvent(this));
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        AddDomainEvent(new UnitOfMeasureActivatedEvent(this));
    }

    public void Delete()
    {
        AddDomainEvent(new UnitOfMeasureDeletedEvent(this));
    }
} 
