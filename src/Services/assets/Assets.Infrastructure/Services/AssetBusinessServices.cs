namespace TossErp.Assets.Infrastructure.Services;

/// <summary>
/// Service for generating asset tags with various numbering schemes
/// </summary>
public class AssetTagGeneratorService : IAssetTagGeneratorService
{
    private readonly AssetsDbContext _context;
    private readonly ICurrentTenantService _currentTenantService;
    private readonly ILogger<AssetTagGeneratorService> _logger;

    public AssetTagGeneratorService(
        AssetsDbContext context,
        ICurrentTenantService currentTenantService,
        ILogger<AssetTagGeneratorService> logger)
    {
        _context = context;
        _currentTenantService = currentTenantService;
        _logger = logger;
    }

    public async Task<string> GenerateAssetTagAsync(
        Guid? categoryId = null, 
        string? customPrefix = null,
        CancellationToken cancellationToken = default)
    {
        var tenantId = _currentTenantService.TenantId;
        
        try
        {
            // Get the category to determine prefix
            string prefix = "AST"; // Default prefix
            
            if (!string.IsNullOrEmpty(customPrefix))
            {
                prefix = customPrefix.ToUpperInvariant();
            }
            else if (categoryId.HasValue)
            {
                var category = await _context.AssetCategories
                    .FirstOrDefaultAsync(c => c.Id == categoryId.Value && c.TenantId == tenantId, cancellationToken);
                
                if (category != null && !string.IsNullOrEmpty(category.Code))
                {
                    prefix = category.Code.ToUpperInvariant();
                }
            }

            // Get the next sequence number for this tenant and prefix
            var lastAsset = await _context.Assets
                .Where(a => a.TenantId == tenantId && a.AssetTag.StartsWith(prefix))
                .OrderByDescending(a => a.AssetTag)
                .FirstOrDefaultAsync(cancellationToken);

            int nextSequence = 1;
            
            if (lastAsset != null)
            {
                // Extract the numeric part from the last asset tag
                var numericPart = lastAsset.AssetTag.Substring(prefix.Length);
                if (int.TryParse(numericPart, out int lastSequence))
                {
                    nextSequence = lastSequence + 1;
                }
            }

            // Format the asset tag with zero-padding
            var assetTag = $"{prefix}{nextSequence:D6}"; // 6-digit zero-padded number

            // Ensure uniqueness (in case of concurrent operations)
            var existingAsset = await _context.Assets
                .AnyAsync(a => a.TenantId == tenantId && a.AssetTag == assetTag, cancellationToken);

            if (existingAsset)
            {
                // If tag exists, increment and try again
                assetTag = $"{prefix}{(nextSequence + 1):D6}";
            }

            _logger.LogInformation("Generated asset tag {AssetTag} for tenant {TenantId}", assetTag, tenantId);
            return assetTag;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate asset tag for tenant {TenantId}", tenantId);
            
            // Fallback to GUID-based tag
            var fallbackTag = $"AST{Guid.NewGuid():N}"[..12].ToUpperInvariant();
            return fallbackTag;
        }
    }

    public async Task<bool> IsAssetTagUniqueAsync(string assetTag, CancellationToken cancellationToken = default)
    {
        var tenantId = _currentTenantService.TenantId;
        
        var exists = await _context.Assets
            .AnyAsync(a => a.TenantId == tenantId && a.AssetTag == assetTag, cancellationToken);
        
        return !exists;
    }

    public async Task<string> GenerateQrCodeAsync(string assetTag, CancellationToken cancellationToken = default)
    {
        // This would integrate with a QR code generation library
        // For now, return a placeholder URL that could be used to generate QR codes
        var tenantId = _currentTenantService.TenantId;
        var qrData = $"https://app.tosserp.com/assets/{tenantId}/{assetTag}";
        
        _logger.LogInformation("Generated QR code data for asset {AssetTag}: {QrData}", assetTag, qrData);
        return qrData;
    }

    public async Task<string> GenerateBarcodeAsync(string assetTag, string format = "CODE128", CancellationToken cancellationToken = default)
    {
        // This would integrate with a barcode generation library
        // For now, return the asset tag formatted for the specified barcode type
        var barcodeData = format.ToUpperInvariant() switch
        {
            "CODE128" => assetTag,
            "CODE39" => assetTag.Replace("-", ""),
            "EAN13" => assetTag.PadLeft(13, '0')[..13],
            _ => assetTag
        };

        _logger.LogInformation("Generated {Format} barcode data for asset {AssetTag}: {BarcodeData}", 
            format, assetTag, barcodeData);
        
        return barcodeData;
    }
}

/// <summary>
/// Service for calculating asset depreciation using various methods
/// </summary>
public class AssetDepreciationService : IAssetDepreciationService
{
    private readonly ILogger<AssetDepreciationService> _logger;

    public AssetDepreciationService(ILogger<AssetDepreciationService> logger)
    {
        _logger = logger;
    }

