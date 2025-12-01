using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Notifications.Commands.UpdateComment;

public record UpdateCommentCommand : IRequest<bool>
{
    public int CommentId { get; init; }
    public string Body { get; init; } = string.Empty;
}

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly IUser _user;

    public UpdateCommentCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        IUser user)
    {
        _context = context;
        _businessContext = businessContext;
        _user = user;
    }

    public async Task<bool> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (string.IsNullOrWhiteSpace(request.Body))
        {
            throw new ValidationException("Comment body is required.");
        }

        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == request.CommentId
                && c.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (comment == null)
        {
            return false;
        }

        // Only the creator can edit
        if (comment.CreatedBy != _user.Id)
        {
            throw new ForbiddenAccessException("Only the comment creator can edit this comment.");
        }

        comment.Body = request.Body;
        comment.IsEdited = true;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

