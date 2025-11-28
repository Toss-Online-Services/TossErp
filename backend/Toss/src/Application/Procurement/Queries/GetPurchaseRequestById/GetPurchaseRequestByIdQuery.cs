using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Procurement.Queries.GetPurchaseRequestById;

public record PurchaseRequestLineDetailDto
{
    public int Id { get; init; }
    public int ItemId { get; init; }
    public string ItemName { get; init; } = string.Empty;
    public string? ItemSKU { get; init; }
    public decimal QuantityRequested { get; init; }
    public string? Remarks { get; init; }
}

public record PurchaseRequestDetailDto
{
    public int Id { get; init; }
    public string PRNumber { get; init; } = string.Empty;
    public int ShopId { get; init; }
    public string ShopName { get; init; } = string.Empty;
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public string RequestedByUserId { get; init; } = string.Empty;
    public DateTime? RequiredByDate { get; init; }
    public Toss.Domain.Enums.PurchaseRequestStatus Status { get; init; }
    public int? PurchaseOrderId { get; init; }
    public string? PurchaseOrderNumber { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset Created { get; init; }
    public DateTimeOffset? LastModified { get; init; }
    public List<PurchaseRequestLineDetailDto> Items { get; init; } = new();
}

public record GetPurchaseRequestByIdQuery : IRequest<PurchaseRequestDetailDto>
{
    public int Id { get; init; }
}

public class GetPurchaseRequestByIdQueryHandler : IRequestHandler<GetPurchaseRequestByIdQuery, PurchaseRequestDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetPurchaseRequestByIdQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PurchaseRequestDetailDto> Handle(GetPurchaseRequestByIdQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var purchaseRequest = await _context.PurchaseRequests
            .Include(pr => pr.Shop)
            .Include(pr => pr.Vendor)
            .Include(pr => pr.PurchaseOrder)
            .Include(pr => pr.Items)
                .ThenInclude(prl => prl.Item)
            .FirstOrDefaultAsync(pr => pr.Id == request.Id, cancellationToken);

        if (purchaseRequest == null)
        {
            throw new NotFoundException(nameof(PurchaseRequest), request.Id);
        }

        // Verify business context
        if (purchaseRequest.Shop.BusinessId != _businessContext.CurrentBusinessId)
        {
            throw new ForbiddenAccessException("Purchase request does not belong to the current business.");
        }

        return new PurchaseRequestDetailDto
        {
            Id = purchaseRequest.Id,
            PRNumber = purchaseRequest.PRNumber,
            ShopId = purchaseRequest.ShopId,
            ShopName = purchaseRequest.Shop.Name,
            VendorId = purchaseRequest.VendorId,
            VendorName = purchaseRequest.Vendor.Name,
            RequestedByUserId = purchaseRequest.RequestedByUserId,
            RequiredByDate = purchaseRequest.RequiredByDate,
            Status = purchaseRequest.Status,
            PurchaseOrderId = purchaseRequest.PurchaseOrderId,
            PurchaseOrderNumber = purchaseRequest.PurchaseOrder?.PONumber,
            Notes = purchaseRequest.Notes,
            Created = purchaseRequest.Created,
            LastModified = purchaseRequest.LastModified,
            Items = purchaseRequest.Items.Select(prl => new PurchaseRequestLineDetailDto
            {
                Id = prl.Id,
                ItemId = prl.ItemId,
                ItemName = prl.Item.Name,
                ItemSKU = prl.Item.SKU,
                QuantityRequested = prl.QuantityRequested,
                Remarks = prl.Remarks
            }).ToList()
        };
    }
}

