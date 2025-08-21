using MediatR;

namespace TossErp.Accounts.Domain.SeedWork;

public interface IAggregateRoot
{
    IReadOnlyCollection<INotification> DomainEvents { get; }
    void AddDomainEvent(INotification eventItem);
    void RemoveDomainEvent(INotification eventItem);
    void ClearDomainEvents();
}

public abstract class AggregateRoot : Entity, IAggregateRoot
{
    // Multi-tenant support
    public string TenantId { get; protected set; } = string.Empty;

    protected AggregateRoot() : base() { }

    protected AggregateRoot(Guid id, string tenantId) : base()
    {
        Id = id;
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        CreatedAt = DateTime.UtcNow;
    }

    // Audit properties
    public DateTime CreatedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public string? UpdatedBy { get; protected set; }

    protected void MarkAsUpdated(string? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}

public interface ITenantEntity
{
    string TenantId { get; }
}

public interface IAuditableEntity
{
    DateTime CreatedAt { get; }
    string? CreatedBy { get; }
    DateTime? UpdatedAt { get; }
    string? UpdatedBy { get; }
}

public abstract class Entity : ITenantEntity, IAuditableEntity
{
    private int? _requestedHashCode;
    private Guid _id;

    private List<INotification> _domainEvents = new();

    public virtual Guid Id
    {
        get => _id;
        protected set => _id = value;
    }

    // Multi-tenant support
    public string TenantId { get; protected set; } = string.Empty;

    // Audit properties
    public DateTime CreatedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public string? UpdatedBy { get; protected set; }

    protected Entity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    protected Entity(Guid id, string tenantId)
    {
        Id = id;
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        CreatedAt = DateTime.UtcNow;
    }

    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly() ?? new List<INotification>().AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents ??= new List<INotification>();
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    protected void MarkAsUpdated(string? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public bool IsTransient()
    {
        return Id == default;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity entity)
            return false;

        if (ReferenceEquals(this, entity))
            return true;

        if (GetType() != entity.GetType())
            return false;

        if (entity.IsTransient() || IsTransient())
            return false;

        return entity.Id == Id;
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode = Id.GetHashCode() ^ 31;

            return _requestedHashCode.Value;
        }

        return base.GetHashCode();
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        return left?.Equals(right) ?? Equals(right, null);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}

public abstract class Entity<T> : Entity where T : notnull
{
    public new virtual T Id { get; protected set; } = default!;
}

public abstract class ValueObject
{
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }
        return left is null || left.Equals(right);
    }

    protected static bool NotEqualOperator(ValueObject left, ValueObject right)
    {
        return !(EqualOperator(left, right));
    }

    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x.GetHashCode())
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? one, ValueObject? two)
    {
        return EqualOperator(one!, two!);
    }

    public static bool operator !=(ValueObject? two, ValueObject? one)
    {
        return NotEqualOperator(one!, two!);
    }
}