    public decimal CalculateDepreciation(
        decimal purchasePrice, 
        decimal salvageValue, 
        int usefulLifeYears, 
        DateTime purchaseDate, 
        DateTime calculationDate,
        string method = "StraightLine")
    {
        if (purchasePrice <= 0 || usefulLifeYears <= 0)
            return 0;

        var depreciableAmount = purchasePrice - salvageValue;
        if (depreciableAmount <= 0)
            return 0;

        var yearsElapsed = (decimal)(calculationDate - purchaseDate).TotalDays / 365.25m;
        if (yearsElapsed <= 0)
            return 0;

        return method.ToLowerInvariant() switch
        {
            "straightline" or "straight-line" => CalculateStraightLineDepreciation(depreciableAmount, usefulLifeYears, yearsElapsed),
            "decliningbalance" or "declining-balance" => CalculateDecliningBalanceDepreciation(purchasePrice, usefulLifeYears, yearsElapsed),
            "doubledeclining" or "double-declining" => CalculateDoubleDecliningDepreciation(purchasePrice, usefulLifeYears, yearsElapsed),
            "sumofyears" or "sum-of-years" => CalculateSumOfYearsDepreciation(depreciableAmount, usefulLifeYears, yearsElapsed),
            _ => CalculateStraightLineDepreciation(depreciableAmount, usefulLifeYears, yearsElapsed)
        };
    }

    public decimal CalculateCurrentValue(
        decimal purchasePrice, 
        decimal accumulatedDepreciation)
    {
        return Math.Max(0, purchasePrice - accumulatedDepreciation);
    }

    public decimal CalculateMonthlyDepreciation(
        decimal purchasePrice, 
        decimal salvageValue, 
        int usefulLifeYears,
        string method = "StraightLine")
    {
        var annualDepreciation = CalculateAnnualDepreciation(purchasePrice, salvageValue, usefulLifeYears, method);
        return annualDepreciation / 12;
    }

    public decimal CalculateAnnualDepreciation(
        decimal purchasePrice, 
        decimal salvageValue, 
        int usefulLifeYears,
        string method = "StraightLine")
    {
        if (purchasePrice <= 0 || usefulLifeYears <= 0)
            return 0;

        var depreciableAmount = purchasePrice - salvageValue;
        if (depreciableAmount <= 0)
            return 0;

        return method.ToLowerInvariant() switch
        {
            "straightline" or "straight-line" => depreciableAmount / usefulLifeYears,
            "decliningbalance" or "declining-balance" => purchasePrice * (1.0m / usefulLifeYears),
            "doubledeclining" or "double-declining" => purchasePrice * (2.0m / usefulLifeYears),
            _ => depreciableAmount / usefulLifeYears
        };
    }

    private decimal CalculateStraightLineDepreciation(decimal depreciableAmount, int usefulLifeYears, decimal yearsElapsed)
    {
        var annualDepreciation = depreciableAmount / usefulLifeYears;
        var totalDepreciation = annualDepreciation * Math.Min(yearsElapsed, usefulLifeYears);
        
        _logger.LogDebug("Straight-line depreciation: {TotalDepreciation} (Annual: {AnnualDepreciation}, Years: {YearsElapsed})", 
            totalDepreciation, annualDepreciation, yearsElapsed);
        
        return Math.Max(0, totalDepreciation);
    }

    private decimal CalculateDecliningBalanceDepreciation(decimal purchasePrice, int usefulLifeYears, decimal yearsElapsed)
    {
        var rate = 1.0m / usefulLifeYears;
        var remainingValue = purchasePrice;
        var totalDepreciation = 0m;

        for (int year = 1; year <= Math.Ceiling(yearsElapsed) && year <= usefulLifeYears; year++)
        {
            var yearDepreciation = remainingValue * rate;
            var actualYearDepreciation = year <= yearsElapsed ? yearDepreciation : yearDepreciation * (yearsElapsed - year + 1);
            
            totalDepreciation += actualYearDepreciation;
            remainingValue -= yearDepreciation;
        }

        _logger.LogDebug("Declining balance depreciation: {TotalDepreciation} (Rate: {Rate}, Years: {YearsElapsed})", 
            totalDepreciation, rate, yearsElapsed);

        return Math.Max(0, totalDepreciation);
    }

    private decimal CalculateDoubleDecliningDepreciation(decimal purchasePrice, int usefulLifeYears, decimal yearsElapsed)
    {
        var rate = 2.0m / usefulLifeYears;
        var remainingValue = purchasePrice;
        var totalDepreciation = 0m;

        for (int year = 1; year <= Math.Ceiling(yearsElapsed) && year <= usefulLifeYears; year++)
        {
            var yearDepreciation = remainingValue * rate;
            var actualYearDepreciation = year <= yearsElapsed ? yearDepreciation : yearDepreciation * (yearsElapsed - year + 1);
            
            totalDepreciation += actualYearDepreciation;
            remainingValue -= yearDepreciation;
        }

        _logger.LogDebug("Double declining depreciation: {TotalDepreciation} (Rate: {Rate}, Years: {YearsElapsed})", 
            totalDepreciation, rate, yearsElapsed);

        return Math.Max(0, totalDepreciation);
    }

    private decimal CalculateSumOfYearsDepreciation(decimal depreciableAmount, int usefulLifeYears, decimal yearsElapsed)
    {
        var sumOfYears = usefulLifeYears * (usefulLifeYears + 1) / 2;
        var totalDepreciation = 0m;

        for (int year = 1; year <= Math.Ceiling(yearsElapsed) && year <= usefulLifeYears; year++)
        {
            var remainingYears = usefulLifeYears - year + 1;
            var yearDepreciation = depreciableAmount * remainingYears / sumOfYears;
            var actualYearDepreciation = year <= yearsElapsed ? yearDepreciation : yearDepreciation * (yearsElapsed - year + 1);
            
            totalDepreciation += actualYearDepreciation;
        }

        _logger.LogDebug("Sum of years depreciation: {TotalDepreciation} (Years: {YearsElapsed})", 
            totalDepreciation, yearsElapsed);

        return Math.Max(0, totalDepreciation);
    }
}
