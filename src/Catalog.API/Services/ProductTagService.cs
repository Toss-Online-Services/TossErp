using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Services;

public class ProductTagService : IProductTagService
{
    private readonly CatalogContext _context;

    public ProductTagService(CatalogContext context)
    {
        _context = context;
    }

    public async Task<ProductTag?> GetProductTagByIdAsync(int productTagId)
    {
        return await _context.ProductTags
            .FirstOrDefaultAsync(pt => pt.Id == productTagId);
    }

    public async Task<IEnumerable<ProductTag>> GetAllProductTagsAsync()
    {
        return await _context.ProductTags.ToListAsync();
    }

    public async Task<IEnumerable<ProductTag>> GetProductTagsByProductIdAsync(int productId)
    {
        return await _context.ProductTags
            .Where(pt => pt.Products.Any(p => p.Id == productId))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductTag>> GetProductTagsByNameAsync(string name)
    {
        return await _context.ProductTags
            .Where(pt => pt.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<ProductTag> InsertProductTagAsync(ProductTag productTag)
    {
        await _context.ProductTags.AddAsync(productTag);
        await _context.SaveChangesAsync();
        return productTag;
    }

    public async Task<ProductTag> UpdateProductTagAsync(ProductTag productTag)
    {
        _context.ProductTags.Update(productTag);
        await _context.SaveChangesAsync();
        return productTag;
    }

    public async Task DeleteProductTagAsync(ProductTag productTag)
    {
        _context.ProductTags.Remove(productTag);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductTagsAsync(IList<ProductTag> productTags)
    {
        _context.ProductTags.RemoveRange(productTags);
        await _context.SaveChangesAsync();
    }

    public async Task<IList<ProductTag>> GetAllProductTagsAsync(bool showHidden = false)
    {
        var query = _context.ProductTags.AsQueryable();

        if (!showHidden)
        {
            query = query.Where(pt => pt.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<ProductTag>> GetProductTagsByProductIdAsync(int productId, bool showHidden = false)
    {
        var query = _context.ProductTags
            .Where(pt => pt.Products.Any(p => p.Id == productId));

        if (!showHidden)
        {
            query = query.Where(pt => pt.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<ProductTag>> GetProductTagsByNameAsync(string name, bool showHidden = false)
    {
        var query = _context.ProductTags
            .Where(pt => pt.Name.Contains(name));

        if (!showHidden)
        {
            query = query.Where(pt => pt.Published);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<ProductTag>> GetAllProductTagsAsync(bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.ProductTags.AsQueryable();

        if (!showHidden)
        {
            query = query.Where(pt => pt.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<ProductTag>> GetProductTagsByProductIdAsync(int productId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.ProductTags
            .Where(pt => pt.Products.Any(p => p.Id == productId));

        if (!showHidden)
        {
            query = query.Where(pt => pt.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<ProductTag>> GetProductTagsByNameAsync(string name, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.ProductTags
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

    public async Task<int> GetProductCountByProductTagIdAsync(int productTagId, bool showHidden = false)
    {
        var query = _context.Products
            .Where(p => p.ProductTags.Any(pt => pt.Id == productTagId));

        if (!showHidden)
        {
            query = query.Where(p => p.Published);
        }

        return await query.CountAsync();
    }

    public async Task<IList<Product>> GetProductsByProductTagIdAsync(int productTagId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _context.Products
            .Where(p => p.ProductTags.Any(pt => pt.Id == productTagId));

        if (!showHidden)
        {
            query = query.Where(p => p.Published);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<ProductTag>> GetProductTagsByProductIdAsync(int productId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue, bool includeDeleted = false)
    {
        var query = _context.ProductTags
            .Where(pt => pt.Products.Any(p => p.Id == productId));

        if (!showHidden)
        {
            query = query.Where(pt => pt.Published);
        }

        if (!includeDeleted)
        {
            query = query.Where(pt => !pt.Deleted);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<ProductTag>> GetProductTagsByProductIdAsync(int productId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue, bool includeDeleted = false, int? storeId = null)
    {
        var query = _context.ProductTags
            .Where(pt => pt.Products.Any(p => p.Id == productId));

        if (!showHidden)
        {
            query = query.Where(pt => pt.Published);
        }

        if (!includeDeleted)
        {
            query = query.Where(pt => !pt.Deleted);
        }

        if (storeId.HasValue)
        {
            query = query.Where(pt => pt.StoreId == storeId.Value);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<ProductTag>> GetProductTagsByProductIdAsync(int productId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue, bool includeDeleted = false, int? storeId = null, int? languageId = null)
    {
        var query = _context.ProductTags
            .Where(pt => pt.Products.Any(p => p.Id == productId));

        if (!showHidden)
        {
            query = query.Where(pt => pt.Published);
        }

        if (!includeDeleted)
        {
            query = query.Where(pt => !pt.Deleted);
        }

        if (storeId.HasValue)
        {
            query = query.Where(pt => pt.StoreId == storeId.Value);
        }

        if (languageId.HasValue)
        {
            query = query.Where(pt => pt.LanguageId == languageId.Value);
        }

        return await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
} 
