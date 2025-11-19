using Toss.Application.Common.Interfaces;

namespace Toss.Application.Users.Commands.DeactivateUser;

public record DeactivateUserCommand : IRequest<bool>
{
    public string UserId { get; init; } = string.Empty;
}

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, bool>
{
    private readonly IUserManagementService _userManagementService;

    public DeactivateUserCommandHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<bool> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        return await _userManagementService.DeactivateUserAsync(
            request.UserId,
            cancellationToken
        );
    }
}

