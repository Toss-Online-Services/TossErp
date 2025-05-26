using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Services;

public class RecentlyViewedProductsService : IRecentlyViewedProductsService
{
    private readonly CatalogContext _context;
    private const int RECENTLY_VIEWED_PRODUCTS_NUMBER = 20;

    public RecentlyViewedProductsService(CatalogContext context)
    {
        _context = context;
    }

    public async Task<IList<Product>> GetRecentlyViewedProductsAsync(int customerId, int number)
    {
        return await GetRecentlyViewedProductsAsync(customerId, number, false);
    }

    public async Task AddProductToRecentlyViewedListAsync(int customerId, int productId)
    {
        var recentlyViewedProduct = await _context.RecentlyViewedProducts
            .FirstOrDefaultAsync(rvp => rvp.CustomerId == customerId && rvp.ProductId == productId);

        if (recentlyViewedProduct == null)
        {
            recentlyViewedProduct = new RecentlyViewedProduct
            {
                CustomerId = customerId,
                ProductId = productId,
                ViewedOn = DateTime.UtcNow
            };
            await _context.RecentlyViewedProducts.AddAsync(recentlyViewedProduct);
        }
        else
        {
            recentlyViewedProduct.ViewedOn = DateTime.UtcNow;
            _context.RecentlyViewedProducts.Update(recentlyViewedProduct);
        }

        await _context.SaveChangesAsync();
    }

    public async Task ClearRecentlyViewedProductsAsync(int customerId)
    {
        var recentlyViewedProducts = await _context.RecentlyViewedProducts
            .Where(rvp => rvp.CustomerId == customerId)
            .ToListAsync();

        _context.RecentlyViewedProducts.RemoveRange(recentlyViewedProducts);
        await _context.SaveChangesAsync();
    }

    public async Task<IList<Product>> GetRecentlyViewedProductsAsync(int customerId, int number, bool showHidden = false)
    {
        return await GetRecentlyViewedProductsAsync(customerId, number, showHidden, null);
    }

    public async Task<IList<Product>> GetRecentlyViewedProductsAsync(int customerId, int number, bool showHidden = false, int? storeId = null)
    {
        return await GetRecentlyViewedProductsAsync(customerId, number, showHidden, storeId, null);
    }

    public async Task<IList<Product>> GetRecentlyViewedProductsAsync(int customerId, int number, bool showHidden = false, int? storeId = null, int? languageId = null)
    {
        return await GetRecentlyViewedProductsAsync(customerId, number, showHidden, storeId, languageId, 0, number);
    }

    public async Task<IList<Product>> GetRecentlyViewedProductsAsync(int customerId, int number, bool showHidden = false, int? storeId = null, int? languageId = null, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.RecentlyViewedProducts
            .Where(rvp => rvp.CustomerId == customerId)
            .OrderByDescending(rvp => rvp.ViewedOn)
            .Select(rvp => rvp.Product);

        if (!showHidden)
        {
            query = query.Where(p => p.Published);
        }

        if (storeId.HasValue)
        {
            query = query.Where(p => p.StoreId == storeId.Value);
        }

        if (languageId.HasValue)
        {
            query = query.Where(p => p.LanguageId == languageId.Value);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
} 
