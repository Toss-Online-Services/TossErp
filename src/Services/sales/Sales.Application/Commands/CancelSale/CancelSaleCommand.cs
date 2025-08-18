using MediatR;
using TossErp.Sales.Application.Common.DTOs;

namespace TossErp.Sales.Application.Commands.CancelSale;

/// <summary>
/// Command to cancel a sale
/// </summary>
public class CancelSaleCommand : IRequest<SaleDto>
{
    public Guid SaleId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
