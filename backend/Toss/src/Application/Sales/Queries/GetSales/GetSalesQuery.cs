using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;

namespace Toss.Application.Sales.Queries.GetSales;

public record GetSalesQuery : IRequest<PaginatedList<SaleDto>>
{
    public int ShopId { get; init; }
    public DateTimeOffset? FromDate { get; init; }
    public DateTimeOffset? ToDate { get; init; }
    public SaleStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, PaginatedList<SaleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSalesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<SaleDto>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Sales
            .Where(s => s.ShopId == request.ShopId);

        if (request.FromDate.HasValue)
            query = query.Where(s => s.SaleDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(s => s.SaleDate <= request.ToDate.Value);

        if (request.Status.HasValue)
            query = query.Where(s => s.Status == request.Status.Value);

        return await query
            .OrderByDescending(s => s.SaleDate)
            .ProjectTo<SaleDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}

public class SaleDto
{
    public int Id { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public DateTimeOffset SaleDate { get; init; }
    public SaleStatus Status { get; init; }
    public decimal Total { get; init; }
    public PaymentType PaymentMethod { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Sale, SaleDto>();
        }
    }
}

