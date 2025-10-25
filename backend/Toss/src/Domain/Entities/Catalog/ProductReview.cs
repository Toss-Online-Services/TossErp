namespace Toss.Domain.Entities.Catalog;

/// <summary>
/// Represents a product review from customers
/// </summary>
public class ProductReview : BaseAuditableEntity
{
    public ProductReview()
    {
        Title = string.Empty;
        ReviewText = string.Empty;
        Rating = 0;
        IsApproved = false;
    }

    /// <summary>
    /// Gets or sets the product ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the review title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the review text
    /// </summary>
    public string ReviewText { get; set; }

    /// <summary>
    /// Gets or sets the rating (1-5)
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Gets or sets whether the review is approved
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Gets or sets the helpfulness yes total
    /// </summary>
    public int HelpfulYesTotal { get; set; }

    /// <summary>
    /// Gets or sets the helpfulness no total
    /// </summary>
    public int HelpfulNoTotal { get; set; }

    /// <summary>
    /// Gets or sets the store ID
    /// </summary>
    public int? StoreId { get; set; }
}

