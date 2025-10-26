using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;

namespace Toss.Application.Vendors.Queries.GetVendors;

public record VendorDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Description { get; init; }
    public string? PhoneNumber { get; init; }
    public bool Active { get; init; }
    public int DisplayOrder { get; init; }
}

public record GetVendorsQuery : IRequest<PaginatedList<VendorDto>>
{
    public string? SearchTerm { get; init; }
    public bool? ActiveOnly { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetVendorsQueryHandler : IRequestHandler<GetVendorsQuery, PaginatedList<VendorDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVendorsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<VendorDto>> Handle(GetVendorsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Vendors
            .Where(v => !v.Deleted) // Exclude soft-deleted vendors
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(v => 
                v.Name.ToLower().Contains(searchTerm) ||
                (v.Email != null && v.Email.ToLower().Contains(searchTerm)) ||
                (v.Description != null && v.Description.ToLower().Contains(searchTerm)));
        }

        if (request.ActiveOnly == true)
        {
            query = query.Where(v => v.Active);
        }

        var vendors = await query
            .OrderBy(v => v.DisplayOrder)
            .ThenBy(v => v.Name)
            .Select(v => new VendorDto
            {
                Id = v.Id,
                Name = v.Name,
                Email = v.Email,
                Description = v.Description,
                PhoneNumber = v.PhoneNumber,
                Active = v.Active,
                DisplayOrder = v.DisplayOrder
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return vendors;
    }
}
