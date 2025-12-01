using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Assets;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Assets.Commands.CreateAsset;

public record CreateAssetCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string? Code { get; init; }
    public decimal Value { get; init; }
    public decimal? PurchaseCost { get; init; }
    public DateTimeOffset? PurchaseDate { get; init; }
    public string Location { get; init; } = string.Empty;
    public int? ShopId { get; init; }
    public AssetCondition Condition { get; init; } = AssetCondition.Good;
    public string? Category { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? SerialNumber { get; init; }
    public string? Notes { get; init; }
}

public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public CreateAssetCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException("Asset name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Location))
        {
            throw new ValidationException("Asset location is required.");
        }

        if (request.Value < 0)
        {
            throw new ValidationException("Asset value cannot be negative.");
        }

        // Validate shop exists if provided
        if (request.ShopId.HasValue)
        {
            var shopExists = await _context.Stores
                .AnyAsync(s => s.Id == request.ShopId.Value
                    && s.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

            if (!shopExists)
            {
                throw new NotFoundException(nameof(Store), request.ShopId.Value.ToString());
            }
        }

        var asset = new Asset
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            Name = request.Name,
            Code = request.Code,
            Value = request.Value,
            PurchaseCost = request.PurchaseCost,
            PurchaseDate = request.PurchaseDate,
            Location = request.Location,
            ShopId = request.ShopId,
            Condition = request.Condition,
            Category = request.Category,
            Brand = request.Brand,
            Model = request.Model,
            SerialNumber = request.SerialNumber,
            Notes = request.Notes,
            IsActive = true
        };

        _context.Assets.Add(asset);
        await _context.SaveChangesAsync(cancellationToken);

        return asset.Id;
    }
}

