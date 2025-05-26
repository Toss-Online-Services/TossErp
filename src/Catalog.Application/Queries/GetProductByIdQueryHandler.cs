using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Queries;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
{
    private readonly IProductService _productService;

    public GetProductByIdQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetProductByIdAsync(request.ProductId);
    }
} 
