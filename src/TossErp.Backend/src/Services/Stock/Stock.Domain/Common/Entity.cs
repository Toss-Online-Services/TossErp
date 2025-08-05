namespace TossErp.Stock.Domain.Common;

/// <summary>
/// Base class for all entities in the domain
/// Provides domain event handling and identity management
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    protected Entity(Guid id)
    {
        Id = id;
    }

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj is not Entity other) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        return EqualOperator(left, right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return NotEqualOperator(left, right);
    }

    protected static bool EqualOperator(Entity? left, Entity? right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }
        return left?.Equals(right) != false;
    }

    protected static bool NotEqualOperator(Entity? left, Entity? right)
    {
        return !EqualOperator(left, right);
    }
} 
