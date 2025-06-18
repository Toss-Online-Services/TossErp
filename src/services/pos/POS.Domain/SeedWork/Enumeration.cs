using System;
using System.Collections.Generic;
using System.Linq;

namespace POS.Domain.SeedWork;

/// <summary>
/// Base class for strongly-typed enumerations (pattern from DDD)
/// </summary>
public abstract class Enumeration : IComparable
{
    public int Id { get; }
    public string Name { get; }

    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString() => Name;

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
            return false;

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public int CompareTo(object? other)
    {
        if (other is null) return 1;
        return Id.CompareTo(((Enumeration)other).Id);
    }

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        var fields = typeof(T).GetFields(System.Reflection.BindingFlags.Public |
                                         System.Reflection.BindingFlags.Static |
                                         System.Reflection.BindingFlags.DeclaredOnly);
        return fields.Select(f => f.GetValue(null)).Cast<T>();
    }
} 
