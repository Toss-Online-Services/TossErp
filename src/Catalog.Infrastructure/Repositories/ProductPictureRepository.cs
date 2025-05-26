using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class ProductPictureRepository : IProductPictureRepository
{
    private readonly CatalogContext _context;

    public ProductPictureRepository(CatalogContext context)
    {
        _context = context;
    }

    public async Task<ProductPicture?> GetAsync(int id)
    {
        return await _context.ProductPictures
            .Include(pp => pp.Product)
            .FirstOrDefaultAsync(pp => pp.Id == id);
    }

    public async Task<IEnumerable<ProductPicture>> GetAllAsync()
    {
        return await _context.ProductPictures
            .Include(pp => pp.Product)
            .OrderBy(pp => pp.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductPicture>> GetByProductAsync(int productId)
    {
        return await _context.ProductPictures
            .Include(pp => pp.Product)
            .Where(pp => pp.ProductId == productId)
            .OrderBy(pp => pp.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductPicture>> GetByPictureAsync(int pictureId)
    {
        return await _context.ProductPictures
            .Include(pp => pp.Product)
            .Where(pp => pp.PictureId == pictureId)
            .OrderBy(pp => pp.DisplayOrder)
            .ToListAsync();
    }

    public async Task<ProductPicture> AddAsync(ProductPicture picture)
    {
        await _context.ProductPictures.AddAsync(picture);
        await _context.SaveChangesAsync();
        return picture;
    }

    public async Task<ProductPicture> UpdateAsync(ProductPicture picture)
    {
        _context.ProductPictures.Update(picture);
        await _context.SaveChangesAsync();
        return picture;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.ProductPictures.AnyAsync(pp => pp.Id == id);
    }

    public async Task<bool> ExistsRelationAsync(int productId, int pictureId)
    {
        return await _context.ProductPictures
            .AnyAsync(pp => pp.ProductId == productId && pp.PictureId == pictureId);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.ProductPictures.CountAsync();
    }

    public async Task<int> GetNextDisplayOrderAsync(int productId)
    {
        var maxDisplayOrder = await _context.ProductPictures
            .Where(pp => pp.ProductId == productId)
            .MaxAsync(pp => (int?)pp.DisplayOrder) ?? 0;
        return maxDisplayOrder + 1;
    }

    public async Task DeleteAsync(int id)
    {
        var picture = await _context.ProductPictures.FindAsync(id);
        if (picture != null)
        {
            _context.ProductPictures.Remove(picture);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteByProductIdAsync(int productId)
    {
        var productPictures = await _context.ProductPictures
            .Where(pp => pp.ProductId == productId)
            .ToListAsync();

        _context.ProductPictures.RemoveRange(productPictures);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByProductIdAsync(int productId)
    {
        return await _context.ProductPictures.AnyAsync(pp => pp.ProductId == productId);
    }

    public async Task<int> CountByProductIdAsync(int productId)
    {
        return await _context.ProductPictures.CountAsync(pp => pp.ProductId == productId);
    }

    public async Task UpdateDisplayOrderAsync(int id, int displayOrder)
    {
        var productPicture = await _context.ProductPictures.FindAsync(id);
        if (productPicture != null)
        {
            productPicture.DisplayOrder = displayOrder;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ProductPicture>> GetByProductIdsAsync(IEnumerable<int> productIds)
    {
        return await _context.ProductPictures
            .Include(pp => pp.Product)
            .Where(pp => productIds.Contains(pp.ProductId))
            .OrderBy(pp => pp.DisplayOrder)
            .ToListAsync();
    }
} 
