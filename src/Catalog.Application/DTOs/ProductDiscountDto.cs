namespace Catalog.Application.DTOs;

public class ProductDiscountDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int DiscountId { get; set; }
    public string DiscountName { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime? StartDateTimeUtc { get; set; }
    public DateTime? EndDateTimeUtc { get; set; }
} 
