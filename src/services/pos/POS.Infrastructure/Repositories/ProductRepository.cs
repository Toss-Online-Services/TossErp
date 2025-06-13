using TossErp.POS.Domain.AggregatesModel.ProductAggregate;
using TossErp.POS.Domain.Common;
using TossErp.POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TossErp.POS.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly POSContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public ProductRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Product?> GetByIdAsync(string id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public void Update(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
    }

    public void Delete(Product product)
    {
        _context.Products.Remove(product);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<Product?> GetByCodeAsync(string code)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Code == code);
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        return await _context.Products
            .Where(p => p.Category == category)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByBrandAsync(string brand)
    {
        return await _context.Products
            .Where(p => p.Brand == brand)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _context.Products
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByStockLevelAsync(int threshold)
    {
        return await _context.Products
            .Where(p => p.StockLevel <= threshold)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByStoreIdAsync(string storeId)
    {
        return await _context.Products
            .Where(p => p.StoreId == storeId)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category, string storeId)
    {
        return await _context.Products
            .Where(p => p.Category == category && p.StoreId == storeId)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product?> GetAsync(string productId)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<IEnumerable<Product>> GetLowStockAsync(string storeId, int threshold)
    {
        return await _context.Products
            .Where(p => p.StoreId == storeId && p.StockQuantity <= threshold)
            .OrderBy(p => p.StockQuantity)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(string storeId, string code)
    {
        return await _context.Products.AnyAsync(p => p.StoreId == storeId && p.Code == code);
    }
} 
