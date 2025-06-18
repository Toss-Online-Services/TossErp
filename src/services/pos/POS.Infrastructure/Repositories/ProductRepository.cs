#nullable enable

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.SeedWork;
using POS.Infrastructure.EntityConfigurations;

namespace POS.Infrastructure.Repositories;

public class ProductRepository : IRepository<Product>
{
    private readonly POSContext _context;

    public ProductRepository(POSContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Products.AddAsync(product, cancellationToken);
        return entry.Entity;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            //.Include(p => p.Categories)
            //.Include(p => p.Variants)
            //.Include(p => p.Attributes)
            //.Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            //.Include(p => p.Categories)
            //.Include(p => p.Variants)
            //.Include(p => p.Attributes)
            //.Include(p => p.Images)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAsync(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            //.Include(p => p.Categories)
            //.Include(p => p.Variants)
            //.Include(p => p.Attributes)
            //.Include(p => p.Images)
            .Where(predicate)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Entry(product).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Remove(product);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
        if (product != null)
        {
            _context.Products.Remove(product);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Products.CountAsync(predicate, cancellationToken);
    }

    // Overloads without CancellationToken
    public Task<Product> AddAsync(Product product) => AddAsync(product, default);
    public Task UpdateAsync(Product product) => UpdateAsync(product, default);
    public Task DeleteAsync(Product product) => DeleteAsync(product, default);

    public async Task<Product?> GetBySkuAsync(string sku)
    {
        return await _context.Products
            //.Include(p => p.Categories)
            //.Include(p => p.Variants)
            //.Include(p => p.Attributes)
            //.Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.SKU == sku);
    }

    // Commented out: GetByCategoryAsync, since Product does not have Categories navigation
    //public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId)
    //{
    //    return await _context.Products
    //        .Include(p => p.Categories)
    //        .Include(p => p.Variants)
    //        .Include(p => p.Attributes)
    //        .Include(p => p.Images)
    //        .Where(p => p.Categories.Any(c => c.Id == categoryId))
    //        .OrderBy(p => p.Name)
    //        .ToListAsync();
    //}

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await _context.Products
            //.Include(p => p.Categories)
            //.Include(p => p.Variants)
            //.Include(p => p.Attributes)
            //.Include(p => p.Images)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _context.Products
            //.Include(p => p.Categories)
            //.Include(p => p.Variants)
            //.Include(p => p.Attributes)
            //.Include(p => p.Images)
            .Where(p => p.Price.Amount >= minPrice && p.Price.Amount <= maxPrice)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByStockLevelAsync(int minStockLevel)
    {
        return await _context.Products
            //.Include(p => p.Categories)
            //.Include(p => p.Variants)
            //.Include(p => p.Attributes)
            //.Include(p => p.Images)
            .Where(p => p.StockQuantity <= minStockLevel)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public void Update(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
    }

    public void Delete(Product product)
    {
        _context.Products.Remove(product);
    }
} 
