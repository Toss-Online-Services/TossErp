using Microsoft.EntityFrameworkCore;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Infrastructure.Data;

namespace TossErp.Stock.Infrastructure.Repositories;

public class StockEntryTypeRepository : Repository<StockEntryType>, IStockEntryTypeRepository
{
    public StockEntryTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<StockEntryType?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<StockEntryType>> GetByPurposeAsync(StockEntryPurpose purpose, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().Where(x => x.Purpose == purpose).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockEntryType>> GetByAddToTransitAsync(bool addToTransit, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().Where(x => x.AddToTransit == addToTransit).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockEntryType>> GetByAllowNegativeStockAsync(bool allowNegativeStock, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().Where(x => x.AllowNegativeStock == allowNegativeStock).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<bool> IsDisabledAsync(string name, CancellationToken cancellationToken = default)
    {
        var entryType = await GetQueryable().FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        return entryType?.IsDisabled == true;
    }

    public async Task<long> GetCountByPurposeAsync(StockEntryPurpose purpose, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().LongCountAsync(x => x.Purpose == purpose, cancellationToken);
    }

    public new IQueryable<StockEntryType> GetQueryable()
    {
        return base.GetQueryable();
    }
}
