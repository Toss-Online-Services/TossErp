using Toss.Application.Common.Interfaces;

namespace Toss.Application.Users.Commands.ActivateUser;

public record ActivateUserCommand : IRequest<bool>
{
    public string UserId { get; init; } = string.Empty;
}

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, bool>
{
    private readonly IUserManagementService _userManagementService;

    public ActivateUserCommandHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<bool> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        return await _userManagementService.ActivateUserAsync(
            request.UserId,
            cancellationToken
        );
    }
}

