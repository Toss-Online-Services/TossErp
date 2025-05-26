using Catalog.Domain.SeedWork;
using MediatR;

namespace Catalog.Domain.Events;

public class ProductReviewApprovedEvent : INotification
{
    public int ReviewId { get; }
    public int ProductId { get; }
    public int CustomerId { get; }

    public ProductReviewApprovedEvent(int reviewId, int productId, int customerId)
    {
        ReviewId = reviewId;
        ProductId = productId;
        CustomerId = customerId;
    }
} 
