using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Batches.Queries.GetBatches;

public record GetBatchesQuery : IRequest<List<BatchDto>>
{
    public Guid? ItemId { get; init; }
    public string? SearchTerm { get; init; }
    public bool? IsDisabled { get; init; }
    public DateTime? ExpiryDateFrom { get; init; }
    public DateTime? ExpiryDateTo { get; init; }
} 
