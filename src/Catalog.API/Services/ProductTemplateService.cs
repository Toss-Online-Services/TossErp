using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Services;

public class ProductTemplateService : IProductTemplateService
{
    private readonly CatalogContext _context;

    public ProductTemplateService(CatalogContext context)
    {
        _context = context;
    }

    public async Task<ProductTemplate?> GetProductTemplateByIdAsync(int productTemplateId)
    {
        return await _context.ProductTemplates
            .FirstOrDefaultAsync(pt => pt.Id == productTemplateId);
    }

    public async Task<IEnumerable<ProductTemplate>> GetAllProductTemplatesAsync()
    {
        return await _context.ProductTemplates.ToListAsync();
    }

    public async Task<IEnumerable<ProductTemplate>> GetProductTemplatesByNameAsync(string name)
    {
        return await _context.ProductTemplates
            .Where(pt => pt.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<ProductTemplate> InsertProductTemplateAsync(ProductTemplate productTemplate)
    {
        await _context.ProductTemplates.AddAsync(productTemplate);
        await _context.SaveChangesAsync();
        return productTemplate;
    }

    public async Task<ProductTemplate> UpdateProductTemplateAsync(ProductTemplate productTemplate)
    {
        _context.ProductTemplates.Update(productTemplate);
        await _context.SaveChangesAsync();
        return productTemplate;
    }

    public async Task DeleteProductTemplateAsync(ProductTemplate productTemplate)
    {
        _context.ProductTemplates.Remove(productTemplate);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductTemplatesAsync(IList<ProductTemplate> productTemplates)
    {
        _context.ProductTemplates.RemoveRange(productTemplates);
        await _context.SaveChangesAsync();
    }

    public async Task<IList<ProductTemplate>> GetAllProductTemplatesAsync(bool showHidden = false)
    {
        var query = _context.ProductTemplates.AsQueryable();

        if (!showHidden)
        {
            query = query.Where(pt => pt.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<ProductTemplate>> GetProductTemplatesByNameAsync(string name, bool showHidden = false)
    {
        var query = _context.ProductTemplates
            .Where(pt => pt.Name.Contains(name));

        if (!showHidden)
        {
            query = query.Where(pt => pt.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<ProductTemplate>> GetAllProductTemplatesAsync(bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.ProductTemplates.AsQueryable();

        if (!showHidden)
        {
            query = query.Where(pt => pt.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<ProductTemplate>> GetProductTemplatesByNameAsync(string name, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.ProductTemplates
            .Where(pt => pt.Name.Contains(name));

        if (!showHidden)
        {
            query = query.Where(pt => pt.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<Product>> GetProductsByTemplateIdAsync(int templateId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.Products
            .Where(p => p.ProductTemplateId == templateId);

        if (!showHidden)
        {
            query = query.Where(p => p.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetProductCountByTemplateIdAsync(int templateId, bool showHidden = false)
    {
        var query = _context.Products
            .Where(p => p.ProductTemplateId == templateId);

        if (!showHidden)
        {
            query = query.Where(p => p.Published);
        }

        return await query.CountAsync();
    }
} 
