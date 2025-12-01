using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Localization;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Businesses;
using System.Globalization;

namespace Toss.Infrastructure.Services.Localization;

public class BusinessLocalizationService : IBusinessLocalizationService
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private BusinessSettings? _cachedSettings;

    public BusinessLocalizationService(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    private async Task<BusinessSettings> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedSettings != null)
        {
            return _cachedSettings;
        }

        if (!_businessContext.HasBusiness)
        {
            // Return default settings if no business context
            return new BusinessSettings
            {
                Currency = "ZAR",
                CurrencySymbol = "R",
                VatRate = 0.15m,
                PricesIncludeVat = true,
                DateFormat = "dd/MM/yyyy",
                TimeFormat = "HH:mm",
                Locale = "en-ZA",
                Timezone = "South Africa Standard Time"
            };
        }

        var settings = await _context.BusinessSettings
            .FirstOrDefaultAsync(s => s.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (settings == null)
        {
            // Create default settings if none exist
            settings = new BusinessSettings
            {
                BusinessId = _businessContext.CurrentBusinessId!.Value,
                Currency = "ZAR",
                CurrencySymbol = "R",
                VatRate = 0.15m,
                PricesIncludeVat = true,
                DateFormat = "dd/MM/yyyy",
                TimeFormat = "HH:mm",
                Locale = "en-ZA",
                Timezone = "South Africa Standard Time"
            };
            _context.BusinessSettings.Add(settings);
            await _context.SaveChangesAsync(cancellationToken);
        }

        _cachedSettings = settings;
        return settings;
    }

    public string GetCurrency()
    {
        var settings = GetSettingsAsync().GetAwaiter().GetResult();
        return settings.Currency;
    }

    public string GetCurrencySymbol()
    {
        var settings = GetSettingsAsync().GetAwaiter().GetResult();
        return settings.CurrencySymbol;
    }

    public decimal GetVatRate()
    {
        var settings = GetSettingsAsync().GetAwaiter().GetResult();
        return settings.VatRate;
    }

    public bool GetPricesIncludeVat()
    {
        var settings = GetSettingsAsync().GetAwaiter().GetResult();
        return settings.PricesIncludeVat;
    }

    public decimal CalculateVat(decimal price, bool? priceIncludesVat = null)
    {
        var settings = GetSettingsAsync().GetAwaiter().GetResult();
        var includesVat = priceIncludesVat ?? settings.PricesIncludeVat;

        if (includesVat)
        {
            // Price includes VAT, so VAT = price * (vatRate / (1 + vatRate))
            return price * (settings.VatRate / (1 + settings.VatRate));
        }
        else
        {
            // Price excludes VAT, so VAT = price * vatRate
            return price * settings.VatRate;
        }
    }

    public decimal CalculatePriceExcludingVat(decimal priceIncludingVat)
    {
        var settings = GetSettingsAsync().GetAwaiter().GetResult();
        // Price excluding VAT = price including VAT / (1 + vatRate)
        return priceIncludingVat / (1 + settings.VatRate);
    }

    public decimal CalculatePriceIncludingVat(decimal priceExcludingVat)
    {
        var settings = GetSettingsAsync().GetAwaiter().GetResult();
        // Price including VAT = price excluding VAT * (1 + vatRate)
        return priceExcludingVat * (1 + settings.VatRate);
    }

    public string FormatCurrency(decimal amount)
    {
        var settings = GetSettingsAsync().GetAwaiter().GetResult();
        
        // Use South African culture for ZAR, otherwise use invariant culture
        var culture = settings.Currency == "ZAR" 
            ? new CultureInfo("en-ZA") 
            : CultureInfo.InvariantCulture;

        return amount.ToString("C", culture);
    }
}

