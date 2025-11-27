using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Models;

namespace Toss.Application.Businesses.Members.Commands.RemoveBusinessMember;

public record RemoveBusinessMemberCommand : IRequest<Result>
{
    public int BusinessId { get; init; }
    public string UserId { get; init; } = string.Empty;
}

public class RemoveBusinessMemberCommandHandler : IRequestHandler<RemoveBusinessMemberCommand, Result>
{
    private readonly IUserManagementService _userManagementService;

    public RemoveBusinessMemberCommandHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public Task<Result> Handle(RemoveBusinessMemberCommand request, CancellationToken cancellationToken)
    {
        return _userManagementService.RemoveBusinessMemberAsync(
            request.BusinessId,
            request.UserId,
            cancellationToken);
    }
}

