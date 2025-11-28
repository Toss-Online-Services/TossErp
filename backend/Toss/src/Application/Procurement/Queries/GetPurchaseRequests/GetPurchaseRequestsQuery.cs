using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Models;
using Toss.Domain.Entities.Orders;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Procurement.Queries.GetPurchaseRequests;

public record PurchaseRequestLineDto
{
    public int Id { get; init; }
    public int ItemId { get; init; }
    public string ItemName { get; init; } = string.Empty;
    public string? ItemSKU { get; init; }
    public decimal QuantityRequested { get; init; }
    public string? Remarks { get; init; }
}

public record PurchaseRequestDto
{
    public int Id { get; init; }
    public string PRNumber { get; init; } = string.Empty;
    public int ShopId { get; init; }
    public string ShopName { get; init; } = string.Empty;
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public string RequestedByUserId { get; init; } = string.Empty;
    public DateTime? RequiredByDate { get; init; }
    public PurchaseRequestStatus Status { get; init; }
    public int? PurchaseOrderId { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset Created { get; init; }
    public List<PurchaseRequestLineDto> Items { get; init; } = new();
}

public record GetPurchaseRequestsQuery : IRequest<PaginatedList<PurchaseRequestDto>>
{
    public int? ShopId { get; init; }
    public int? VendorId { get; init; }
    public PurchaseRequestStatus? Status { get; init; }
    public DateTime? RequiredByDateFrom { get; init; }
    public DateTime? RequiredByDateTo { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetPurchaseRequestsQueryHandler : IRequestHandler<GetPurchaseRequestsQuery, PaginatedList<PurchaseRequestDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetPurchaseRequestsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<PurchaseRequestDto>> Handle(GetPurchaseRequestsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            return new PaginatedList<PurchaseRequestDto>(new List<PurchaseRequestDto>(), 0, request.PageNumber, request.PageSize);
        }

        var query = _context.PurchaseRequests
            .Include(pr => pr.Shop)
            .Include(pr => pr.Vendor)
            .Include(pr => pr.Items)
                .ThenInclude(prl => prl.Item)
            .AsQueryable();

        // Apply filters
        if (request.ShopId.HasValue)
        {
            query = query.Where(pr => pr.ShopId == request.ShopId.Value);
        }

        if (request.VendorId.HasValue)
        {
            query = query.Where(pr => pr.VendorId == request.VendorId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(pr => pr.Status == request.Status.Value);
        }

        if (request.RequiredByDateFrom.HasValue)
        {
            query = query.Where(pr => pr.RequiredByDate >= request.RequiredByDateFrom.Value);
        }

        if (request.RequiredByDateTo.HasValue)
        {
            query = query.Where(pr => pr.RequiredByDate <= request.RequiredByDateTo.Value);
        }

        // Order by created date descending
        query = query.OrderByDescending(pr => pr.Created);

        return await query
            .Select(pr => new PurchaseRequestDto
            {
                Id = pr.Id,
                PRNumber = pr.PRNumber,
                ShopId = pr.ShopId,
                ShopName = pr.Shop.Name,
                VendorId = pr.VendorId,
                VendorName = pr.Vendor.Name,
                RequestedByUserId = pr.RequestedByUserId,
                RequiredByDate = pr.RequiredByDate,
                Status = pr.Status,
                PurchaseOrderId = pr.PurchaseOrderId,
                Notes = pr.Notes,
                Created = pr.Created,
                Items = pr.Items.Select(prl => new PurchaseRequestLineDto
                {
                    Id = prl.Id,
                    ItemId = prl.ItemId,
                    ItemName = prl.Item.Name,
                    ItemSKU = prl.Item.SKU,
                    QuantityRequested = prl.QuantityRequested,
                    Remarks = prl.Remarks
                }).ToList()
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}

