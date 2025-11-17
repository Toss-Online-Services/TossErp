using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;

namespace Toss.Application.Catalog.Queries.GetProductReviews;

public record ProductReviewDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public int CustomerId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ReviewText { get; init; } = string.Empty;
    public int Rating { get; init; }
    public bool IsApproved { get; init; }
    public DateTimeOffset Created { get; init; }
}

public record GetProductReviewsQuery : IRequest<PaginatedList<ProductReviewDto>>
{
    public int ProductId { get; init; }
    public bool? ApprovedOnly { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetProductReviewsQueryHandler : IRequestHandler<GetProductReviewsQuery, PaginatedList<ProductReviewDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductReviewsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<ProductReviewDto>> Handle(GetProductReviewsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ProductReviews
            .Where(r => r.ProductId == request.ProductId);

        if (request.ApprovedOnly == true)
        {
            query = query.Where(r => r.IsApproved);
        }

        var reviews = await query
            .OrderByDescending(r => r.Created)
            .Select(r => new ProductReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                CustomerId = r.CustomerId,
                Title = r.Title,
                ReviewText = r.ReviewText,
                Rating = r.Rating,
                IsApproved = r.IsApproved,
                Created = r.Created
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return reviews;
    }
}

