using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Notifications.Commands.DeleteComment;

public record DeleteCommentCommand : IRequest<bool>
{
    public int CommentId { get; init; }
}

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly IUser _user;

    public DeleteCommentCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        IUser user)
    {
        _context = context;
        _businessContext = businessContext;
        _user = user;
    }

    public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var comment = await _context.Comments
            .Include(c => c.Replies)
            .FirstOrDefaultAsync(c => c.Id == request.CommentId
                && c.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (comment == null)
        {
            return false;
        }

        // Only the creator can delete (or admin/manager)
        if (comment.CreatedBy != _user.Id)
        {
            // TODO: Check if user has admin/manager role
            throw new ForbiddenAccessException("Only the comment creator can delete this comment.");
        }

        // Delete replies first
        if (comment.Replies.Any())
        {
            _context.Comments.RemoveRange(comment.Replies);
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

