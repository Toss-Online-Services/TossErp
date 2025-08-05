using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Batches.Commands.CreateBatch;

public record CreateBatchCommand : IRequest<BatchDto>
{
    public Guid ItemId { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime? ManufacturingDate { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public DateTime? WarrantyExpiryDate { get; init; }
    public string? Supplier { get; init; }
    public string? ReferenceDocumentType { get; init; }
    public string? ReferenceDocumentNo { get; init; }
    public string? ReferenceDocumentDetailNo { get; init; }
    public string? Description { get; init; }
    public string? Remarks { get; init; }
    public decimal Quantity { get; init; }
    public decimal RetainSample { get; init; }
    public decimal RetainSampleQuantity { get; init; }
    public string RetainSampleUOM { get; init; } = string.Empty;
    public decimal RetainSampleUOMQuantity { get; init; }
    public string RetainSampleWarehouse { get; init; } = string.Empty;
    public string RetainSampleBin { get; init; } = string.Empty;
} 
