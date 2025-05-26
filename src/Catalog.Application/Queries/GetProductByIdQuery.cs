using Catalog.Domain.Entities;
using MediatR;

namespace Catalog.Application.Queries;

public class GetProductByIdQuery : IRequest<Product>
{
    public int ProductId { get; set; }
    public GetProductByIdQuery(int productId) => ProductId = productId;
} 
