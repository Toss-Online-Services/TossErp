using System.Linq.Expressions;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.Repositories;
using TossErp.POS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using IProductRepository = POS.Domain.Repositories.IProductRepository;

namespace TossErp.POS.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly POSContext _context;

    public ProductRepository(POSContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> FindAsync(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task AddAsync(Product entity, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
    }

    public async Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _context.Products.Where(p => p.Category == category).ToListAsync(cancellationToken);
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

    public async Task<IEnumerable<Product>> GetByStoreIdAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.StoreId == storeId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
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

    public async Task<IEnumerable<Product>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.SKU == sku)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => p.Barcode == barcode)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
} 
