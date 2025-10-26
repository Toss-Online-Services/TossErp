using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.GroupBuying;
using Toss.Domain.Enums;

namespace Toss.Application.GroupBuying.Queries.GetPoolById;

public record GetPoolByIdQuery : IRequest<PoolDetailDto>
{
    public int Id { get; init; }
}

public class GetPoolByIdQueryHandler : IRequestHandler<GetPoolByIdQuery, PoolDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetPoolByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PoolDetailDto> Handle(GetPoolByIdQuery request, CancellationToken cancellationToken)
    {
        var pool = await _context.GroupBuyPools
            .Include(p => p.Product)
            .Include(p => p.Vendor)
            .Include(p => p.InitiatorShop)
            .Include(p => p.Participations)
                .ThenInclude(pp => pp.Shop)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (pool == null)
            throw new NotFoundException(nameof(GroupBuyPool), request.Id.ToString());

        return new PoolDetailDto
        {
            Id = pool.Id,
            PoolNumber = pool.PoolNumber,
            Title = pool.Title,
            Description = pool.Description,
            Status = pool.Status,
            ProductId = pool.ProductId,
            ProductName = pool.Product.Name,
            ProductSKU = pool.Product.SKU,
            VendorId = pool.VendorId,
            VendorName = pool.Vendor.Name,
            InitiatorShopId = pool.InitiatorShopId,
            InitiatorShopName = pool.InitiatorShop.Name,
            MinimumQuantity = pool.MinimumQuantity,
            MaximumQuantity = pool.MaximumQuantity,
            CurrentQuantity = pool.CurrentQuantity,
            UnitPrice = pool.UnitPrice,
            BulkDiscountPercentage = pool.BulkDiscountPercentage,
            FinalUnitPrice = pool.FinalUnitPrice,
            OpenDate = pool.OpenDate,
            CloseDate = pool.CloseDate,
            ConfirmedDate = pool.ConfirmedDate,
            EstimatedShippingCost = pool.EstimatedShippingCost,
            ActualShippingCost = pool.ActualShippingCost,
            AreaGroup = pool.AreaGroup,
            MaxDistanceKm = pool.MaxDistanceKm,
            Participations = pool.Participations.Select(pp => new ParticipationDto
            {
                Id = pp.Id,
                ShopId = pp.ShopId,
                ShopName = pp.Shop.Name,
                QuantityCommitted = pp.QuantityCommitted,
                Total = pp.Total,
                JoinedDate = pp.JoinedDate,
                IsConfirmed = pp.IsConfirmed
            }).ToList()
        };
    }
}

public class PoolDetailDto
{
    public int Id { get; init; }
    public string PoolNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public PoolStatus Status { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSKU { get; init; } = string.Empty;
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public int InitiatorShopId { get; init; }
    public string InitiatorShopName { get; init; } = string.Empty;
    public int MinimumQuantity { get; init; }
    public int? MaximumQuantity { get; init; }
    public int CurrentQuantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal BulkDiscountPercentage { get; init; }
    public decimal FinalUnitPrice { get; init; }
    public DateTimeOffset OpenDate { get; init; }
    public DateTimeOffset CloseDate { get; init; }
    public DateTimeOffset? ConfirmedDate { get; init; }
    public decimal EstimatedShippingCost { get; init; }
    public decimal? ActualShippingCost { get; init; }
    public string? AreaGroup { get; init; }
    public double? MaxDistanceKm { get; init; }
    public List<ParticipationDto> Participations { get; init; } = new();
}

public class ParticipationDto
{
    public int Id { get; init; }
    public int ShopId { get; init; }
    public string ShopName { get; init; } = string.Empty;
    public int QuantityCommitted { get; init; }
    public decimal Total { get; init; }
    public DateTimeOffset JoinedDate { get; init; }
    public bool IsConfirmed { get; init; }
}

