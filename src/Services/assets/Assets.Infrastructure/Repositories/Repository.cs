namespace TossErp.Assets.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation with EF Core 9 optimizations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Repository<T> : IRepository<T> where T : class, ITenantEntity
{
    protected readonly AssetsDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<Repository<T>> _logger;

    public Repository(AssetsDbContext context, ILogger<Repository<T>> logger)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _logger = logger;
    }

    public virtual async Task<T?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.FindAsync([id], cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public virtual async Task<T?> GetAsync(
        Expression<Func<T, bool>> filter,
        bool tracked = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<T> query = _dbSet;

            if (!tracked)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving {EntityType} with filter", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "",
        bool tracked = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            // Include related properties
            foreach (var includeProperty in includeProperties.Split(
                new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            if (!tracked)
                query = query.AsNoTracking();

            if (orderBy != null)
                return await orderBy(query).ToListAsync(cancellationToken);
            else
                return await query.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving {EntityType} list", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "",
        bool tracked = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync(cancellationToken);

            // Include related properties
            foreach (var includeProperty in includeProperties.Split(
                new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            if (!tracked)
                query = query.AsNoTracking();

            if (orderBy != null)
                query = orderBy(query);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged {EntityType} list", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.AnyAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            return await query.CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task AddRangeAsync(
        IEnumerable<T> entities, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding range of {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual void Update(T entity)
    {
        try
        {
            _dbSet.Update(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        try
        {
            _dbSet.UpdateRange(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating range of {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual void Remove(T entity)
    {
        try
        {
            _dbSet.Remove(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        try
        {
            _dbSet.RemoveRange(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing range of {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<int> ExecuteUpdateAsync(
        Expression<Func<T, bool>> filter,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // EF Core 9 ExecuteUpdate support for complex types
            return await _dbSet
                .Where(filter)
                .ExecuteUpdateAsync(setPropertyCalls, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing bulk update on {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<int> ExecuteDeleteAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Where(filter)
                .ExecuteDeleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing bulk delete on {EntityType}", typeof(T).Name);
            throw;
        }
    }

    // Advanced querying with specifications
    public virtual async Task<IEnumerable<T>> GetWithSpecificationAsync(
        ISpecification<T> specification,
        bool tracked = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<T> query = _dbSet;

            if (!tracked)
                query = query.AsNoTracking();

            query = ApplySpecification(query, specification);

            return await query.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving {EntityType} with specification", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedWithSpecificationAsync(
        ISpecification<T> specification,
        int page,
        int pageSize,
        bool tracked = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<T> query = _dbSet;
            
            query = ApplySpecificationFilters(query, specification);
            var totalCount = await query.CountAsync(cancellationToken);

            if (!tracked)
                query = query.AsNoTracking();

            query = ApplySpecification(query, specification);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged {EntityType} with specification", typeof(T).Name);
            throw;
        }
    }

    protected virtual IQueryable<T> ApplySpecification(IQueryable<T> query, ISpecification<T> spec)
    {
        // Apply filters
        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);

        // Apply includes
        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

        // Apply string includes
        query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

        // Apply ordering
        if (spec.OrderBy != null)
            query = query.OrderBy(spec.OrderBy);
        else if (spec.OrderByDescending != null)
            query = query.OrderByDescending(spec.OrderByDescending);

        // Apply grouping
        if (spec.GroupBy != null)
            query = query.GroupBy(spec.GroupBy).SelectMany(x => x);

        return query;
    }

    protected virtual IQueryable<T> ApplySpecificationFilters(IQueryable<T> query, ISpecification<T> spec)
    {
        // Apply only filters for count operations
        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);

        return query;
    }
}
