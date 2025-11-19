using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Mappings;
using Toss.Application.Common.Models;

namespace Toss.Application.ArtificialIntelligence.Queries.GetAIConversations;

public record AIConversationDto
{
    public int Id { get; init; }
    public string? Title { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastMessageAt { get; init; }
    public bool IsActive { get; init; }
    public int MessageCount { get; init; }
}

public record GetAIConversationsQuery : IRequest<PaginatedList<AIConversationDto>>
{
    public int ShopId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetAIConversationsQueryHandler : IRequestHandler<GetAIConversationsQuery, PaginatedList<AIConversationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAIConversationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<AIConversationDto>> Handle(GetAIConversationsQuery request, CancellationToken cancellationToken)
    {
        var conversations = await _context.AIConversations
            .Where(c => c.ShopId == request.ShopId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new AIConversationDto
            {
                Id = c.Id,
                Title = c.Title,
                CreatedAt = c.CreatedAt,
                LastMessageAt = c.LastMessageAt,
                IsActive = c.IsActive,
                MessageCount = c.Messages.Count
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return conversations;
    }
}

