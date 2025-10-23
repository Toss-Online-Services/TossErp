using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.Suppliers;

namespace Toss.Application.Suppliers.Queries.GetSuppliers;

public record GetSuppliersQuery : IRequest<PaginatedList<SupplierDto>>
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, PaginatedList<SupplierDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSuppliersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<SupplierDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Suppliers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(s => 
                s.Name.ToLower().Contains(searchTerm) ||
                (s.Email != null && s.Email.ToLower().Contains(searchTerm)));
        }

        if (request.IsActive.HasValue)
            query = query.Where(s => s.IsActive == request.IsActive.Value);

        return await query
            .OrderBy(s => s.Name)
            .ProjectTo<SupplierDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}

public class SupplierDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public bool IsActive { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Supplier, SupplierDto>()
                .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.ContactPhone != null ? s.ContactPhone.ToString() : null));
        }
    }
}

