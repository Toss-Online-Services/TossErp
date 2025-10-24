using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Suppliers;

namespace Toss.Application.Suppliers.Queries.GetSupplierById;

public record SupplierDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? CompanyRegNumber { get; init; }
    public string? ContactPerson { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public bool IsActive { get; init; }
    public int ProductCount { get; init; }
    public List<SupplierProductDto> Products { get; init; } = new();
}

public record SupplierProductDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? SupplierSKU { get; init; }
    public decimal BasePrice { get; init; }
}

public record GetSupplierByIdQuery : IRequest<SupplierDetailDto>
{
    public int Id { get; init; }
}

public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetSupplierByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SupplierDetailDto> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.SupplierProducts)
                .ThenInclude(sp => sp.Product)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (supplier == null)
            throw new NotFoundException(nameof(Supplier), request.Id.ToString());

        return new SupplierDetailDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            CompanyRegNumber = supplier.CompanyRegNumber,
            ContactPerson = supplier.ContactPerson,
            PhoneNumber = supplier.PhoneNumber,
            Email = supplier.Email,
            Address = supplier.Address != null ? $"{supplier.Address.Street}, {supplier.Address.City}" : null,
            IsActive = supplier.IsActive,
            ProductCount = supplier.SupplierProducts.Count,
            Products = supplier.SupplierProducts.Select(sp => new SupplierProductDto
            {
                ProductId = sp.ProductId,
                ProductName = sp.Product.Name,
                SupplierSKU = sp.SupplierSKU,
                BasePrice = sp.BasePrice
            }).ToList()
        };
    }
}

