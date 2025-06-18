using System;
using System.Linq.Expressions;

namespace POS.Domain.SeedWork;

public interface ISpecification<T>
{
    bool IsSatisfiedBy(T entity);
}

/// <summary>
/// Base class for specifications supporting both expression and in-memory evaluation
/// </summary>
public abstract class Specification<T> : ISpecification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();

    public virtual bool IsSatisfiedBy(T entity)
    {
        var predicate = ToExpression().Compile();
        return predicate(entity);
    }

    public Specification<T> And(ISpecification<T> other)
    {
        return new AndSpecification<T>(this, other);
    }

    public Specification<T> Or(ISpecification<T> other)
    {
        return new OrSpecification<T>(this, other);
    }

    public Specification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}

internal class AndSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        // If both are Specification<T>, combine their expressions; otherwise, fallback to IsSatisfiedBy
        if (_left is Specification<T> leftSpec && _right is Specification<T> rightSpec)
        {
            var leftExpr = leftSpec.ToExpression();
            var rightExpr = rightSpec.ToExpression();
            var invokedExpr = Expression.Invoke(rightExpr, leftExpr.Parameters);
            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(leftExpr.Body, invokedExpr), leftExpr.Parameters);
        }
        // Fallback: use IsSatisfiedBy
        return x => _left.IsSatisfiedBy(x) && _right.IsSatisfiedBy(x);
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);
    }
}

internal class OrSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        if (_left is Specification<T> leftSpec && _right is Specification<T> rightSpec)
        {
            var leftExpr = leftSpec.ToExpression();
            var rightExpr = rightSpec.ToExpression();
            var invokedExpr = Expression.Invoke(rightExpr, leftExpr.Parameters);
            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(leftExpr.Body, invokedExpr), leftExpr.Parameters);
        }
        return x => _left.IsSatisfiedBy(x) || _right.IsSatisfiedBy(x);
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) || _right.IsSatisfiedBy(entity);
    }
}

internal class NotSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _specification;

    public NotSpecification(ISpecification<T> specification)
    {
        _specification = specification ?? throw new ArgumentNullException(nameof(specification));
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        if (_specification is Specification<T> spec)
        {
            var expr = spec.ToExpression();
            var notExpr = Expression.Not(expr.Body);
            return Expression.Lambda<Func<T, bool>>(notExpr, expr.Parameters);
        }
        return x => !_specification.IsSatisfiedBy(x);
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return !_specification.IsSatisfiedBy(entity);
    }
} 
