using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Inventory;

namespace Toss.Application.Catalog.Commands.CreateProductReview;

public record CreateProductReviewCommand : IRequest<int>
{
    public int ProductId { get; init; }
    public int CustomerId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ReviewText { get; init; } = string.Empty;
    public int Rating { get; init; }
}

public class CreateProductReviewCommandHandler : IRequestHandler<CreateProductReviewCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateProductReviewCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateProductReviewCommand request, CancellationToken cancellationToken)
    {
        // Validate product exists
        var product = await _context.Products.FindAsync(new object[] { request.ProductId }, cancellationToken);
        if (product == null)
            throw new NotFoundException(nameof(Product), request.ProductId.ToString());

        // Validate customer exists
        var customer = await _context.Customers.FindAsync(new object[] { request.CustomerId }, cancellationToken);
        if (customer == null)
            throw new NotFoundException(nameof(Domain.Entities.CRM.Customer), request.CustomerId.ToString());

        var review = new ProductReview
        {
            ProductId = request.ProductId,
            CustomerId = request.CustomerId,
            Title = request.Title,
            ReviewText = request.ReviewText,
            Rating = request.Rating,
            IsApproved = false,
            HelpfulYesTotal = 0,
            HelpfulNoTotal = 0
        };

        _context.ProductReviews.Add(review);
        await _context.SaveChangesAsync(cancellationToken);

        return review.Id;
    }
}

