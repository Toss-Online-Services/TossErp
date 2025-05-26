namespace Catalog.Application.DTOs;

public class ProductTierPriceDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int CustomerRoleId { get; set; }
    public string CustomerRoleName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime? StartDateTimeUtc { get; set; }
    public DateTime? EndDateTimeUtc { get; set; }
} 
