namespace TossErp.Shared.SeedWork;

/// <summary>
/// Base abstract entity class
/// </summary>
public abstract class Entity : IEntity
{
    public abstract Guid Id { get; protected set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (Id == Guid.Empty || other.Id == Guid.Empty)
            return false;

        return Id == other.Id;
    }

    public static bool operator ==(Entity? a, Entity? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity? a, Entity? b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }
}

/// <summary>
/// Base abstract entity class with typed ID
/// </summary>
public abstract class Entity<T> : Entity, IEntity<T>
{
    public abstract new T Id { get; protected set; }

    public abstract DateTime CreatedAt { get; protected set; }
    public abstract string CreatedBy { get; protected set; }

    Guid IEntity.Id => Id is Guid guid ? guid : Guid.Empty;
}
