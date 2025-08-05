using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Bins.Queries.GetBins;

public record GetBinsQuery : IRequest<List<BinDto>>; 
