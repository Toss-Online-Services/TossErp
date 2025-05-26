using Catalog.Domain.Entities;

namespace Catalog.Application.Abstractions;

public interface IProductService
{
    Task<Product> GetProductByIdAsync(int productId);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product> CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int productId);
    // Add more methods as needed, e.g., price calculation, stock check, etc.
} 
