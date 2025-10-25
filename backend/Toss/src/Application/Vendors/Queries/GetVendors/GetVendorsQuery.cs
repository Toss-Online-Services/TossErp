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
        var query = _context.Vendors.AsQueryable();

        if (request.ActiveOnly == true)
        {
            query = query.Where(v => v.Active);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(v => 
                v.Name.Contains(request.SearchTerm) ||
                (v.Email != null && v.Email.Contains(request.SearchTerm)) ||
                (v.Description != null && v.Description.Contains(request.SearchTerm)));
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
                Active = v.Active,
                DisplayOrder = v.DisplayOrder
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return vendors;
    }
}

