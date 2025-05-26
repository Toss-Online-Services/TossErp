using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IProductService _productService;

    public CreateProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(request.Name, request.Sku, request.Price);
        // Set other properties as needed
        var created = await _productService.CreateProductAsync(product);
        return created.Id;
    }
} 
