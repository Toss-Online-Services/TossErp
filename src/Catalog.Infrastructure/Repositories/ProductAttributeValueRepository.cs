using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class ProductAttributeValueRepository : IProductAttributeValueRepository
{
    private readonly CatalogContext _context;

    public ProductAttributeValueRepository(CatalogContext context)
    {
        _context = context;
    }

    public async Task<ProductAttributeValue?> GetAsync(int id)
    {
        return await _context.ProductAttributeValues
            .Include(pav => pav.Product)
            .Include(pav => pav.Attribute)
            .FirstOrDefaultAsync(pav => pav.Id == id);
    }

    public async Task<IEnumerable<ProductAttributeValue>> GetAllAsync()
    {
        return await _context.ProductAttributeValues
            .Include(pav => pav.Product)
            .Include(pav => pav.Attribute)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAttributeValue>> GetByProductAsync(int productId)
    {
        return await _context.ProductAttributeValues
            .Include(pav => pav.Product)
            .Include(pav => pav.Attribute)
            .Where(pav => pav.ProductId == productId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAttributeValue>> GetByAttributeAsync(int attributeId)
    {
        return await _context.ProductAttributeValues
            .Include(pav => pav.Product)
            .Include(pav => pav.Attribute)
            .Where(pav => pav.AttributeId == attributeId)
            .ToListAsync();
    }

    public async Task<ProductAttributeValue?> GetByProductAndAttributeAsync(int productId, int attributeId)
    {
        return await _context.ProductAttributeValues
            .Include(pav => pav.Product)
            .Include(pav => pav.Attribute)
            .FirstOrDefaultAsync(pav => pav.ProductId == productId && pav.AttributeId == attributeId);
    }

    public async Task<ProductAttributeValue> AddAsync(ProductAttributeValue value)
    {
        await _context.ProductAttributeValues.AddAsync(value);
        await _context.SaveChangesAsync();
        return value;
    }

    public async Task<ProductAttributeValue> UpdateAsync(ProductAttributeValue value)
    {
        _context.ProductAttributeValues.Update(value);
        await _context.SaveChangesAsync();
        return value;
    }

    public async Task DeleteAsync(int valueId)
    {
        var value = await _context.ProductAttributeValues.FindAsync(valueId);
        if (value != null)
        {
            _context.ProductAttributeValues.Remove(value);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int valueId)
    {
        return await _context.ProductAttributeValues.AnyAsync(pav => pav.Id == valueId);
    }

    public async Task<bool> ExistsByNameAsync(int productId, int attributeId, string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        return await _context.ProductAttributeValues
            .AnyAsync(pav => pav.ProductId == productId &&
                            pav.AttributeId == attributeId &&
                            pav.Name == name);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.ProductAttributeValues.CountAsync();
    }

    public async Task<int> GetNextDisplayOrderAsync(int productId, int attributeId)
    {
        var maxDisplayOrder = await _context.ProductAttributeValues
            .Where(pav => pav.ProductId == productId &&
                         pav.AttributeId == attributeId)
            .MaxAsync(pav => (int?)pav.DisplayOrder) ?? 0;
        return maxDisplayOrder + 1;
    }
} 
