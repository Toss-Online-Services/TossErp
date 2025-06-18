using MediatR;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.ValueObjects;
using POS.Domain.SeedWork;

namespace POS.API.Application.Commands;

public record UpdateProductCommand : IRequest<bool>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = "USD";
    public decimal CostPrice { get; init; }
    public string SKU { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public bool IsActive { get; init; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<ProductCategory> _categoryRepository;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IRepository<Product> productRepository,
        IRepository<ProductCategory> categoryRepository,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating product: {ProductId}", request.Id);

        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null)
        {
            _logger.LogWarning("Product not found: {ProductId}", request.Id);
            return false;
        }

        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            throw new InvalidOperationException($"Category with ID {request.CategoryId} not found");
        }

        var money = new Money(request.Price, request.Currency);
        product.UpdateDetails(
            request.Name,
            request.Description,
            money,
            request.CostPrice,
            request.CategoryId);

        if (request.IsActive != product.IsActive)
        {
            if (request.IsActive)
                product.Activate();
            else
                product.Deactivate();
        }

        await _productRepository.UpdateAsync(product);
        await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product updated successfully: {ProductId}", product.Id);

        return true;
    }
} 
