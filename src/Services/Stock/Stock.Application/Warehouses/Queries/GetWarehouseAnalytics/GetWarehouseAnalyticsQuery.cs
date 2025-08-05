using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Warehouses.Queries.GetWarehouseAnalytics;

public record GetWarehouseAnalyticsQuery(Guid WarehouseId) : IRequest<WarehouseAnalyticsDto>; 
