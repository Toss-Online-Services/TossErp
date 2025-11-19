using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Toss.Application.Sales.Queries.GetHeldSales;

public record GetHeldSalesQuery : IRequest<List<HeldSaleDto>>
{
    public int ShopId { get; init; }
}

public class GetHeldSalesQueryHandler : IRequestHandler<GetHeldSalesQuery, List<HeldSaleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetHeldSalesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<HeldSaleDto>> Handle(GetHeldSalesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Sales
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .Include(s => s.Customer)
            .Where(s => s.ShopId == request.ShopId && s.Status == SaleStatus.OnHold)
            .OrderByDescending(s => s.Created)
            .ProjectTo<HeldSaleDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}

public class HeldSaleDto
{
    public int Id { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public int? CustomerId { get; init; }
    public string? CustomerName { get; init; }
    public DateTimeOffset HeldAt { get; init; }
    public decimal Total { get; init; }
    public PaymentType PaymentMethod { get; init; }
    public string? Notes { get; init; }
    public List<HeldSaleItemDto> Items { get; init; } = new();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Sales.Sale, HeldSaleDto>()
                .ForMember(d => d.CustomerName, opt => opt.MapFrom(s => s.Customer != null ? s.Customer.FullName : "Walk-in Customer"))
                .ForMember(d => d.HeldAt, opt => opt.MapFrom(s => s.Created));

            CreateMap<Domain.Entities.Sales.SaleItem, HeldSaleItemDto>()
                .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product != null ? s.Product.Name : "Unknown"));
        }
    }
}

public class HeldSaleItemDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Total { get; init; }
}

