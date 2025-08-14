using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.ItemAggregate;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Domain.Enums;
using TossErp.Stock.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TossErp.Stock.Infrastructure.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly ApplicationDbContext _context;
    
    public ItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ItemAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StockUOM)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ItemAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StockUOM)
            .ToListAsync(cancellationToken);
    }

    public async Task<ItemAggregate> AddAsync(ItemAggregate entity, CancellationToken cancellationToken = default)
    {
        _context.Items.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(ItemAggregate entity, CancellationToken cancellationToken = default)
    {
        _context.Items.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ItemAggregate entity, CancellationToken cancellationToken = default)
    {
        _context.Items.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await GetByIdAsync(id, cancellationToken);
        if (item != null)
        {
            await DeleteAsync(item, cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Items.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Items.LongCountAsync(cancellationToken);
    }

    public void Add(ItemAggregate item)
    {
        _context.Items.Add(item);
    }

    public void Update(ItemAggregate item)
    {
        _context.Items.Update(item);
    }

    public void Delete(ItemAggregate item)
    {
        _context.Items.Remove(item);
    }

    public async Task<ItemAggregate?> GetByCodeAsync(ItemCode itemCode, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StockUOM)
            .FirstOrDefaultAsync(x => x.ItemCode.Value == itemCode.Value, cancellationToken);
    }

    public async Task<ItemAggregate?> GetByCodeAsync(string itemCode, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StockUOM)
            .FirstOrDefaultAsync(x => x.ItemCode.Value == itemCode, cancellationToken);
    }

    public async Task<IEnumerable<ItemAggregate>> GetByGroupAsync(string itemGroup, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StockUOM)
            .Where(x => x.ItemGroup == itemGroup)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ItemAggregate>> GetByTypeAsync(ItemType itemType, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StockUOM)
            .Where(x => x.ItemType == itemType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ItemAggregate>> GetByStatusAsync(ItemStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StockUOM)
            .Where(x => x.ItemStatus == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ItemAggregate>> GetStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StockUOM)
            .Where(x => x.IsStockItem)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ItemAggregate>> GetSalesItemsAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Implement when IsSalesItem property is added to ItemAggregate
        return await Task.FromResult<IEnumerable<ItemAggregate>>(Array.Empty<ItemAggregate>());
    }

    public async Task<IEnumerable<ItemAggregate>> GetPurchaseItemsAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Implement when IsPurchaseItem property is added to ItemAggregate
        return await Task.FromResult<IEnumerable<ItemAggregate>>(Array.Empty<ItemAggregate>());
    }

    public async Task<IEnumerable<ItemAggregate>> GetManufacturingItemsAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Implement when IsManufacturingItem property is added to ItemAggregate
        return await Task.FromResult<IEnumerable<ItemAggregate>>(Array.Empty<ItemAggregate>());
    }

    public async Task<IEnumerable<ItemAggregate>> GetByBrandAsync(string brand, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Include(x => x.StockUOM)
            .Where(x => x.Brand == brand)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ItemAggregate>> GetBySupplierAsync(string supplier, CancellationToken cancellationToken = default)
    {
        // Not implemented: ItemSupplier does not have SupplierId property
        return await Task.FromResult<IEnumerable<ItemAggregate>>(Array.Empty<ItemAggregate>());
    }

    public async Task<IEnumerable<ItemAggregate>> GetByCustomerAsync(string customer, CancellationToken cancellationToken = default)
    {
        // Not implemented: ItemCustomer does not have CustomerId property
        return await Task.FromResult<IEnumerable<ItemAggregate>>(Array.Empty<ItemAggregate>());
    }

    public async Task<IEnumerable<ItemAggregate>> GetExpiringItemsAsync(int daysThreshold, CancellationToken cancellationToken = default)
    {
        // Items don't have expiry dates in this domain model
        return await Task.FromResult<IEnumerable<ItemAggregate>>(Array.Empty<ItemAggregate>());
    }

    public async Task<IEnumerable<ItemAggregate>> GetLowStockItemsAsync(decimal threshold, CancellationToken cancellationToken = default)
    {
        // TODO: Implement when WarehouseInventories relationship is established
        return await Task.FromResult<IEnumerable<ItemAggregate>>(Array.Empty<ItemAggregate>());
    }

    public async Task<IEnumerable<ItemAggregate>> GetOverstockItemsAsync(decimal threshold, CancellationToken cancellationToken = default)
    {
        // TODO: Implement when WarehouseInventories relationship is established
        return await Task.FromResult<IEnumerable<ItemAggregate>>(Array.Empty<ItemAggregate>());
    }

    public async Task<bool> ExistsByCodeAsync(ItemCode itemCode, CancellationToken cancellationToken = default)
    {
        return await _context.Items.AnyAsync(x => x.ItemCode.Value == itemCode.Value, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string itemCode, CancellationToken cancellationToken = default)
    {
        return await _context.Items.AnyAsync(x => x.ItemCode.Value == itemCode, cancellationToken);
    }

    public async Task<long> GetCountByGroupAsync(string itemGroup, CancellationToken cancellationToken = default)
    {
        return await _context.Items.LongCountAsync(x => x.ItemGroup == itemGroup, cancellationToken);
    }

    public async Task<long> GetCountByTypeAsync(ItemType itemType, CancellationToken cancellationToken = default)
    {
        return await _context.Items.LongCountAsync(x => x.ItemType == itemType, cancellationToken);
    }

    public async Task<long> GetCountByStatusAsync(ItemStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Items.LongCountAsync(x => x.ItemStatus == status, cancellationToken);
    }

    public async Task<IEnumerable<ItemAggregate>> GetItemsAsync(
        string? itemCode = null, 
        string? itemName = null, 
        string? itemGroup = null, 
        ItemType? itemType = null, 
        bool? isStockItem = null, 
        bool? disabled = null, 
        int page = 1, 
        int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Items.AsQueryable();

        if (!string.IsNullOrWhiteSpace(itemCode))
            query = query.Where(x => x.ItemCode.Value.Contains(itemCode));

        if (!string.IsNullOrWhiteSpace(itemName))
            query = query.Where(x => x.ItemName.Contains(itemName));

        if (!string.IsNullOrWhiteSpace(itemGroup))
            query = query.Where(x => x.ItemGroup == itemGroup);

        if (itemType.HasValue)
            query = query.Where(x => x.ItemType == itemType.Value);

        if (isStockItem.HasValue)
            query = query.Where(x => x.IsStockItem == isStockItem.Value);

        if (disabled.HasValue)
            query = query.Where(x => x.Disabled == disabled.Value);

        return await query
            .Include(x => x.StockUOM)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    // Missing interface methods
    public Task<IEnumerable<ItemAggregate>> GetByCompanyAsync(string company, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<ItemAggregate>>(Array.Empty<ItemAggregate>());
    public Task<IEnumerable<ItemAggregate>> GetItemsNeedingReorderAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<ItemAggregate>>(Array.Empty<ItemAggregate>());
    public Task<IEnumerable<ItemAggregate>> GetAvailableForSaleAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<ItemAggregate>>(Array.Empty<ItemAggregate>());
    public Task<IEnumerable<ItemAggregate>> GetAvailableForPurchaseAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<ItemAggregate>>(Array.Empty<ItemAggregate>());
    public Task<IEnumerable<ItemAggregate>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<ItemAggregate>>(Array.Empty<ItemAggregate>());

    public IQueryable<ItemAggregate> GetQueryable()
    {
        return _context.Items
            .Include(x => x.StockUOM)
            .AsQueryable();
    }
}
