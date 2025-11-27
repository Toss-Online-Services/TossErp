using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Models;

namespace Toss.Application.Businesses.Members.Commands.UpsertBusinessMember;

public record UpsertBusinessMemberCommand : IRequest<Result>
{
    public int BusinessId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public bool IsDefault { get; init; }
}

public class UpsertBusinessMemberCommandHandler : IRequestHandler<UpsertBusinessMemberCommand, Result>
{
    private readonly IUserManagementService _userManagementService;

    public UpsertBusinessMemberCommandHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public Task<Result> Handle(UpsertBusinessMemberCommand request, CancellationToken cancellationToken)
    {
        return _userManagementService.UpsertBusinessMemberAsync(
            request.BusinessId,
            request.UserId,
            request.Role,
            request.IsDefault,
            cancellationToken);
    }
}

