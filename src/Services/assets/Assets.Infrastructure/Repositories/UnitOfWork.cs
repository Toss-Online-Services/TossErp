namespace TossErp.Assets.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation with EF Core 9 optimizations
/// </summary>
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AssetsDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly Dictionary<Type, object> _repositories = new();
    private bool _disposed = false;

    public UnitOfWork(AssetsDbContext context, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IRepository<T> Repository<T>() where T : class, ITenantEntity
    {
        var type = typeof(T);
        
        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(Repository<>).MakeGenericType(type);
            var repositoryLogger = LoggerFactory.Create(builder => builder.AddConsole())
                                               .CreateLogger(repositoryType);
            
            var repository = Activator.CreateInstance(repositoryType, _context, repositoryLogger);
            _repositories[type] = repository!;
        }

        return (IRepository<T>)_repositories[type];
    }

    public IAssetRepository AssetRepository
    {
        get
        {
            const string key = "AssetRepository";
            if (!_repositories.ContainsKey(typeof(string)) || !_repositories.ContainsKey(key))
            {
                var logger = LoggerFactory.Create(builder => builder.AddConsole())
                                         .CreateLogger<AssetRepository>();
                _repositories[key] = new AssetRepository(_context, logger);
            }
            return (IAssetRepository)_repositories[key];
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Saved {ChangeCount} changes to database", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    public int SaveChanges()
    {
        try
        {
            var result = _context.SaveChanges();
            _logger.LogDebug("Saved {ChangeCount} changes to database", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Database transaction started");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting database transaction");
            throw;
        }
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.CommitTransactionAsync(cancellationToken);
            _logger.LogDebug("Database transaction committed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing database transaction");
            throw;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
            _logger.LogDebug("Database transaction rolled back");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back database transaction");
            throw;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
            _repositories.Clear();
            _disposed = true;
        }
    }
}
