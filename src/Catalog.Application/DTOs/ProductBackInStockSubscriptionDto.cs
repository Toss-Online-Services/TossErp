namespace Catalog.Application.DTOs;

public class ProductBackInStockSubscriptionDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerEmail { get; set; }
    public DateTime CreatedOnUtc { get; set; }
} 
