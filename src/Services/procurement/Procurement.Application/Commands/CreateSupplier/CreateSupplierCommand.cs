using MediatR;
using TossErp.Procurement.Application.Common.DTOs;

namespace TossErp.Procurement.Application.Commands.CreateSupplier;

/// <summary>
/// Command to create a new supplier
/// </summary>
public class CreateSupplierCommand : IRequest<SupplierDto>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? TaxNumber { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Notes { get; set; }
    public decimal? CreditLimit { get; set; }
    public int? PaymentTermsDays { get; set; }
    public decimal? LeadTimeDays { get; set; }
}
