using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Assets.Commands.UpdateAsset;

public record UpdateAssetCommand : IRequest
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? Code { get; init; }
    public decimal? Value { get; init; }
    public decimal? PurchaseCost { get; init; }
    public DateTimeOffset? PurchaseDate { get; init; }
    public string? Location { get; init; }
    public int? ShopId { get; init; }
    public AssetCondition? Condition { get; init; }
    public string? Category { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? SerialNumber { get; init; }
    public string? Notes { get; init; }
    public bool? IsActive { get; init; }
}

public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public UpdateAssetCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var asset = await _context.Assets
            .FirstOrDefaultAsync(a => a.Id == request.Id
                && a.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (asset == null)
        {
            throw new NotFoundException("Asset", request.Id.ToString());
        }

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ValidationException("Asset name cannot be empty.");
            }
            asset.Name = request.Name;
        }

        if (request.Code != null)
        {
            asset.Code = request.Code;
        }

        if (request.Value.HasValue)
        {
            if (request.Value.Value < 0)
            {
                throw new ValidationException("Asset value cannot be negative.");
            }
            asset.Value = request.Value.Value;
        }

        if (request.PurchaseCost.HasValue)
        {
            asset.PurchaseCost = request.PurchaseCost.Value;
        }

        if (request.PurchaseDate.HasValue)
        {
            asset.PurchaseDate = request.PurchaseDate.Value;
        }

        if (request.Location != null)
        {
            if (string.IsNullOrWhiteSpace(request.Location))
            {
                throw new ValidationException("Asset location cannot be empty.");
            }
            asset.Location = request.Location;
        }

        if (request.ShopId.HasValue)
        {
            var shopExists = await _context.Stores
                .AnyAsync(s => s.Id == request.ShopId.Value
                    && s.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

            if (!shopExists)
            {
                throw new NotFoundException(nameof(Store), request.ShopId.Value.ToString());
            }
            asset.ShopId = request.ShopId;
        }

        if (request.Condition.HasValue)
        {
            asset.Condition = request.Condition.Value;
        }

        if (request.Category != null)
        {
            asset.Category = request.Category;
        }

        if (request.Brand != null)
        {
            asset.Brand = request.Brand;
        }

        if (request.Model != null)
        {
            asset.Model = request.Model;
        }

        if (request.SerialNumber != null)
        {
            asset.SerialNumber = request.SerialNumber;
        }

        if (request.Notes != null)
        {
            asset.Notes = request.Notes;
        }

        if (request.IsActive.HasValue)
        {
            asset.IsActive = request.IsActive.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

