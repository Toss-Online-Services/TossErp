using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Events;

namespace TossErp.Stock.Domain.Entities;

public class ItemGroup : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private ItemGroup() { } // For EF Core

    public ItemGroup(string code, string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = description?.Trim();

        AddDomainEvent(new ItemGroupCreatedEvent(this));
    }

    public void Update(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();

        AddDomainEvent(new ItemGroupUpdatedEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        AddDomainEvent(new ItemGroupDeactivatedEvent(this));
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        AddDomainEvent(new ItemGroupActivatedEvent(this));
    }

    public void Delete()
    {
        AddDomainEvent(new ItemGroupDeletedEvent(this));
    }
} 
