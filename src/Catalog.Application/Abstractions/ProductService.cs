using Catalog.Domain.AggregatesModel.CatalogAggregate;
using Catalog.Domain.Repositories;

namespace Catalog.Application.Abstractions;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product> GetProductByIdAsync(int productId)
    {
        return await _productRepository.GetAsync(productId);
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        return await _productRepository.AddAsync(product);
    }

    public async Task UpdateProductAsync(Product product)
    {
        await _productRepository.UpdateAsync(product);
    }

    public async Task DeleteProductAsync(int productId)
    {
        await _productRepository.DeleteAsync(productId);
    }
} 
