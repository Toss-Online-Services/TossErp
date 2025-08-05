using TossErp.Stock.Domain.Entities;

namespace TossErp.Stock.Domain.Common;

public interface IItemGroupRepository : IRepository<ItemGroup>
{
    Task<ItemGroup?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<ItemGroup?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemGroup>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemGroup>> GetInactiveAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<long> GetActiveCountAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemGroup>> GetGroupGroupsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemGroup>> GetLeafGroupsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemGroup>> GetChildrenAsync(string parentGroupName, CancellationToken cancellationToken = default);
    Task<ItemGroup?> GetParentAsync(string groupName, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemGroup>> GetAncestorsAsync(string groupName, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemGroup>> GetDescendantsAsync(string groupName, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemGroup>> GetByShowInWebsiteAsync(bool showInWebsite, CancellationToken cancellationToken = default);
    Task<bool> IsGroupAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> IsLeafAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> IsDisabledAsync(string name, CancellationToken cancellationToken = default);
    Task<long> GetGroupCountAsync(CancellationToken cancellationToken = default);
    Task<long> GetLeafCountAsync(CancellationToken cancellationToken = default);
} 
