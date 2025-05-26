using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CatalogContext _context;

    public ProductRepository(CatalogContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.CatalogBrand)
            .Include(p => p.CatalogType)
            .Include(p => p.Pictures)
            .Include(p => p.ProductReviews)
            .Include(p => p.ProductAttributeMappings)
                .ThenInclude(pam => pam.ProductAttribute)
            .Include(p => p.ProductAttributeMappings)
                .ThenInclude(pam => pam.ProductAttributeValues)
            .Include(p => p.RelatedProducts)
                .ThenInclude(rp => rp.Product2)
            .Include(p => p.CrossSellProducts)
                .ThenInclude(csp => csp.Product2)
            .Include(p => p.ProductTags)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.CatalogBrand)
            .Include(p => p.CatalogType)
            .Include(p => p.Pictures)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.CatalogBrand)
            .Include(p => p.CatalogType)
            .Include(p => p.Pictures)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<Product> AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
} 
