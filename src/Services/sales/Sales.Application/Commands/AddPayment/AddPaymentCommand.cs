using MediatR;
using TossErp.Sales.Application.Common.DTOs;
using TossErp.Sales.Domain.Enums;

namespace TossErp.Sales.Application.Commands.AddPayment;

/// <summary>
/// Command to add payment to a sale
/// </summary>
public class AddPaymentCommand : IRequest<SaleDto>
{
    public Guid SaleId { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
}
