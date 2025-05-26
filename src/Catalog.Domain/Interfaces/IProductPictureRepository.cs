using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces;

public interface IProductPictureRepository
{
    Task<ProductPicture?> GetAsync(int id);
    Task<IEnumerable<ProductPicture?>> GetAllAsync();
    Task<IEnumerable<ProductPicture?>> GetByProductAsync(int productId);
    Task<IEnumerable<ProductPicture?>> GetByPictureAsync(int pictureId);
    Task<ProductPicture> AddAsync(ProductPicture picture);
    Task<ProductPicture> UpdateAsync(ProductPicture picture);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsRelationAsync(int productId, int pictureId);
    Task<int> GetTotalCountAsync();
    Task<int> GetNextDisplayOrderAsync(int productId);
} 
