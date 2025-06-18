using MediatR;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.SeedWork;

namespace POS.API.Application.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<bool>;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IRepository<Product> _productRepository;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(
        IRepository<Product> productRepository,
        ILogger<DeleteProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting product: {ProductId}", request.Id);

        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null)
        {
            _logger.LogWarning("Product not found: {ProductId}", request.Id);
            return false;
        }

        await _productRepository.DeleteAsync(product);
        await _productRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product deleted successfully: {ProductId}", request.Id);

        return true;
    }
} 
