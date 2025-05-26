using MediatR;

namespace Catalog.Application.Commands;

public class CreateProductCommand : IRequest<int>
{
    public string Name { get; set; }
    public string Sku { get; set; }
    public decimal Price { get; set; }
    // Add other product properties as needed
} 
