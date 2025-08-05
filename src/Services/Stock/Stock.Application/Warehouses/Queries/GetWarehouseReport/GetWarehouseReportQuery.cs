using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehouseReport;

public record GetWarehouseReportQuery(
    Guid? WarehouseId = null,
    string? ReportType = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null) : IRequest<WarehouseReportDto>; 
