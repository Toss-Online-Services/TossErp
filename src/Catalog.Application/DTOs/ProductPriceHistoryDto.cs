namespace Catalog.Application.DTOs;

public class ProductPriceHistoryDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal OldPrice { get; set; }
    public decimal SpecialPrice { get; set; }
    public DateTime? SpecialPriceStartDateTimeUtc { get; set; }
    public DateTime? SpecialPriceEndDateTimeUtc { get; set; }
    public DateTime CreatedOnUtc { get; set; }
} 
