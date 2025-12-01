using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Notifications.Commands.CreateComment;

public record CreateCommentCommand : IRequest<int>
{
    public string LinkedType { get; init; } = string.Empty;
    public int LinkedId { get; init; }
    public string Body { get; init; } = string.Empty;
    public int? ParentCommentId { get; init; }
}

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly IUser _user;

    public CreateCommentCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        IUser user)
    {
        _context = context;
        _businessContext = businessContext;
        _user = user;
    }

    public async Task<int> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.LinkedType))
        {
            throw new ValidationException("Linked type is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Body))
        {
            throw new ValidationException("Comment body is required.");
        }

        if (string.IsNullOrWhiteSpace(_user.Id))
        {
            throw new ForbiddenAccessException("User must be authenticated to create comments.");
        }

        // Validate parent comment exists if provided
        if (request.ParentCommentId.HasValue)
        {
            var parentExists = await _context.Comments
                .AnyAsync(c => c.Id == request.ParentCommentId.Value
                    && c.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

            if (!parentExists)
            {
                throw new NotFoundException("Comment", request.ParentCommentId.Value.ToString());
            }
        }

        var comment = new Comment
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            LinkedType = request.LinkedType,
            LinkedId = request.LinkedId,
            Body = request.Body,
            ParentCommentId = request.ParentCommentId
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);

        // TODO: Publish CommentAddedEvent to notify relevant users
        // This would be done via domain events or MediatR notification

        return comment.Id;
    }
}

