using TossErp.Application.DTOs;
using TossErp.Domain.AggregatesModel.ProductAggregate;

namespace TossErp.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found");
        return MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category)
    {
        // This method needs a businessId parameter, but we'll return empty for now
        // TODO: Update the interface to include businessId parameter
        return Task.FromResult<IEnumerable<ProductDto>>(new List<ProductDto>());
    }

    public async Task<IEnumerable<ProductDto>> GetByBusinessAsync(Guid businessId)
    {
        var products = await _productRepository.GetByBusinessIdAsync(businessId);
        return products.Select(MapToDto);
    }

    public Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm)
    {
        // This method needs a businessId parameter, but we'll return empty for now
        // TODO: Update the interface to include businessId parameter
        return Task.FromResult<IEnumerable<ProductDto>>(new List<ProductDto>());
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto createDto)
    {
        var product = new Product(
            createDto.Name,
            createDto.Description,
            createDto.Barcode,
            createDto.SKU,
            createDto.Price,
            createDto.CostPrice,
            createDto.StockQuantity,
            createDto.MinimumStockLevel,
            createDto.Category,
            createDto.Unit,
            createDto.BusinessId,
            createDto.VendorId);

        await _productRepository.AddAsync(product);
        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, CreateProductDto updateDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found");

        product.UpdateDetails(
            updateDto.Name,
            updateDto.Description,
            updateDto.Price,
            updateDto.CostPrice,
            updateDto.Category,
            updateDto.Unit);

        await _productRepository.UpdateAsync(product);
        return MapToDto(product);
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found");

        product.Deactivate();
        await _productRepository.UpdateAsync(product);
    }

    public async Task<int> GetLowStockItemsCountAsync(Guid businessId)
    {
        var lowStockProducts = await _productRepository.GetLowStockProductsAsync(businessId);
        return lowStockProducts.Count();
    }

    public Task<IEnumerable<TopProductDto>> GetTopSellingProductsAsync(int count)
    {
        // TODO: Implement actual top selling products logic
        // For now, return empty list
        return Task.FromResult<IEnumerable<TopProductDto>>(new List<TopProductDto>());
    }

    public async Task UpdateStockAsync(Guid id, int quantity, string movementType, string reason = "")
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found");

        product.UpdateStock(quantity, movementType, reason);
        await _productRepository.UpdateAsync(product);
    }

    // Additional methods for backward compatibility
    public async Task<ProductDto?> GetByBarcodeAsync(string barcode, Guid businessId)
    {
        var product = await _productRepository.GetByBarcodeAsync(barcode, businessId);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(Guid businessId)
    {
        var products = await _productRepository.GetLowStockProductsAsync(businessId);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category, Guid businessId)
    {
        var products = await _productRepository.GetByCategoryAsync(category, businessId);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm, Guid businessId)
    {
        var products = await _productRepository.SearchAsync(searchTerm, businessId);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto updateDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found");

        product.UpdateDetails(
            updateDto.Name,
            updateDto.Description,
            updateDto.Price,
            updateDto.CostPrice,
            updateDto.Category,
            updateDto.Unit);

        await _productRepository.UpdateAsync(product);
        return MapToDto(product);
    }

    public async Task UpdateStockAsync(Guid id, UpdateStockDto updateStockDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found");

        product.UpdateStock(
            updateStockDto.Quantity,
            updateStockDto.MovementType,
            updateStockDto.Reason);

        await _productRepository.UpdateAsync(product);
    }

    public async Task SetMinimumStockLevelAsync(Guid id, int minimumLevel)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found");

        product.SetMinimumStockLevel(minimumLevel);
        await _productRepository.UpdateAsync(product);
    }

    public async Task DeactivateAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found");

        product.Deactivate();
        await _productRepository.UpdateAsync(product);
    }

    public async Task ActivateAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found");

        product.Activate();
        await _productRepository.UpdateAsync(product);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _productRepository.ExistsAsync(id);
    }

    public async Task<bool> BarcodeExistsAsync(string barcode, Guid businessId)
    {
        return await _productRepository.BarcodeExistsAsync(barcode, businessId);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Barcode = product.Barcode,
            SKU = product.SKU,
            Price = product.Price,
            CostPrice = product.CostPrice,
            StockQuantity = product.StockQuantity,
            MinimumStockLevel = product.MinimumStockLevel,
            Category = product.Category,
            Unit = product.Unit,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            BusinessId = product.BusinessId,
            VendorId = product.VendorId
        };
    }
} 
