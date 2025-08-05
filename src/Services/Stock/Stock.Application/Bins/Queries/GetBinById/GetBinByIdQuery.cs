using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Bins.Queries.GetBinById;

public record GetBinByIdQuery(Guid Id) : IRequest<BinDto>; 
