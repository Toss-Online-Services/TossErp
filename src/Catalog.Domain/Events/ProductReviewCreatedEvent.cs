using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.Domain.Events;

public class ProductReviewCreatedEvent : INotification
{
    public int ReviewId { get; }
    public int ProductId { get; }
    public int CustomerId { get; }
    public int Rating { get; }

    public ProductReviewCreatedEvent(int reviewId, int productId, int customerId, int rating)
    {
        ReviewId = reviewId;
        ProductId = productId;
        CustomerId = customerId;
        Rating = rating;
    }
} 
