using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System.Net.Http.Json;
using TossErp.Stock.Application.Common.Interfaces;

namespace TossErp.Stock.Infrastructure.Services;

/// <summary>
/// Implementation of barcode service with external API integration
/// </summary>
public class BarcodeService : IBarcodeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BarcodeService> _logger;
    private readonly AsyncRetryPolicy<BarcodeScanResult?> _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy<BarcodeScanResult?> _circuitBreakerPolicy;

    public BarcodeService(HttpClient httpClient, ILogger<BarcodeService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        // Configure retry policy
        _retryPolicy = Policy<BarcodeScanResult?>
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(3, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning("Barcode service retry {RetryCount} after {Delay}ms", 
                        retryCount, timeSpan.TotalMilliseconds);
                });

        // Configure circuit breaker policy
        _circuitBreakerPolicy = Policy<BarcodeScanResult?>
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .AdvancedCircuitBreakerAsync(
                failureThreshold: 0.5,
                samplingDuration: TimeSpan.FromMinutes(1),
                minimumThroughput: 3,
                durationOfBreak: TimeSpan.FromMinutes(1),
                onBreak: (exception, duration) =>
                {
                    _logger.LogError("Barcode service circuit breaker opened for {Duration}ms", 
                        duration.TotalMilliseconds);
                },
                onReset: () =>
                {
                    _logger.LogInformation("Barcode service circuit breaker reset");
                });
    }

    public async Task<bool> ValidateBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return false;

        try
        {
            // Basic validation logic
            var barcodeType = await GetBarcodeTypeAsync(barcode, cancellationToken);
            
            return barcodeType switch
            {
                BarcodeType.EAN13 => barcode.Length == 13 && IsNumeric(barcode),
                BarcodeType.EAN8 => barcode.Length == 8 && IsNumeric(barcode),
                BarcodeType.UPCA => barcode.Length == 12 && IsNumeric(barcode),
                BarcodeType.UPCE => barcode.Length == 8 && IsNumeric(barcode),
                BarcodeType.Code128 => barcode.Length > 0,
                BarcodeType.Code39 => barcode.Length > 0 && IsCode39Valid(barcode),
                _ => false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating barcode: {Barcode}", barcode);
            return false;
        }
    }

    public async Task<BarcodeScanResult?> ScanBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return null;

        try
        {
            // Combine retry and circuit breaker policies
            var combinedPolicy = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);

            return await combinedPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation("Scanning barcode: {Barcode}", barcode);

                // Try to get product information from external API
                var productInfo = await GetProductInfoFromExternalApiAsync(barcode, cancellationToken);

                if (productInfo != null)
                {
                    return new BarcodeScanResult
                    {
                        Barcode = barcode,
                        ProductCode = productInfo.ProductCode,
                        ProductName = productInfo.ProductName,
                        BarcodeType = await GetBarcodeTypeAsync(barcode, cancellationToken),
                        IsValid = true,
                        Metadata = productInfo.Metadata
                    };
                }

                // Fallback: return basic scan result
                return new BarcodeScanResult
                {
                    Barcode = barcode,
                    BarcodeType = await GetBarcodeTypeAsync(barcode, cancellationToken),
                    IsValid = await ValidateBarcodeAsync(barcode, cancellationToken)
                };
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scanning barcode: {Barcode}", barcode);
            return null;
        }
    }

    public Task<string> GenerateBarcodeAsync(string productCode, BarcodeType barcodeType = BarcodeType.Code128, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("Product code cannot be empty", nameof(productCode));

        try
        {
            _logger.LogInformation("Generating barcode for product: {ProductCode}, Type: {BarcodeType}", 
                productCode, barcodeType);

            // For now, return a simple generated barcode
            // In a real implementation, you would call an external barcode generation service
            var barcode = barcodeType switch
            {
                BarcodeType.Code128 => $"128{productCode}",
                BarcodeType.Code39 => $"39{productCode}",
                BarcodeType.EAN13 => GenerateEAN13(productCode),
                BarcodeType.EAN8 => GenerateEAN8(productCode),
                BarcodeType.UPCA => GenerateUPCA(productCode),
                BarcodeType.UPCE => GenerateUPCE(productCode),
                _ => productCode
            };

            return Task.FromResult(barcode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating barcode for product: {ProductCode}", productCode);
            throw;
        }
    }

    public Task<BarcodeType> GetBarcodeTypeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return Task.FromResult(BarcodeType.Unknown);

        // Simple barcode type detection based on length and format
        if (barcode.Length == 13 && IsNumeric(barcode))
            return Task.FromResult(BarcodeType.EAN13);
        
        if (barcode.Length == 8 && IsNumeric(barcode))
            return Task.FromResult(BarcodeType.EAN8);
        
        if (barcode.Length == 12 && IsNumeric(barcode))
            return Task.FromResult(BarcodeType.UPCA);
        
        if (barcode.Length == 8 && IsNumeric(barcode))
            return Task.FromResult(BarcodeType.UPCE);
        
        if (IsCode39Valid(barcode))
            return Task.FromResult(BarcodeType.Code39);
        
        // Default to Code128 for other formats
        return Task.FromResult(BarcodeType.Code128);
    }

    private async Task<ProductInfo?> GetProductInfoFromExternalApiAsync(string barcode, CancellationToken cancellationToken)
    {
        try
        {
            // This would be a call to an external barcode database API
            // For now, return null to indicate no external data available
            await Task.Delay(100, cancellationToken); // Simulate API call
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get product info from external API for barcode: {Barcode}", barcode);
            return null;
        }
    }

    private static bool IsNumeric(string value)
    {
        return value.All(char.IsDigit);
    }

    private static bool IsCode39Valid(string barcode)
    {
        // Code39 can contain digits, uppercase letters, and some special characters
        return barcode.All(c => char.IsDigit(c) || char.IsUpper(c) || c == '-' || c == '.' || c == ' ' || c == '$' || c == '/' || c == '+' || c == '%');
    }

    private static string GenerateEAN13(string productCode)
    {
        // Simple EAN-13 generation (in real implementation, you'd calculate check digit)
        var padded = productCode.PadLeft(12, '0');
        return padded + "0"; // Add dummy check digit
    }

    private static string GenerateEAN8(string productCode)
    {
        // Simple EAN-8 generation
        var padded = productCode.PadLeft(7, '0');
        return padded + "0"; // Add dummy check digit
    }

    private static string GenerateUPCA(string productCode)
    {
        // Simple UPC-A generation
        return productCode.PadLeft(12, '0');
    }

    private static string GenerateUPCE(string productCode)
    {
        // Simple UPC-E generation
        return productCode.PadLeft(8, '0');
    }

    private record ProductInfo
    {
        public string? ProductCode { get; init; }
        public string? ProductName { get; init; }
        public Dictionary<string, string> Metadata { get; init; } = new();
    }
}
