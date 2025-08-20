using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.Interfaces;

namespace TossErp.Accounting.Application.Queries.GetStockValuationSummary;

/// <summary>
/// Handler for GetStockValuationSummaryQuery
/// </summary>
public class GetStockValuationSummaryQueryHandler : IRequestHandler<GetStockValuationSummaryQuery, StockValuationSummaryDto>
{
    private readonly IStockValuationService _stockValuationService;
    private readonly ILogger<GetStockValuationSummaryQueryHandler> _logger;

    public GetStockValuationSummaryQueryHandler(
        IStockValuationService stockValuationService,
        ILogger<GetStockValuationSummaryQueryHandler> logger)
    {
        _stockValuationService = stockValuationService;
        _logger = logger;
    }

    public async Task<StockValuationSummaryDto> Handle(GetStockValuationSummaryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting stock valuation summary as of {Date}", request.AsOfDate);

        // For MVP, we'll use a hardcoded tenant ID
        // In a real implementation, this would come from the current user context
        var tenantId = "tenant-001";

        var summary = await _stockValuationService.GetStockValuationSummaryAsync(
            request.AsOfDate, 
            tenantId, 
            cancellationToken);

        var dto = new StockValuationSummaryDto
        {
            AsOfDate = summary.AsOfDate,
            TotalValue = summary.TotalValue.Amount,
            Currency = summary.TotalValue.Currency,
            ItemCount = summary.ItemCount,
            WarehouseCount = summary.WarehouseCount,
            Method = summary.Method,
            WarehouseValues = summary.WarehouseValues.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Amount),
            CategoryValues = summary.CategoryValues.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Amount)
        };

        _logger.LogInformation("Retrieved stock valuation summary with total value {TotalValue} {Currency}", 
            dto.TotalValue, dto.Currency);

        return dto;
    }
}


