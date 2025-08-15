using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Infrastructure.Data;

/// <summary>
/// Unit of Work implementation for managing database transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets whether a transaction is currently active
    /// </summary>
    public bool HasActiveTransaction => _transaction != null;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Saving changes to database");
            var result = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Successfully saved {Count} changes to database", result);
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
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already active. Cannot begin a new transaction.");
        }

        try
        {
            _logger.LogDebug("Beginning database transaction");
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Database transaction begun successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error beginning database transaction");
            throw;
        }
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No active transaction to commit.");
        }

        try
        {
            _logger.LogDebug("Committing database transaction");
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
            _logger.LogDebug("Database transaction committed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing database transaction");
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            _logger.LogWarning("No active transaction to rollback");
            return;
        }

        try
        {
            _logger.LogDebug("Rolling back database transaction");
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
            _logger.LogDebug("Database transaction rolled back successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back database transaction");
            throw;
        }
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (_transaction != null)
        {
            throw new InvalidOperationException("Cannot execute in transaction when a transaction is already active.");
        }

        try
        {
            await BeginTransactionAsync(cancellationToken);
            
            var result = await action();
            
            await SaveChangesAsync(cancellationToken);
            await CommitTransactionAsync(cancellationToken);
            
            return result;
        }
        catch (Exception)
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (_transaction != null)
        {
            throw new InvalidOperationException("Cannot execute in transaction when a transaction is already active.");
        }

        try
        {
            await BeginTransactionAsync(cancellationToken);
            
            await action();
            
            await SaveChangesAsync(cancellationToken);
            await CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await RollbackTransactionAsync(cancellationToken);
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
            try
            {
                if (_transaction != null)
                {
                    _logger.LogWarning("Disposing UnitOfWork with active transaction. Rolling back.");
                    _transaction.Rollback();
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing UnitOfWork");
            }
        }
        _disposed = true;
    }
} 
