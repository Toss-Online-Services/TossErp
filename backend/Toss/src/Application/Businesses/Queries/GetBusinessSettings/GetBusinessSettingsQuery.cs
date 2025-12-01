using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Businesses;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Businesses.Queries.GetBusinessSettings;

public record GetBusinessSettingsQuery : IRequest<BusinessSettingsDto>
{
}

public record BusinessSettingsDto
{
    public string Currency { get; init; } = "ZAR";
    public string CurrencySymbol { get; init; } = "R";
    public decimal VatRate { get; init; } = 0.15m;
    public bool PricesIncludeVat { get; init; } = true;
    public string DateFormat { get; init; } = "dd/MM/yyyy";
    public string TimeFormat { get; init; } = "HH:mm";
    public string Locale { get; init; } = "en-ZA";
    public string Timezone { get; init; } = "South Africa Standard Time";
}

public class GetBusinessSettingsQueryHandler : IRequestHandler<GetBusinessSettingsQuery, BusinessSettingsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetBusinessSettingsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<BusinessSettingsDto> Handle(GetBusinessSettingsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        var settings = await _context.BusinessSettings
            .FirstOrDefaultAsync(s => s.BusinessId == businessId, cancellationToken);

        if (settings == null)
        {
            // Return default settings
            return new BusinessSettingsDto();
        }

        return new BusinessSettingsDto
        {
            Currency = settings.Currency,
            CurrencySymbol = settings.CurrencySymbol,
            VatRate = settings.VatRate,
            PricesIncludeVat = settings.PricesIncludeVat,
            DateFormat = settings.DateFormat,
            TimeFormat = settings.TimeFormat,
            Locale = settings.Locale,
            Timezone = settings.Timezone
        };
    }
}

