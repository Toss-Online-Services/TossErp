namespace Toss.Application.Common.Interfaces.Localization;

/// <summary>
/// Service for accessing business-specific localization settings (currency, VAT, date formats)
/// </summary>
public interface IBusinessLocalizationService
{
    /// <summary>
    /// Gets the currency code for the current business (default: "ZAR")
    /// </summary>
    string GetCurrency();

    /// <summary>
    /// Gets the currency symbol for the current business (default: "R")
    /// </summary>
    string GetCurrencySymbol();

    /// <summary>
    /// Gets the VAT rate for the current business (default: 0.15 for 15%)
    /// </summary>
    decimal GetVatRate();

    /// <summary>
    /// Gets whether prices include VAT by default for the current business
    /// </summary>
    bool GetPricesIncludeVat();

    /// <summary>
    /// Calculates VAT amount from a price
    /// </summary>
    /// <param name="price">The price (VAT included or excluded depending on settings)</param>
    /// <param name="priceIncludesVat">Whether the provided price includes VAT</param>
    /// <returns>The VAT amount</returns>
    decimal CalculateVat(decimal price, bool? priceIncludesVat = null);

    /// <summary>
    /// Calculates price excluding VAT
    /// </summary>
    /// <param name="priceIncludingVat">The price including VAT</param>
    /// <returns>The price excluding VAT</returns>
    decimal CalculatePriceExcludingVat(decimal priceIncludingVat);

    /// <summary>
    /// Calculates price including VAT
    /// </summary>
    /// <param name="priceExcludingVat">The price excluding VAT</param>
    /// <returns>The price including VAT</returns>
    decimal CalculatePriceIncludingVat(decimal priceExcludingVat);

    /// <summary>
    /// Formats a decimal amount as currency string
    /// </summary>
    string FormatCurrency(decimal amount);
}

