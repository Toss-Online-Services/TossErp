using Toss.Application.Common.Interfaces;

namespace Toss.Application.Users.Commands.UpdateUserRoles;

public record UpdateUserRolesCommand : IRequest<bool>
{
    public string UserId { get; init; } = string.Empty;
    public List<string> Roles { get; init; } = new();
}

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, bool>
{
    private readonly IUserManagementService _userManagementService;

    public UpdateUserRolesCommandHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<bool> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.UpdateUserRolesAsync(
            request.UserId,
            request.Roles,
            cancellationToken
        );

        return result;
    }
}

