using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities;

namespace Toss.Application.Settings.Commands.UpdateShopSettings;

public record UpdateShopSettingsCommand : IRequest<bool>
{
    public int ShopId { get; init; }
    public string? Currency { get; init; }
    public decimal? TaxRate { get; init; }
    public string? Language { get; init; }
    public string? Timezone { get; init; }
    public bool? EnableGroupBuying { get; init; }
    public bool? EnableSharedLogistics { get; init; }
}

public class UpdateShopSettingsCommandHandler : IRequestHandler<UpdateShopSettingsCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateShopSettingsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateShopSettingsCommand request, CancellationToken cancellationToken)
    {
        var shop = await _context.Shops
            .FindAsync(new object[] { request.ShopId }, cancellationToken);

        if (shop == null)
            throw new NotFoundException(nameof(Store), request.ShopId.ToString());

        // Update settings (simplified - in production, you'd have a Settings entity)
        if (!string.IsNullOrWhiteSpace(request.Currency))
            shop.Currency = request.Currency;

        if (request.TaxRate.HasValue)
            shop.TaxRate = request.TaxRate.Value;

        if (!string.IsNullOrWhiteSpace(request.Language))
            shop.Language = request.Language;

        if (!string.IsNullOrWhiteSpace(request.Timezone))
            shop.Timezone = request.Timezone;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

