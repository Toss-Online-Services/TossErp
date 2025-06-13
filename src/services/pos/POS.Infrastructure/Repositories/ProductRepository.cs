using Microsoft.EntityFrameworkCore;
using eShop.POS.Domain.AggregatesModel.ProductAggregate;
using eShop.POS.Domain.Repositories;
using eShop.POS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShop.POS.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly POSContext _context;

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
        return await _context.Products
            .ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string productId)
    {
        return await _context.Products.AnyAsync(p => p.Id == productId);
    }

    public async Task<Product?> GetByCodeAsync(string code)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Code == code);
    }

    public async Task<Product> GetByNameAsync(string name)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category, string storeId)
    {
        return await _context.Products
            .Where(p => p.Category == category && p.StoreId == storeId)
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

    public async Task<Product?> GetAsync(string productId)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<IEnumerable<Product>> GetByStoreAsync(string storeId)
    {
        return await _context.Products
            .Where(p => p.StoreId == storeId)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetLowStockAsync(string storeId, int threshold)
    {
        return await _context.Products
            .Where(p => p.StoreId == storeId && p.StockQuantity <= threshold)
            .OrderBy(p => p.StockQuantity)
            .ToListAsync();
    }

    public async Task DeleteAsync(string productId)
    {
        var product = await GetAsync(productId);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string storeId, string code)
    {
        return await _context.Products.AnyAsync(p => p.StoreId == storeId && p.Code == code);
    }
} 
