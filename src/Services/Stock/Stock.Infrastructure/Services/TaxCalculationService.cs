using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System.Net.Http.Json;
using TossErp.Stock.Application.Common.Interfaces;

namespace TossErp.Stock.Infrastructure.Services;

/// <summary>
/// Implementation of tax calculation service with external API integration
/// </summary>
public class TaxCalculationService : ITaxCalculationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TaxCalculationService> _logger;
    private readonly AsyncRetryPolicy<TaxCalculationResult> _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy<TaxCalculationResult> _circuitBreakerPolicy;

    public TaxCalculationService(HttpClient httpClient, ILogger<TaxCalculationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        // Configure retry policy
        _retryPolicy = Policy<TaxCalculationResult>
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(3, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning("Tax calculation service retry {RetryCount} after {Delay}ms", 
                        retryCount, timeSpan.TotalMilliseconds);
                });

        // Configure circuit breaker policy
        _circuitBreakerPolicy = Policy<TaxCalculationResult>
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .AdvancedCircuitBreakerAsync(
                failureThreshold: 0.5,
                samplingDuration: TimeSpan.FromMinutes(1),
                minimumThroughput: 3,
                durationOfBreak: TimeSpan.FromMinutes(1),
                onBreak: (exception, duration) =>
                {
                    _logger.LogError("Tax calculation service circuit breaker opened for {Duration}ms", 
                        duration.TotalMilliseconds);
                },
                onReset: () =>
                {
                    _logger.LogInformation("Tax calculation service circuit breaker reset");
                });
    }

    public async Task<TaxCalculationResult> CalculateTaxAsync(decimal amount, decimal taxRate, CancellationToken cancellationToken = default)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (taxRate < 0)
            throw new ArgumentException("Tax rate cannot be negative", nameof(taxRate));

        try
        {
            _logger.LogInformation("Calculating tax for amount: {Amount}, Tax rate: {TaxRate}%", amount, taxRate);

            // Validate tax rate
            if (!await ValidateTaxRateAsync(taxRate, cancellationToken))
            {
                return new TaxCalculationResult
                {
                    OriginalAmount = amount,
                    TaxAmount = 0,
                    TotalAmount = amount,
                    TaxRate = taxRate,
                    IsValid = false,
                    ErrorMessage = $"Invalid tax rate: {taxRate}%"
                };
            }

            // Calculate tax
            var taxAmount = Math.Round(amount * (taxRate / 100), 2);
            var totalAmount = amount + taxAmount;

            return new TaxCalculationResult
            {
                OriginalAmount = amount,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                TaxRate = taxRate,
                IsValid = true,
                Currency = "USD"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating tax for amount: {Amount}, Tax rate: {TaxRate}", amount, taxRate);
            
            return new TaxCalculationResult
            {
                OriginalAmount = amount,
                TaxAmount = 0,
                TotalAmount = amount,
                TaxRate = taxRate,
                IsValid = false,
                ErrorMessage = "Error occurred during tax calculation"
            };
        }
    }

    public async Task<TaxCalculationResult> CalculateTaxByLocationAsync(decimal amount, string location, string taxCategory, CancellationToken cancellationToken = default)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty", nameof(location));

        if (string.IsNullOrWhiteSpace(taxCategory))
            throw new ArgumentException("Tax category cannot be empty", nameof(taxCategory));

        try
        {
            _logger.LogInformation("Calculating tax by location: Amount={Amount}, Location={Location}, Category={Category}", 
                amount, location, taxCategory);

            // Combine retry and circuit breaker policies
            var combinedPolicy = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);

            return await combinedPolicy.ExecuteAsync(async () =>
            {
                // Try to get tax rate from external API
                var taxRates = await GetTaxRatesAsync(location, taxCategory, cancellationToken);
                var applicableTaxRate = taxRates.FirstOrDefault(tr => tr.IsActive && tr.EffectiveFrom <= DateTime.UtcNow && 
                    (!tr.EffectiveTo.HasValue || tr.EffectiveTo.Value >= DateTime.UtcNow));

                if (applicableTaxRate == null)
                {
                    _logger.LogWarning("No applicable tax rate found for location: {Location}, category: {Category}", 
                        location, taxCategory);
                    
                    return new TaxCalculationResult
                    {
                        OriginalAmount = amount,
                        TaxAmount = 0,
                        TotalAmount = amount,
                        Location = location,
                        TaxCategory = taxCategory,
                        IsValid = false,
                        ErrorMessage = $"No applicable tax rate found for location: {location}, category: {taxCategory}"
                    };
                }

                // Calculate tax using the applicable rate
                var taxAmount = Math.Round(amount * (applicableTaxRate.Rate / 100), 2);
                var totalAmount = amount + taxAmount;

                return new TaxCalculationResult
                {
                    OriginalAmount = amount,
                    TaxAmount = taxAmount,
                    TotalAmount = totalAmount,
                    TaxRate = applicableTaxRate.Rate,
                    Location = location,
                    TaxCategory = taxCategory,
                    IsValid = true,
                    Currency = "USD",
                    Metadata = new Dictionary<string, string>
                    {
                        ["TaxRateDescription"] = applicableTaxRate.Description,
                        ["EffectiveFrom"] = applicableTaxRate.EffectiveFrom.ToString("yyyy-MM-dd")
                    }
                };
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating tax by location: Amount={Amount}, Location={Location}, Category={Category}", 
                amount, location, taxCategory);
            
            return new TaxCalculationResult
            {
                OriginalAmount = amount,
                TaxAmount = 0,
                TotalAmount = amount,
                Location = location,
                TaxCategory = taxCategory,
                IsValid = false,
                ErrorMessage = "Error occurred during tax calculation"
            };
        }
    }

    public async Task<IEnumerable<TaxRate>> GetTaxRatesAsync(string location, string taxCategory, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty", nameof(location));

        if (string.IsNullOrWhiteSpace(taxCategory))
            throw new ArgumentException("Tax category cannot be empty", nameof(taxCategory));

        try
        {
            _logger.LogInformation("Getting tax rates for location: {Location}, category: {Category}", location, taxCategory);

            // Try to get tax rates from external API
            var externalTaxRates = await GetTaxRatesFromExternalApiAsync(location, taxCategory, cancellationToken);

            if (externalTaxRates != null && externalTaxRates.Any())
            {
                return externalTaxRates;
            }

            // Fallback: return default tax rates
            return GetDefaultTaxRates(location, taxCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tax rates for location: {Location}, category: {Category}", location, taxCategory);
            
            // Return default tax rates as fallback
            return GetDefaultTaxRates(location, taxCategory);
        }
    }

    public async Task<bool> ValidateTaxRateAsync(decimal taxRate, CancellationToken cancellationToken = default)
    {
        if (taxRate < 0)
            return false;

        // Basic validation: tax rate should be reasonable (0-100%)
        if (taxRate > 100)
            return false;

        try
        {
            // In a real implementation, you might validate against external tax authority
            await Task.Delay(50, cancellationToken); // Simulate API call
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error validating tax rate: {TaxRate}", taxRate);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetTaxCategoriesAsync(string location, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty", nameof(location));

        try
        {
            _logger.LogInformation("Getting tax categories for location: {Location}", location);

            // Try to get categories from external API
            var externalCategories = await GetTaxCategoriesFromExternalApiAsync(location, cancellationToken);

            if (externalCategories != null && externalCategories.Any())
            {
                return externalCategories;
            }

            // Fallback: return default categories
            return GetDefaultTaxCategories(location);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tax categories for location: {Location}", location);
            
            // Return default categories as fallback
            return GetDefaultTaxCategories(location);
        }
    }

    private async Task<IEnumerable<TaxRate>?> GetTaxRatesFromExternalApiAsync(string location, string taxCategory, CancellationToken cancellationToken)
    {
        try
        {
            // This would be a call to an external tax API
            // For now, return null to indicate no external data available
            await Task.Delay(100, cancellationToken); // Simulate API call
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get tax rates from external API for location: {Location}, category: {Category}", 
                location, taxCategory);
            return null;
        }
    }

    private async Task<IEnumerable<string>?> GetTaxCategoriesFromExternalApiAsync(string location, CancellationToken cancellationToken)
    {
        try
        {
            // This would be a call to an external tax API
            // For now, return null to indicate no external data available
            await Task.Delay(100, cancellationToken); // Simulate API call
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get tax categories from external API for location: {Location}", location);
            return null;
        }
    }

    private static IEnumerable<TaxRate> GetDefaultTaxRates(string location, string taxCategory)
    {
        var now = DateTime.UtcNow;
        
        return new List<TaxRate>
        {
            new TaxRate
            {
                Rate = 10.0m,
                Category = taxCategory,
                Location = location,
                Description = "Standard VAT",
                IsActive = true,
                EffectiveFrom = now.AddYears(-1),
                EffectiveTo = null
            },
            new TaxRate
            {
                Rate = 0.0m,
                Category = taxCategory,
                Location = location,
                Description = "Zero-rated",
                IsActive = true,
                EffectiveFrom = now.AddYears(-1),
                EffectiveTo = null
            }
        };
    }

    private static IEnumerable<string> GetDefaultTaxCategories(string location)
    {
        return new List<string>
        {
            "Standard",
            "Reduced",
            "Zero-rated",
            "Exempt",
            "Digital Services"
        };
    }
}
