using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TossErp.Domain.SeedWork
{
    /// <summary>
    /// Generic repository interface for aggregate roots.
    /// </summary>
    /// <typeparam name="TEntity">Aggregate root type.</typeparam>
    public interface IRepository<TEntity> where TEntity : IAggregateRoot
    {
        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }
} 
