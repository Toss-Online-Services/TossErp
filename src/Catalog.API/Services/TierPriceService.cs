using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Services;

public class TierPriceService : ITierPriceService
{
    private readonly CatalogContext _context;

    public TierPriceService(CatalogContext context)
    {
        _context = context;
    }

    public async Task<TierPrice?> GetTierPriceByIdAsync(int tierPriceId)
    {
        return await _context.TierPrices
            .Include(tp => tp.Product)
            .Include(tp => tp.CustomerRole)
            .Include(tp => tp.Store)
            .FirstOrDefaultAsync(tp => tp.Id == tierPriceId);
    }

    public async Task<IEnumerable<TierPrice>> GetTierPricesByProductIdAsync(int productId)
    {
        return await _context.TierPrices
            .Include(tp => tp.Product)
            .Include(tp => tp.CustomerRole)
            .Include(tp => tp.Store)
            .Where(tp => tp.ProductId == productId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TierPrice>> GetTierPricesByCustomerRoleIdAsync(int customerRoleId)
    {
        return await _context.TierPrices
            .Include(tp => tp.Product)
            .Include(tp => tp.CustomerRole)
            .Include(tp => tp.Store)
            .Where(tp => tp.CustomerRoleId == customerRoleId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TierPrice>> GetTierPricesByStoreIdAsync(int storeId)
    {
        return await _context.TierPrices
            .Include(tp => tp.Product)
            .Include(tp => tp.CustomerRole)
            .Include(tp => tp.Store)
            .Where(tp => tp.StoreId == storeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TierPrice>> GetTierPricesByQuantityAsync(int productId, int quantity)
    {
        return await _context.TierPrices
            .Include(tp => tp.Product)
            .Include(tp => tp.CustomerRole)
            .Include(tp => tp.Store)
            .Where(tp => tp.ProductId == productId && tp.Quantity <= quantity)
            .OrderByDescending(tp => tp.Quantity)
            .ToListAsync();
    }

    public async Task<TierPrice> InsertTierPriceAsync(TierPrice tierPrice)
    {
        await _context.TierPrices.AddAsync(tierPrice);
        await _context.SaveChangesAsync();
        return tierPrice;
    }

    public async Task<TierPrice> UpdateTierPriceAsync(TierPrice tierPrice)
    {
        _context.TierPrices.Update(tierPrice);
        await _context.SaveChangesAsync();
        return tierPrice;
    }

    public async Task DeleteTierPriceAsync(TierPrice tierPrice)
    {
        _context.TierPrices.Remove(tierPrice);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTierPricesAsync(IList<TierPrice> tierPrices)
    {
        _context.TierPrices.RemoveRange(tierPrices);
        await _context.SaveChangesAsync();
    }

    public async Task<IList<TierPrice>> GetTierPricesByProductIdAsync(int productId, bool showHidden = false)
    {
        var query = _context.TierPrices
            .Include(tp => tp.Product)
            .Include(tp => tp.CustomerRole)
            .Include(tp => tp.Store)
            .Where(tp => tp.ProductId == productId);

        if (!showHidden)
        {
            query = query.Where(tp => tp.Product.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<TierPrice>> GetTierPricesByCustomerRoleIdAsync(int customerRoleId, bool showHidden = false)
    {
        var query = _context.TierPrices
            .Include(tp => tp.Product)
            .Include(tp => tp.CustomerRole)
            .Include(tp => tp.Store)
            .Where(tp => tp.CustomerRoleId == customerRoleId);

        if (!showHidden)
        {
            query = query.Where(tp => tp.Product.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<TierPrice>> GetTierPricesByStoreIdAsync(int storeId, bool showHidden = false)
    {
        var query = _context.TierPrices
            .Include(tp => tp.Product)
            .Include(tp => tp.CustomerRole)
            .Include(tp => tp.Store)
            .Where(tp => tp.StoreId == storeId);

        if (!showHidden)
        {
            query = query.Where(tp => tp.Product.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<TierPrice>> GetTierPricesByQuantityAsync(int productId, int quantity, bool showHidden = false)
    {
        var query = _context.TierPrices
            .Include(tp => tp.Product)
            .Include(tp => tp.CustomerRole)
            .Include(tp => tp.Store)
            .Where(tp => tp.ProductId == productId && tp.Quantity <= quantity)
            .OrderByDescending(tp => tp.Quantity);

        if (!showHidden)
        {
            query = query.Where(tp => tp.Product.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<TierPrice>> GetTierPricesByProductIdAsync(int productId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.TierPrices
            .Include(tp => tp.Product)
            .Include(tp => tp.CustomerRole)
            .Include(tp => tp.Store)
            .Where(tp => tp.ProductId == productId);

        if (!showHidden)
        {
            query = query.Where(tp => tp.Product.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<TierPrice>> GetTierPricesByCustomerRoleIdAsync(int customerRoleId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.TierPrices
            .Include(tp => tp.Product)
            .Include(tp => tp.CustomerRole)
            .Include(tp => tp.Store)
            .Where(tp => tp.CustomerRoleId == customerRoleId);

        if (!showHidden)
        {
            query = query.Where(tp => tp.Product.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<TierPrice>> GetTierPricesByStoreIdAsync(int storeId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.TierPrices
            .Include(tp => tp.Product)
            .Include(tp => tp.CustomerRole)
            .Include(tp => tp.Store)
            .Where(tp => tp.StoreId == storeId);

        if (!showHidden)
        {
            query = query.Where(tp => tp.Product.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<TierPrice>> GetTierPricesByQuantityAsync(int productId, int quantity, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.TierPrices
            .Include(tp => tp.Product)
            .Include(tp => tp.CustomerRole)
            .Include(tp => tp.Store)
            .Where(tp => tp.ProductId == productId && tp.Quantity <= quantity)
            .OrderByDescending(tp => tp.Quantity);

        if (!showHidden)
        {
            query = query.Where(tp => tp.Product.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
} 
