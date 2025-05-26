namespace Catalog.Application.DTOs;

public class ProductRentalDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public bool IsRental { get; set; }
    public int RentalPriceLength { get; set; }
    public int RentalPricePeriodId { get; set; }
} 
