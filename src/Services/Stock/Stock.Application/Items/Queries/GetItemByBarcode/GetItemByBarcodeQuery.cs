using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Items.Queries.GetItemByBarcode;

public record GetItemByBarcodeQuery : IRequest<ItemDto?>
{
    public string Barcode { get; init; } = string.Empty;
} 
