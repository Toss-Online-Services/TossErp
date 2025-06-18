using MediatR;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.ValueObjects;
using POS.Domain.SeedWork;

namespace POS.API.Application.Commands;

public record CreateProductCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = "USD";
    public decimal CostPrice { get; init; }
    public string SKU { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public Guid StoreId { get; init; }
    public int StockQuantity { get; init; }
    public int LowStockThreshold { get; init; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<ProductCategory> _categoryRepository;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IRepository<Product> productRepository,
        IRepository<ProductCategory> categoryRepository,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating product: {ProductName}", request.Name);

        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            throw new InvalidOperationException($"Category with ID {request.CategoryId} not found");
        }

        var money = new Money(request.Price, request.Currency);
        var product = new Product(
            request.Name,
            request.Description,
            request.SKU,
            request.Barcode,
            money,
            request.CostPrice,
            request.CategoryId,
            request.StoreId,
            request.StockQuantity,
            request.LowStockThreshold);

        await _productRepository.AddAsync(product);
        await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);

        return product.Id;
    }
} 
