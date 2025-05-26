using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class ProductAttributeRepository : IProductAttributeRepository
{
    private readonly CatalogContext _context;

    public ProductAttributeRepository(CatalogContext context)
    {
        _context = context;
    }

    public async Task<ProductAttribute> GetAsync(int id)
    {
        return await _context.ProductAttributes
            .Include(pa => pa.ProductAttributeMappings)
            .FirstOrDefaultAsync(pa => pa.Id == id) ?? throw new InvalidOperationException($"ProductAttribute with id {id} not found");
    }

    public async Task<IEnumerable<ProductAttribute>> GetAllAsync()
    {
        return await _context.ProductAttributes
            .Include(pa => pa.ProductAttributeMappings)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAttribute>> GetByProductAsync(int productId)
    {
        return await _context.ProductAttributes
            .Include(pa => pa.ProductAttributeMappings)
            .Where(pa => pa.ProductAttributeMappings.Any(pam => pam.ProductId == productId))
            .ToListAsync();
    }

    public async Task<ProductAttribute> AddAsync(ProductAttribute attribute)
    {
        await _context.ProductAttributes.AddAsync(attribute);
        await _context.SaveChangesAsync();
        return attribute;
    }

    public async Task UpdateAsync(ProductAttribute attribute)
    {
        _context.ProductAttributes.Update(attribute);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int attributeId)
    {
        var attribute = await _context.ProductAttributes.FindAsync(attributeId);
        if (attribute != null)
        {
            _context.ProductAttributes.Remove(attribute);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int attributeId)
    {
        return await _context.ProductAttributes.AnyAsync(pa => pa.Id == attributeId);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.ProductAttributes.CountAsync();
    }
} 
