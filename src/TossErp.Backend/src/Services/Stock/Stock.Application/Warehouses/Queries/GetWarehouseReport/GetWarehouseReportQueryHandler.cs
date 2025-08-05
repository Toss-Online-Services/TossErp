using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehouseReport;

public class GetWarehouseReportQueryHandler : IRequestHandler<GetWarehouseReportQuery, WarehouseReportDto>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILogger<GetWarehouseReportQueryHandler> _logger;

    public GetWarehouseReportQueryHandler(
        IWarehouseRepository warehouseRepository,
        ILogger<GetWarehouseReportQueryHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    public async Task<WarehouseReportDto> Handle(GetWarehouseReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting warehouse report for warehouse: {WarehouseId}, type: {ReportType}", 
            request.WarehouseId, request.ReportType);

        var report = await _warehouseRepository.GetReportAsync(
            request.WarehouseId, 
            request.ReportType, 
            request.FromDate, 
            request.ToDate, 
            cancellationToken);
        
        if (report == null)
        {
            return new WarehouseReportDto
            {
                WarehouseId = request.WarehouseId,
                ReportType = request.ReportType ?? "Summary",
                GeneratedDate = DateTime.UtcNow,
                Data = new List<object>()
            };
        }

        // Cast the anonymous object to the DTO
        var reportDict = (dynamic)report;
        return new WarehouseReportDto
        {
            WarehouseId = reportDict.WarehouseId,
            WarehouseName = reportDict.WarehouseName,
            ReportType = reportDict.ReportType,
            GeneratedDate = reportDict.GeneratedDate,
            FromDate = reportDict.FromDate,
            ToDate = reportDict.ToDate,
            Data = reportDict.Data,
            Summary = reportDict.Summary
        };
    }
} 
