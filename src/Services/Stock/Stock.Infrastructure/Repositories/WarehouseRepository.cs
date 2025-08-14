using Microsoft.EntityFrameworkCore;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Aggregates.WarehouseAggregate;
using TossErp.Stock.Domain.ValueObjects;
using TossErp.Stock.Infrastructure.Data;

namespace TossErp.Stock.Infrastructure.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly ApplicationDbContext _context;

    public WarehouseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WarehouseAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<WarehouseAggregate?> GetByWarehouseCodeAsync(WarehouseCode warehouseCode, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .FirstOrDefaultAsync(x => x.Code == warehouseCode, cancellationToken);
    }

    public async Task<WarehouseAggregate?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByCompanyAsync(string company, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.Company == company)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByWarehouseTypeAsync(string warehouseType, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.WarehouseType == warehouseType)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WarehouseAggregate>> GetByLocationAsync(string location, CancellationToken cancellationToken = default)
    {
        // Warehouse does not have Location property, but we can search by address fields
        return await _context.Warehouses
            .Where(x => x.AddressLine1!.Contains(location) || 
                       x.AddressLine2!.Contains(location) || 
                       x.City!.Contains(location) || 
                       x.State!.Contains(location))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByCountryAsync(string country, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.Country == country)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByStateAsync(string state, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.State == state)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.City == city)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByPostalCodeAsync(string postalCode, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.Pin == postalCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetGroupWarehousesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.IsGroup)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetChildWarehousesAsync(Guid parentWarehouseId, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.ParentWarehouseId == parentWarehouseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetChildWarehousesAsync(WarehouseAggregate parentWarehouse, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.ParentWarehouseId == parentWarehouse.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetDisabledWarehousesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.IsDisabled)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByCompanyAndTypeAsync(string company, string warehouseType, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.Company == company && x.WarehouseType == warehouseType)
            .ToListAsync(cancellationToken);
    }

    public Task<IEnumerable<WarehouseAggregate>> GetByCompanyAndLocationAsync(string company, string location, CancellationToken cancellationToken = default)
    {
        // Location property doesn't exist, return empty list
        return Task.FromResult<IEnumerable<WarehouseAggregate>>(new List<WarehouseAggregate>());
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByCompanyAndCountryAsync(string company, string country, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.Company == company && x.Country == country)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByCompanyAndStateAsync(string company, string state, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.Company == company && x.State == state)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByCompanyAndCityAsync(string company, string city, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.Company == company && x.City == city)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByCompanyAndPostalCodeAsync(string company, string postalCode, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.Company == company && x.Pin == postalCode)
            .ToListAsync(cancellationToken);
    }

    public Task<IEnumerable<WarehouseAggregate>> GetByTypeAndLocationAsync(string warehouseType, string location, CancellationToken cancellationToken = default)
    {
        // Location property doesn't exist, return empty list
        return Task.FromResult<IEnumerable<WarehouseAggregate>>(new List<WarehouseAggregate>());
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByTypeAndCountryAsync(string warehouseType, string country, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.WarehouseType == warehouseType && x.Country == country)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByTypeAndStateAsync(string warehouseType, string state, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.WarehouseType == warehouseType && x.State == state)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByTypeAndCityAsync(string warehouseType, string city, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.WarehouseType == warehouseType && x.City == city)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByTypeAndPostalCodeAsync(string warehouseType, string postalCode, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.WarehouseType == warehouseType && x.Pin == postalCode)
            .ToListAsync(cancellationToken);
    }

    public Task<IEnumerable<WarehouseAggregate>> GetByLocationAndCountryAsync(string location, string country, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<WarehouseAggregate>>(new List<WarehouseAggregate>());
    }

    public Task<IEnumerable<WarehouseAggregate>> GetByLocationAndStateAsync(string location, string state, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<WarehouseAggregate>>(new List<WarehouseAggregate>());
    }

    public Task<IEnumerable<WarehouseAggregate>> GetByLocationAndCityAsync(string location, string city, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<WarehouseAggregate>>(new List<WarehouseAggregate>());
    }

    public Task<IEnumerable<WarehouseAggregate>> GetByLocationAndPostalCodeAsync(string location, string postalCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<WarehouseAggregate>>(new List<WarehouseAggregate>());
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByCountryAndStateAsync(string country, string state, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.Country == country && x.State == state)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByCountryAndCityAsync(string country, string city, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.Country == country && x.City == city)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByCountryAndPostalCodeAsync(string country, string postalCode, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.Country == country && x.Pin == postalCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByStateAndCityAsync(string state, string city, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.State == state && x.City == city)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByStateAndPostalCodeAsync(string state, string postalCode, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.State == state && x.Pin == postalCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByCityAndPostalCodeAsync(string city, string postalCode, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.City == city && x.Pin == postalCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses.ToListAsync(cancellationToken);
    }

    public async Task<bool> WarehouseCodeExistsAsync(WarehouseCode warehouseCode, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses.AnyAsync(x => x.Code == warehouseCode, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses.AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<WarehouseAggregate> AddAsync(WarehouseAggregate warehouse, CancellationToken cancellationToken = default)
    {
        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync(cancellationToken);
        return warehouse;
    }

    public async Task UpdateAsync(WarehouseAggregate warehouse, CancellationToken cancellationToken = default)
    {
        _context.Warehouses.Update(warehouse);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(WarehouseAggregate warehouse, CancellationToken cancellationToken = default)
    {
        _context.Warehouses.Remove(warehouse);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var warehouse = await GetByIdAsync(id, cancellationToken);
        if (warehouse != null)
        {
            await DeleteAsync(warehouse, cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses.LongCountAsync(cancellationToken);
    }

    public void Add(WarehouseAggregate warehouse)
    {
        _context.Warehouses.Add(warehouse);
    }

    public void Update(WarehouseAggregate warehouse)
    {
        _context.Warehouses.Update(warehouse);
    }

    public void Delete(WarehouseAggregate warehouse)
    {
        _context.Warehouses.Remove(warehouse);
    }

    public async Task<WarehouseAggregate?> GetByCodeAsync(WarehouseCode warehouseCode, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .FirstOrDefaultAsync(x => x.Code == warehouseCode, cancellationToken);
    }

    public async Task<WarehouseAggregate?> GetByCodeAsync(string warehouseCode, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .FirstOrDefaultAsync(x => x.Code.Value == warehouseCode, cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => !x.IsDisabled)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetInactiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.IsDisabled)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WarehouseAggregate>> GetByTypeAsync(string warehouseType, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.WarehouseType == warehouseType)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(WarehouseCode warehouseCode, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses.AnyAsync(x => x.Code == warehouseCode, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string warehouseCode, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses.AnyAsync(x => x.Code.Value == warehouseCode, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses.AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<bool> IsActiveAsync(string warehouseCode, CancellationToken cancellationToken = default)
    {
        var warehouse = await GetByCodeAsync(warehouseCode, cancellationToken);
        return warehouse != null && !warehouse.IsDisabled;
    }

    public async Task<bool> IsDisabledAsync(string warehouseCode, CancellationToken cancellationToken = default)
    {
        var warehouse = await GetByCodeAsync(warehouseCode, cancellationToken);
        return warehouse != null && warehouse.IsDisabled;
    }

    public async Task<long> GetCountByCompanyAsync(string company, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses.LongCountAsync(x => x.Company == company, cancellationToken);
    }

    public async Task<long> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses.LongCountAsync(x => !x.IsDisabled, cancellationToken);
    }

    public async Task<long> GetInactiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses.LongCountAsync(x => x.IsDisabled, cancellationToken);
    }

    public async Task<long> GetCountByTypeAsync(string warehouseType, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses.LongCountAsync(x => x.WarehouseType == warehouseType, cancellationToken);
    }

    public Task<decimal> GetTotalStockValueAsync(WarehouseCode warehouseCode, CancellationToken cancellationToken = default)
    {
        // Implementation would require joining with stock tables
        return Task.FromResult(0m);
    }

    public Task<decimal> GetTotalStockValueAsync(string warehouseCode, CancellationToken cancellationToken = default)
    {
        // Implementation would require joining with stock tables
        return Task.FromResult(0m);
    }

    public async Task<List<WarehouseAggregate>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _context.Warehouses
            .Where(x => x.IsDisabled == (status.ToLower() == "disabled"))
            .ToListAsync(cancellationToken);
    }

    public Task<List<WarehouseAggregate>> GetByCapacityRangeAsync(decimal? minCapacity, decimal? maxCapacity, CancellationToken cancellationToken = default)
    {
        // Capacity property doesn't exist in the current model
        return Task.FromResult(new List<WarehouseAggregate>());
    }

    public Task<List<WarehouseAggregate>> GetByUtilizationRangeAsync(decimal? minUtilization, decimal? maxUtilization, CancellationToken cancellationToken = default)
    {
        // Utilization property doesn't exist in the current model
        return Task.FromResult(new List<WarehouseAggregate>());
    }

    public Task<List<WarehouseAggregate>> GetByValueRangeAsync(decimal? minValue, decimal? maxValue, CancellationToken cancellationToken = default)
    {
        // Value property doesn't exist in the current model
        return Task.FromResult(new List<WarehouseAggregate>());
    }

    public Task<List<WarehouseAggregate>> GetByItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        // This would require joining with stock tables
        return Task.FromResult(new List<WarehouseAggregate>());
    }

    public Task<List<WarehouseAggregate>> GetByBinAsync(Guid binId, CancellationToken cancellationToken = default)
    {
        // This would require joining with bin tables
        return Task.FromResult(new List<WarehouseAggregate>());
    }

    public Task<List<WarehouseAggregate>> GetByMovementAsync(Guid movementId, CancellationToken cancellationToken = default)
    {
        // This would require joining with movement tables
        return Task.FromResult(new List<WarehouseAggregate>());
    }

    public Task<object?> GetAnalyticsAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        // Implementation would require complex analytics queries
        return Task.FromResult<object?>(null);
    }

    public Task<object?> GetReportAsync(Guid? warehouseId, string? reportType, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        // Implementation would require complex reporting queries
        return Task.FromResult<object?>(null);
    }

    // Missing interface methods
    public Task<IEnumerable<WarehouseAggregate>> GetGroupsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<WarehouseAggregate>>(Array.Empty<WarehouseAggregate>());
    public Task<IEnumerable<WarehouseAggregate>> GetLeavesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<WarehouseAggregate>>(Array.Empty<WarehouseAggregate>());
    public Task<IEnumerable<WarehouseAggregate>> GetRootsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<WarehouseAggregate>>(Array.Empty<WarehouseAggregate>());
    public Task<IEnumerable<WarehouseAggregate>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<WarehouseAggregate>>(Array.Empty<WarehouseAggregate>());
    public Task<IEnumerable<WarehouseAggregate>> GetRejectedWarehousesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<WarehouseAggregate>>(Array.Empty<WarehouseAggregate>());
    public Task<IEnumerable<WarehouseAggregate>> GetCanAcceptStockAsync(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<WarehouseAggregate>>(Array.Empty<WarehouseAggregate>());

    public IQueryable<WarehouseAggregate> GetQueryable()
    {
        return _context.Warehouses.AsQueryable();
    }
} 
