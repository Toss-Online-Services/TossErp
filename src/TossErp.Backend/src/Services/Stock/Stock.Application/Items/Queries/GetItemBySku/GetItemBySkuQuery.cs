using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Items.Queries.GetItemBySku;

public record GetItemBySkuQuery : IRequest<ItemDto?>
{
    public string Sku { get; init; } = string.Empty;
} 
