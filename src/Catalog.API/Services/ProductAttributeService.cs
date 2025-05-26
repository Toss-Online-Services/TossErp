using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Services;

public class ProductAttributeService : IProductAttributeService
{
    private readonly CatalogContext _context;

    public ProductAttributeService(CatalogContext context)
    {
        _context = context;
    }

    public async Task<ProductAttribute?> GetProductAttributeByIdAsync(int productAttributeId)
    {
        return await _context.ProductAttributes
            .FirstOrDefaultAsync(pa => pa.Id == productAttributeId);
    }

    public async Task<IEnumerable<ProductAttribute>> GetAllProductAttributesAsync()
    {
        return await _context.ProductAttributes.ToListAsync();
    }

    public async Task<IEnumerable<ProductAttribute>> GetProductAttributesByIdsAsync(int[] productAttributeIds)
    {
        return await _context.ProductAttributes
            .Where(pa => productAttributeIds.Contains(pa.Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAttribute>> GetProductAttributesByNameAsync(string name)
    {
        return await _context.ProductAttributes
            .Where(pa => pa.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAttribute>> GetProductAttributesByDescriptionAsync(string description)
    {
        return await _context.ProductAttributes
            .Where(pa => pa.Description.Contains(description))
            .ToListAsync();
    }

    public async Task<ProductAttribute> InsertProductAttributeAsync(ProductAttribute productAttribute)
    {
        await _context.ProductAttributes.AddAsync(productAttribute);
        await _context.SaveChangesAsync();
        return productAttribute;
    }

    public async Task<ProductAttribute> UpdateProductAttributeAsync(ProductAttribute productAttribute)
    {
        _context.ProductAttributes.Update(productAttribute);
        await _context.SaveChangesAsync();
        return productAttribute;
    }

    public async Task DeleteProductAttributeAsync(ProductAttribute productAttribute)
    {
        _context.ProductAttributes.Remove(productAttribute);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAttributesAsync(IList<ProductAttribute> productAttributes)
    {
        _context.ProductAttributes.RemoveRange(productAttributes);
        await _context.SaveChangesAsync();
    }

    public async Task<IList<ProductAttribute>> GetAllProductAttributesAsync(bool showHidden = false)
    {
        var query = _context.ProductAttributes.AsQueryable();
        
        if (!showHidden)
        {
            query = query.Where(pa => pa.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<ProductAttribute>> GetProductAttributesByIdsAsync(int[] productAttributeIds, bool showHidden = false)
    {
        var query = _context.ProductAttributes
            .Where(pa => productAttributeIds.Contains(pa.Id));

        if (!showHidden)
        {
            query = query.Where(pa => pa.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<ProductAttribute>> GetProductAttributesByNameAsync(string name, bool showHidden = false)
    {
        var query = _context.ProductAttributes
            .Where(pa => pa.Name.Contains(name));

        if (!showHidden)
        {
            query = query.Where(pa => pa.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<ProductAttribute>> GetProductAttributesByDescriptionAsync(string description, bool showHidden = false)
    {
        var query = _context.ProductAttributes
            .Where(pa => pa.Description.Contains(description));

        if (!showHidden)
        {
            query = query.Where(pa => pa.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<ProductAttribute>> GetAllProductAttributesAsync(bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.ProductAttributes.AsQueryable();

        if (!showHidden)
        {
            query = query.Where(pa => pa.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<ProductAttribute>> GetProductAttributesByIdsAsync(int[] productAttributeIds, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.ProductAttributes
            .Where(pa => productAttributeIds.Contains(pa.Id));

        if (!showHidden)
        {
            query = query.Where(pa => pa.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<ProductAttribute>> GetProductAttributesByNameAsync(string name, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.ProductAttributes
            .Where(pa => pa.Name.Contains(name));

        if (!showHidden)
        {
            query = query.Where(pa => pa.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<ProductAttribute>> GetProductAttributesByDescriptionAsync(string description, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.ProductAttributes
            .Where(pa => pa.Description.Contains(description));

        if (!showHidden)
        {
            query = query.Where(pa => pa.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
} 
