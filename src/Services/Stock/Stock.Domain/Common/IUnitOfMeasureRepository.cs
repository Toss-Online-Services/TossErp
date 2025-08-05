using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.Domain.Common;

public interface IUnitOfMeasureRepository
{
    Task<UnitOfMeasure?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UnitOfMeasure?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<UnitOfMeasure>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UnitOfMeasure> AddAsync(UnitOfMeasure entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(UnitOfMeasure entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(UnitOfMeasure entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(CancellationToken cancellationToken = default);
} 
