using System.ComponentModel.DataAnnotations;
using MediatR;

namespace TossErp.Stock.Application.Commands.UpdateItem;

public class UpdateItemCommand : IRequest<bool>
{
    [Required]
    public Guid ItemId { get; set; }

    [StringLength(100)]
    public string? Barcode { get; set; }

    [StringLength(200)]
    public string? Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Selling price must be greater than zero")]
    public decimal? SellingPrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Cost price cannot be negative")]
    public decimal? CostPrice { get; set; }
}
