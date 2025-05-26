using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class ProductAttributeMappingRepository : IProductAttributeMappingRepository
{
    private readonly CatalogContext _context;

    public ProductAttributeMappingRepository(CatalogContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ProductAttributeMapping> GetAsync(int mappingId)
    {
        return await _context.ProductAttributeMappings
            .Include(pam => pam.Product)
            .Include(pam => pam.ProductAttribute)
            .Include(pam => pam.ProductAttributeValues)
            .FirstOrDefaultAsync(pam => pam.Id == mappingId) ?? throw new InvalidOperationException($"ProductAttributeMapping with ID {mappingId} not found");
    }

    public async Task<IEnumerable<ProductAttributeMapping>> GetAllAsync()
    {
        return await _context.ProductAttributeMappings
            .Include(pam => pam.Product)
            .Include(pam => pam.ProductAttribute)
            .Include(pam => pam.ProductAttributeValues)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAttributeMapping>> GetByProductAsync(int productId)
    {
        return await _context.ProductAttributeMappings
            .Include(pam => pam.ProductAttribute)
            .Include(pam => pam.ProductAttributeValues)
            .Where(pam => pam.ProductId == productId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAttributeMapping>> GetByAttributeAsync(int attributeId)
    {
        return await _context.ProductAttributeMappings
            .Include(pam => pam.Product)
            .Include(pam => pam.ProductAttributeValues)
            .Where(pam => pam.ProductAttributeId == attributeId)
            .ToListAsync();
    }

    public async Task<ProductAttributeMapping> AddAsync(ProductAttributeMapping mapping)
    {
        ArgumentNullException.ThrowIfNull(mapping);
        await _context.ProductAttributeMappings.AddAsync(mapping);
        await _context.SaveChangesAsync();
        return mapping;
    }

    public async Task UpdateAsync(ProductAttributeMapping mapping)
    {
        ArgumentNullException.ThrowIfNull(mapping);
        _context.ProductAttributeMappings.Update(mapping);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int mappingId)
    {
        var mapping = await _context.ProductAttributeMappings.FindAsync(mappingId);
        if (mapping != null)
        {
            _context.ProductAttributeMappings.Remove(mapping);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int mappingId)
    {
        return await _context.ProductAttributeMappings.AnyAsync(pam => pam.Id == mappingId);
    }

    public async Task<bool> ExistsRelationAsync(int productId, int attributeId)
    {
        return await _context.ProductAttributeMappings
            .AnyAsync(pam => pam.ProductId == productId && pam.ProductAttributeId == attributeId);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.ProductAttributeMappings.CountAsync();
    }

    public async Task<int> GetNextDisplayOrderAsync(int productId)
    {
        var maxDisplayOrder = await _context.ProductAttributeMappings
            .Where(pam => pam.ProductId == productId)
            .MaxAsync(pam => (int?)pam.DisplayOrder) ?? 0;
        return maxDisplayOrder + 1;
    }
} 
