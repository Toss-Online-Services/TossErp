using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehouseAnalytics;

public class GetWarehouseAnalyticsQueryHandler : IRequestHandler<GetWarehouseAnalyticsQuery, WarehouseAnalyticsDto>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetWarehouseAnalyticsQueryHandler> _logger;

    public GetWarehouseAnalyticsQueryHandler(
        IWarehouseRepository warehouseRepository,
        ILogger<GetWarehouseAnalyticsQueryHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<WarehouseAnalyticsDto> Handle(GetWarehouseAnalyticsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting analytics for warehouse: {WarehouseId}", request.WarehouseId);

        var analytics = await _warehouseRepository.GetAnalyticsAsync(request.WarehouseId, cancellationToken);
        
        if (analytics == null)
        {
            return new WarehouseAnalyticsDto
            {
                WarehouseId = request.WarehouseId,
                TotalItems = 0,
                TotalValue = 0,
                UtilizationPercentage = 0,
                LowStockItems = 0,
                OutOfStockItems = 0
            };
        }

        // Cast the anonymous object to the DTO
        var analyticsDict = (dynamic)analytics;
        return new WarehouseAnalyticsDto
        {
            WarehouseId = analyticsDict.WarehouseId,
            WarehouseName = analyticsDict.WarehouseName,
            TotalItems = analyticsDict.TotalItems,
            TotalValue = analyticsDict.TotalValue,
            UtilizationPercentage = analyticsDict.UtilizationPercentage,
            LowStockItems = analyticsDict.LowStockItems,
            OutOfStockItems = analyticsDict.OutOfStockItems,
            TotalBins = analyticsDict.TotalBins,
            OccupiedBins = analyticsDict.OccupiedBins,
            AverageItemValue = analyticsDict.AverageItemValue,
            LastUpdated = analyticsDict.LastUpdated
        };
    }
} 
