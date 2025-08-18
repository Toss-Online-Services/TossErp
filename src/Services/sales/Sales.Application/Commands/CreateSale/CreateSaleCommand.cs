using MediatR;
using TossErp.Sales.Application.Common.DTOs;

namespace TossErp.Sales.Application.Commands.CreateSale;

/// <summary>
/// Command to create a new sale
/// </summary>
public class CreateSaleCommand : IRequest<SaleDto>
{
    public Guid TillId { get; set; }
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<CreateSaleItemRequest> Items { get; set; } = new();
    public decimal? DiscountAmount { get; set; }
    public string? DiscountReason { get; set; }
    public string? Notes { get; set; }
}
