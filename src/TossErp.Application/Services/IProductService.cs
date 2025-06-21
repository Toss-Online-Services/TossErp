using TossErp.Application.DTOs;

namespace TossErp.Application.Services;

public interface IProductService
{
    Task<ProductDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category);
    Task<IEnumerable<ProductDto>> GetByBusinessAsync(Guid businessId);
    Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm);
    Task<ProductDto> CreateAsync(CreateProductDto createProductDto);
    Task<ProductDto> UpdateAsync(Guid id, CreateProductDto updateProductDto);
    Task DeleteAsync(Guid id);
    Task<int> GetLowStockItemsCountAsync(Guid businessId);
    Task<IEnumerable<TopProductDto>> GetTopSellingProductsAsync(int count);
    Task UpdateStockAsync(Guid id, int quantity, string movementType, string reason = "");
} 
