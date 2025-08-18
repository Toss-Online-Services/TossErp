using TossErp.Sales.Application.Common.Interfaces;

namespace TossErp.Sales.API.Services;

/// <summary>
/// Implementation of IUnitOfWork for API
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(ILogger<UnitOfWork> logger)
    {
        _logger = logger;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // For MVP, this is a no-op since we're not using EF Core yet
        _logger.LogInformation("SaveChangesAsync called - no-op for MVP");
        return Task.FromResult(1);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // For MVP, this is a no-op since we're not using EF Core yet
        _logger.LogInformation("BeginTransactionAsync called - no-op for MVP");
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        // For MVP, this is a no-op since we're not using EF Core yet
        _logger.LogInformation("CommitTransactionAsync called - no-op for MVP");
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        // For MVP, this is a no-op since we're not using EF Core yet
        _logger.LogInformation("RollbackTransactionAsync called - no-op for MVP");
        return Task.CompletedTask;
    }

    public bool HasActiveTransaction => false; // For MVP, always false
}
