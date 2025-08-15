namespace TossErp.Stock.Domain.Common;

/// <summary>
/// Unit of Work pattern interface for managing database transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets whether a transaction is currently active
    /// </summary>
    bool HasActiveTransaction { get; }

    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The database transaction</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action within a transaction scope
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the action</returns>
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action within a transaction scope
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
} 
