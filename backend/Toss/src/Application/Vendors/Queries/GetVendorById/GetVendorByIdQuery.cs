using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Vendors;

namespace Toss.Application.Vendors.Queries.GetVendorById;

public record VendorDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Description { get; init; }
    public string? CompanyRegNumber { get; init; }
    public string? VATNumber { get; init; }
    public string? ContactPerson { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Website { get; init; }
    public string? Address { get; init; }
    public bool Active { get; init; }
    public decimal? CreditLimit { get; init; }
    public int PaymentTermsDays { get; init; }
    public int ProductCount { get; init; }
    public List<VendorProductDto> Products { get; init; } = new();
}

public record VendorProductDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? VendorSKU { get; init; }
    public decimal BasePrice { get; init; }
}

public record GetVendorByIdQuery : IRequest<VendorDetailDto>
{
    public int Id { get; init; }
}

public class GetVendorByIdQueryHandler : IRequestHandler<GetVendorByIdQuery, VendorDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetVendorByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VendorDetailDto> Handle(GetVendorByIdQuery request, CancellationToken cancellationToken)
    {
        var vendor = await _context.Vendors
            .Include(v => v.VendorProducts)
                .ThenInclude(vp => vp.Product)
            .Include(v => v.Address)
            .FirstOrDefaultAsync(v => v.Id == request.Id && !v.Deleted, cancellationToken);

        if (vendor == null)
            throw new NotFoundException(nameof(Vendor), request.Id.ToString());

        return new VendorDetailDto
        {
            Id = vendor.Id,
            Name = vendor.Name,
            Email = vendor.Email,
            Description = vendor.Description,
            CompanyRegNumber = vendor.CompanyRegNumber,
            VATNumber = vendor.VATNumber,
            ContactPerson = vendor.ContactPerson,
            PhoneNumber = vendor.PhoneNumber,
            Website = vendor.Website,
            Address = vendor.Address != null ? $"{vendor.Address.Street}, {vendor.Address.City}" : null,
            Active = vendor.Active,
            CreditLimit = vendor.CreditLimit,
            PaymentTermsDays = vendor.PaymentTermsDays,
            ProductCount = vendor.VendorProducts.Count,
            Products = vendor.VendorProducts.Select(vp => new VendorProductDto
            {
                ProductId = vp.ProductId,
                ProductName = vp.Product.Name,
                VendorSKU = vp.VendorSKU,
                BasePrice = vp.BasePrice
            }).ToList()
        };
    }
}

