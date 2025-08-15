using Microsoft.EntityFrameworkCore;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Infrastructure.Data;

namespace TossErp.Stock.Infrastructure.Repositories;

public class ItemGroupRepository : Repository<ItemGroup>, IItemGroupRepository
{
    public ItemGroupRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ItemGroup?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<ItemGroup?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<ItemGroup>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await GetQueryable().Where(x => x.IsActive).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ItemGroup>> GetInactiveAsync(CancellationToken cancellationToken = default)
    {
        return await GetQueryable().Where(x => !x.IsActive).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().AnyAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await GetQueryable().AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<long> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await GetQueryable().LongCountAsync(x => x.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<ItemGroup>> GetGroupGroupsAsync(CancellationToken cancellationToken = default)
    {
        // Since ItemGroup doesn't have hierarchical structure, return all active groups
        return await GetQueryable().Where(x => x.IsActive).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ItemGroup>> GetLeafGroupsAsync(CancellationToken cancellationToken = default)
    {
        // Since ItemGroup doesn't have hierarchical structure, return all active groups
        return await GetQueryable().Where(x => x.IsActive).ToListAsync(cancellationToken);
    }

    public Task<IEnumerable<ItemGroup>> GetChildrenAsync(string parentGroupName, CancellationToken cancellationToken = default)
    {
        // Since ItemGroup doesn't have hierarchical structure, return empty collection
        return Task.FromResult<IEnumerable<ItemGroup>>(new List<ItemGroup>());
    }

    public Task<ItemGroup?> GetParentAsync(string groupName, CancellationToken cancellationToken = default)
    {
        // Since ItemGroup doesn't have hierarchical structure, return null
        return Task.FromResult<ItemGroup?>(null);
    }

    public Task<IEnumerable<ItemGroup>> GetAncestorsAsync(string groupName, CancellationToken cancellationToken = default)
    {
        // Since ItemGroup doesn't have hierarchical structure, return empty collection
        return Task.FromResult<IEnumerable<ItemGroup>>(new List<ItemGroup>());
    }

    public Task<IEnumerable<ItemGroup>> GetDescendantsAsync(string groupName, CancellationToken cancellationToken = default)
    {
        // Since ItemGroup doesn't have hierarchical structure, return empty collection
        return Task.FromResult<IEnumerable<ItemGroup>>(new List<ItemGroup>());
    }

    public async Task<IEnumerable<ItemGroup>> GetByShowInWebsiteAsync(bool showInWebsite, CancellationToken cancellationToken = default)
    {
        // Since ItemGroup doesn't have ShowInWebsite property, return all active groups
        return await GetQueryable().Where(x => x.IsActive).ToListAsync(cancellationToken);
    }

    public Task<bool> IsGroupAsync(string name, CancellationToken cancellationToken = default)
    {
        // Since ItemGroup doesn't have IsGroup property, return false
        return Task.FromResult(false);
    }

    public Task<bool> IsLeafAsync(string name, CancellationToken cancellationToken = default)
    {
        // Since ItemGroup doesn't have hierarchical structure, return true for all groups
        return Task.FromResult(true);
    }

    public async Task<bool> IsDisabledAsync(string name, CancellationToken cancellationToken = default)
    {
        var group = await GetQueryable().FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        return group?.IsActive == false;
    }

    public async Task<long> GetGroupCountAsync(CancellationToken cancellationToken = default)
    {
        // Since ItemGroup doesn't have hierarchical structure, return total count
        return await GetQueryable().LongCountAsync(cancellationToken);
    }

    public async Task<long> GetLeafCountAsync(CancellationToken cancellationToken = default)
    {
        // Since ItemGroup doesn't have hierarchical structure, return total count
        return await GetQueryable().LongCountAsync(cancellationToken);
    }

    public new IQueryable<ItemGroup> GetQueryable()
    {
        return base.GetQueryable();
    }
}
