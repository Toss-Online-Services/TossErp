namespace Catalog.Application.DTOs;

public class ProductReviewDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string Title { get; set; }
    public string ReviewText { get; set; }
    public int Rating { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public string ReplyText { get; set; }
    public DateTime? ReplyDateUtc { get; set; }
} 
