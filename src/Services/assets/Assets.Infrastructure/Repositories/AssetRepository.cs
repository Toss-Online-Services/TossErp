namespace TossErp.Assets.Infrastructure.Repositories;

/// <summary>
/// Specialized Asset repository with business-specific operations
/// </summary>
public class AssetRepository : Repository<Asset>, IAssetRepository
{
    public AssetRepository(AssetsDbContext context, ILogger<AssetRepository> logger)
        : base(context, logger)
    {
    }

    public async Task<Asset?> GetByAssetTagAsync(
        string assetTag, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.Location)
                .FirstOrDefaultAsync(a => a.AssetTag == assetTag, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset by tag {AssetTag}", assetTag);
            throw;
        }
    }

    public async Task<Asset?> GetBySerialNumberAsync(
        string serialNumber, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.Location)
                .FirstOrDefaultAsync(a => a.SerialNumber == serialNumber, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset by serial number {SerialNumber}", serialNumber);
            throw;
        }
    }

    public async Task<IEnumerable<Asset>> GetByStatusAsync(
        AssetStatus status, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.Location)
                .Where(a => a.Status == status)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assets by status {Status}", status);
            throw;
        }
    }

    public async Task<IEnumerable<Asset>> GetByCategoryAsync(
        Guid categoryId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.Location)
                .Where(a => a.CategoryId == categoryId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assets by category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<Asset>> GetByLocationAsync(
        Guid locationId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.Location)
                .Where(a => a.LocationId == locationId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assets by location {LocationId}", locationId);
            throw;
        }
    }

    public async Task<IEnumerable<Asset>> GetMaintenanceDueAsync(
        DateTime? beforeDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var date = beforeDate ?? DateTime.UtcNow;
            
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.Location)
                .Where(a => a.NextMaintenanceDue.HasValue && a.NextMaintenanceDue <= date)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assets with maintenance due");
            throw;
        }
    }

    public async Task<IEnumerable<Asset>> GetByValueRangeAsync(
        decimal minValue, 
        decimal maxValue, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.Location)
                .Where(a => a.FinancialInfo!.CurrentValue >= minValue && 
                           a.FinancialInfo.CurrentValue <= maxValue)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assets by value range {MinValue}-{MaxValue}", minValue, maxValue);
            throw;
        }
    }

    public async Task<IEnumerable<Asset>> GetDepreciatingAssetsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.Location)
                .Where(a => a.FinancialInfo!.DepreciationRate > 0)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving depreciating assets");
            throw;
        }
    }

    public async Task<bool> IsAssetTagUniqueAsync(
        string assetTag, 
        Guid? excludeAssetId = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _dbSet.Where(a => a.AssetTag == assetTag);
            
            if (excludeAssetId.HasValue)
                query = query.Where(a => a.Id != excludeAssetId.Value);

            return !await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking asset tag uniqueness for {AssetTag}", assetTag);
            throw;
        }
    }

    public async Task<Asset?> GetWithFullDetailsAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.Location)
                .Include(a => a.Transfers.Take(10).OrderByDescending(t => t.TransferDate))
                    .ThenInclude(t => t.FromLocation)
                .Include(a => a.Transfers.Take(10))
                    .ThenInclude(t => t.ToLocation)
                .Include(a => a.MaintenanceRecords.Take(10).OrderByDescending(m => m.ScheduledDate))
                .Include(a => a.Documents.Where(d => d.IsActive))
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset with full details {AssetId}", id);
            throw;
        }
    }

    public async Task<decimal> CalculateCurrentValueAsync(
        Guid assetId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var asset = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == assetId, cancellationToken);

            if (asset?.FinancialInfo == null)
                return 0;

            var yearsOwned = (DateTime.UtcNow - asset.PurchaseDate).TotalDays / 365.25;
            var depreciationAmount = asset.FinancialInfo.PurchasePrice * 
                                   (asset.FinancialInfo.DepreciationRate / 100) * 
                                   (decimal)yearsOwned;

            var currentValue = asset.FinancialInfo.PurchasePrice - depreciationAmount;
            
            // Ensure value doesn't go below salvage value
            return Math.Max(currentValue, asset.FinancialInfo.SalvageValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating current value for asset {AssetId}", assetId);
            throw;
        }
    }

    public async Task<IEnumerable<Asset>> SearchAssetsAsync(
        string searchTerm, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Include(a => a.Category)
                .Include(a => a.Location)
                .Where(a => a.Name.Contains(searchTerm) ||
                           a.AssetTag.Contains(searchTerm) ||
                           a.SerialNumber!.Contains(searchTerm) ||
                           a.Model!.Contains(searchTerm) ||
                           a.Manufacturer!.Contains(searchTerm) ||
                           a.Category!.Name.Contains(searchTerm) ||
                           a.Location!.Name.Contains(searchTerm))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching assets with term {SearchTerm}", searchTerm);
            throw;
        }
    }
}
