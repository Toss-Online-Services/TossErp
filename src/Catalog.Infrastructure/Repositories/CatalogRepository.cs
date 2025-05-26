using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class CatalogRepository : ICatalogRepository
{
    private readonly CatalogContext _context;

    public CatalogRepository(CatalogContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await _context.Products
            .Include(p => p.CatalogBrand)
            .Include(p => p.CatalogType)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.CatalogBrand)
            .Include(p => p.CatalogType)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId)
    {
        return await _context.Products
            .Include(p => p.CatalogBrand)
            .Include(p => p.CatalogType)
            .Where(p => p.CatalogBrandId == brandId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByTypeAsync(int typeId)
    {
        return await _context.Products
            .Include(p => p.CatalogBrand)
            .Include(p => p.CatalogType)
            .Where(p => p.CatalogTypeId == typeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<CatalogBrand>> GetBrandsAsync()
    {
        return await _context.CatalogBrands.ToListAsync();
    }

    public async Task<IEnumerable<CatalogType>> GetTypesAsync()
    {
        return await _context.CatalogTypes.ToListAsync();
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
} 
