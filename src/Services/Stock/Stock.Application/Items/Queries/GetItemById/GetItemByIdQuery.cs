using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Items.Queries.GetItemById;

/// <summary>
/// Query for retrieving a single item by ID
/// </summary>
public record GetItemByIdQuery : IRequest<ItemDto?>
{
    /// <summary>
    /// The unique identifier of the item
    /// </summary>
    public Guid Id { get; init; }
} 
