using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities;

namespace Toss.Application.Settings.Queries.GetShopSettings;

public record ShopSettingsDto
{
    public int ShopId { get; init; }
    public string ShopName { get; init; } = string.Empty;
    public string Currency { get; init; } = "ZAR";
    public decimal TaxRate { get; init; }
    public string Language { get; init; } = "en";
    public string Timezone { get; init; } = "Africa/Johannesburg";
    public bool EnableGroupBuying { get; init; } = true;
    public bool EnableSharedLogistics { get; init; } = true;
}

public record GetShopSettingsQuery : IRequest<ShopSettingsDto>
{
    public int ShopId { get; init; }
}

public class GetShopSettingsQueryHandler : IRequestHandler<GetShopSettingsQuery, ShopSettingsDto>
{
    private readonly IApplicationDbContext _context;

    public GetShopSettingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ShopSettingsDto> Handle(GetShopSettingsQuery request, CancellationToken cancellationToken)
    {
        var shop = await _context.Shops
            .FindAsync(new object[] { request.ShopId }, cancellationToken);

        if (shop == null)
            throw new NotFoundException(nameof(Shop), request.ShopId.ToString());

        return new ShopSettingsDto
        {
            ShopId = shop.Id,
            ShopName = shop.Name,
            Currency = shop.Currency ?? "ZAR",
            TaxRate = shop.TaxRate,
            Language = shop.Language ?? "en",
            Timezone = shop.Timezone ?? "Africa/Johannesburg",
            EnableGroupBuying = true, // Default enabled
            EnableSharedLogistics = true // Default enabled
        };
    }
}

