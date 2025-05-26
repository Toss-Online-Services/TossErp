namespace Catalog.Domain.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public int CatalogItemId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public bool IsVerifiedPurchase { get; set; }
    public int HelpfulVotes { get; set; }
} 
