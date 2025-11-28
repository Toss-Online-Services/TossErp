using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Orders;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Procurement.Commands.CreatePurchaseRequest;

public record CreatePurchaseRequestLineDto
{
    public int ItemId { get; init; }
    public decimal QuantityRequested { get; init; }
    public string? Remarks { get; init; }
}

public record CreatePurchaseRequestCommand : IRequest<int>
{
    public int ShopId { get; init; }
    public int VendorId { get; init; }
    public DateTime? RequiredByDate { get; init; }
    public string? Notes { get; init; }
    public List<CreatePurchaseRequestLineDto> Items { get; init; } = new();
}

public class CreatePurchaseRequestCommandHandler : IRequestHandler<CreatePurchaseRequestCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly IUser _currentUser;

    public CreatePurchaseRequestCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        IUser currentUser)
    {
        _context = context;
        _businessContext = businessContext;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreatePurchaseRequestCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrEmpty(_currentUser.Id))
        {
            throw new UnauthorizedAccessException("User must be authenticated to create a purchase request.");
        }

        // Validate Shop belongs to current business
        var shop = await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == request.ShopId, cancellationToken);

        if (shop == null)
        {
            throw new NotFoundException(nameof(shop), request.ShopId);
        }

        if (shop.BusinessId != _businessContext.CurrentBusinessId)
        {
            throw new ForbiddenAccessException("Shop does not belong to the current business.");
        }

        // Validate Vendor belongs to current business
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.Id == request.VendorId, cancellationToken);

        if (vendor == null)
        {
            throw new NotFoundException(nameof(vendor), request.VendorId);
        }

        if (vendor.BusinessId != _businessContext.CurrentBusinessId)
        {
            throw new ForbiddenAccessException("Vendor does not belong to the current business.");
        }

        // Validate Items
        if (request.Items == null || !request.Items.Any())
        {
            throw new ValidationException("Purchase request must have at least one item.");
        }

        // Validate all items belong to current business
        var itemIds = request.Items.Select(i => i.ItemId).Distinct().ToList();
        var products = await _context.Products
            .Where(p => itemIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (products.Count != itemIds.Count)
        {
            var missingIds = itemIds.Except(products.Select(p => p.Id)).ToList();
            throw new NotFoundException($"One or more products not found: {string.Join(", ", missingIds)}");
        }

        foreach (var product in products)
        {
            if (product.BusinessId != _businessContext.CurrentBusinessId)
            {
                throw new ForbiddenAccessException($"Product {product.Id} does not belong to the current business.");
            }
        }

        // Validate quantities
        foreach (var item in request.Items)
        {
            if (item.QuantityRequested <= 0)
            {
                throw new ValidationException($"Quantity requested must be greater than zero for item {item.ItemId}.");
            }
        }

        // Generate unique PR number
        var year = DateTimeOffset.UtcNow.Year;
        var existingPRNumbers = await _context.PurchaseRequests
            .Where(pr => pr.ShopId == request.ShopId && pr.PRNumber.StartsWith($"PR-{year}-"))
            .Select(pr => pr.PRNumber)
            .ToListAsync(cancellationToken);

        var prCounter = 1;
        if (existingPRNumbers.Any())
        {
            var maxCounter = existingPRNumbers
                .Select(n => n.Substring($"PR-{year}-".Length))
                .Where(n => int.TryParse(n, out _))
                .Select(n => int.Parse(n))
                .DefaultIfEmpty(0)
                .Max();
            prCounter = maxCounter + 1;
        }

        string prNumber;
        do
        {
            prNumber = $"PR-{year}-{prCounter++:D4}";
        } while (existingPRNumbers.Contains(prNumber));

        // Create PurchaseRequest
        var purchaseRequest = new PurchaseRequest
        {
            PRNumber = prNumber,
            ShopId = request.ShopId,
            VendorId = request.VendorId,
            RequestedByUserId = _currentUser.Id,
            RequiredByDate = request.RequiredByDate,
            Status = PurchaseRequestStatus.Draft,
            Notes = request.Notes
        };

        _context.PurchaseRequests.Add(purchaseRequest);

        // Create PurchaseRequestLines
        foreach (var itemDto in request.Items)
        {
            var product = products.First(p => p.Id == itemDto.ItemId);
            var line = new PurchaseRequestLine
            {
                PurchaseRequestId = purchaseRequest.Id,
                ItemId = itemDto.ItemId,
                QuantityRequested = itemDto.QuantityRequested,
                Remarks = itemDto.Remarks
            };
            _context.PurchaseRequestLines.Add(line);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return purchaseRequest.Id;
    }
}

