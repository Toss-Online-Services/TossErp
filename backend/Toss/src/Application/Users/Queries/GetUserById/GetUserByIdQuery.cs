using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;

namespace Toss.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery : IRequest<UserDetailInfoDto>
{
    public string Id { get; init; } = string.Empty;
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDetailInfoDto>
{
    private readonly IUserManagementService _userManagementService;

    public GetUserByIdQueryHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<UserDetailInfoDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManagementService.GetUserByIdAsync(request.Id, cancellationToken);

        if (user == null)
            throw new Common.Exceptions.NotFoundException("User", request.Id);

        return user;
    }
}

