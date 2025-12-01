using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Businesses;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Businesses.Commands.UpdateBusinessSettings;

public record UpdateBusinessSettingsCommand : IRequest<bool>
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

public class UpdateBusinessSettingsCommandHandler : IRequestHandler<UpdateBusinessSettingsCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public UpdateBusinessSettingsCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<bool> Handle(UpdateBusinessSettingsCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (request.VatRate < 0 || request.VatRate > 1)
        {
            throw new ValidationException("VAT rate must be between 0 and 1 (0% to 100%).");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        var settings = await _context.BusinessSettings
            .FirstOrDefaultAsync(s => s.BusinessId == businessId, cancellationToken);

        if (settings == null)
        {
            settings = new BusinessSettings
            {
                BusinessId = businessId
            };
            _context.BusinessSettings.Add(settings);
        }

        settings.Currency = request.Currency;
        settings.CurrencySymbol = request.CurrencySymbol;
        settings.VatRate = request.VatRate;
        settings.PricesIncludeVat = request.PricesIncludeVat;
        settings.DateFormat = request.DateFormat;
        settings.TimeFormat = request.TimeFormat;
        settings.Locale = request.Locale;
        settings.Timezone = request.Timezone;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

