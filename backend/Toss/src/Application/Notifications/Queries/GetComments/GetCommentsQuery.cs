using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Notifications.Queries.GetComments;

public record GetCommentsQuery : IRequest<List<CommentDto>>
{
    public string LinkedType { get; init; } = string.Empty;
    public int LinkedId { get; init; }
    public bool IncludeReplies { get; init; } = true;
}

public record CommentDto
{
    public int Id { get; init; }
    public string LinkedType { get; init; } = string.Empty;
    public int LinkedId { get; init; }
    public string Body { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
    public int? ParentCommentId { get; init; }
    public bool IsEdited { get; init; }
    public DateTimeOffset Created { get; init; }
    public DateTimeOffset LastModified { get; init; }
    public List<CommentDto> Replies { get; init; } = new();
}

public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, List<CommentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetCommentsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<List<CommentDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.LinkedType))
        {
            throw new ValidationException("Linked type is required.");
        }

        var query = _context.Comments
            .Where(c => c.BusinessId == _businessContext.CurrentBusinessId!.Value
                && c.LinkedType == request.LinkedType
                && c.LinkedId == request.LinkedId);

        if (request.IncludeReplies)
        {
            query = query.Include(c => c.Replies);
        }
        else
        {
            query = query.Where(c => c.ParentCommentId == null); // Only top-level comments
        }

        var comments = await query
            .OrderBy(c => c.Created)
            .ToListAsync(cancellationToken);

        // Build hierarchical structure
        var topLevelComments = comments.Where(c => c.ParentCommentId == null).ToList();
        
        return topLevelComments.Select(c => MapToDto(c, comments)).ToList();
    }

    private CommentDto MapToDto(Comment comment, List<Comment> allComments)
    {
        var replies = allComments
            .Where(c => c.ParentCommentId == comment.Id)
            .Select(c => MapToDto(c, allComments))
            .ToList();

        return new CommentDto
        {
            Id = comment.Id,
            LinkedType = comment.LinkedType,
            LinkedId = comment.LinkedId,
            Body = comment.Body,
            CreatedBy = comment.CreatedBy ?? string.Empty,
            ParentCommentId = comment.ParentCommentId,
            IsEdited = comment.IsEdited,
            Created = comment.Created,
            LastModified = comment.LastModified,
            Replies = replies
        };
    }
}

