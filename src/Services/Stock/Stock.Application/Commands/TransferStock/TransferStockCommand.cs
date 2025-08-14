using System.ComponentModel.DataAnnotations;
using MediatR;

namespace TossErp.Stock.Application.Commands.TransferStock;

public class TransferStockCommand : IRequest<bool>
{
    [Required]
    public Guid TenantId { get; set; }

    [Required]
    public Guid ItemId { get; set; }

    [Required]
    public Guid FromWarehouseId { get; set; }

    [Required]
    public Guid ToWarehouseId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
    public decimal Quantity { get; set; }

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = null!;

    [StringLength(200)]
    public string? Reason { get; set; }

    [Required]
    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;
}
