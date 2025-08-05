using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Batches.Queries.GetBatchById;

public record GetBatchByIdQuery : IRequest<BatchDto?>
{
    public Guid Id { get; init; }
} 
