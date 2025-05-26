using Microsoft.EntityFrameworkCore.Storage;

namespace Catalog.Domain.SeedWork;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(IDbContextTransaction transaction);
    void RollbackTransaction();
    bool HasActiveTransaction { get; }
    IDbContextTransaction GetCurrentTransaction();
} 
