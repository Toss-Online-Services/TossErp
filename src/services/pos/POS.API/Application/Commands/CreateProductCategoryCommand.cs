using MediatR;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.SeedWork;

namespace POS.API.Application.Commands;

public record CreateProductCategoryCommand(string Name, string Description, Guid? ParentCategoryId = null) : IRequest<Guid>;

public class CreateProductCategoryCommandHandler : IRequestHandler<CreateProductCategoryCommand, Guid>
{
    private readonly IRepository<ProductCategory> _categoryRepository;
    private readonly ILogger<CreateProductCategoryCommandHandler> _logger;

    public CreateProductCategoryCommandHandler(
        IRepository<ProductCategory> categoryRepository,
        ILogger<CreateProductCategoryCommandHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating product category: {CategoryName}", request.Name);
        
        ProductCategory? parentCategory = null;
        if (request.ParentCategoryId.HasValue)
        {
            parentCategory = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value);
            if (parentCategory == null)
            {
                throw new InvalidOperationException($"Parent category with ID {request.ParentCategoryId.Value} not found");
            }
        }
        
        var category = new ProductCategory(request.Name, request.Description, parentCategory);
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Product category created successfully with ID: {CategoryId}", category.Id);
        return category.Id;
    }
} 
