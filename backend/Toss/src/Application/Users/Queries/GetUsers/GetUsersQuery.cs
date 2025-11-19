using Toss.Application.Common.Interfaces;

namespace Toss.Application.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<List<UserListItemDto>>
{
    public int Skip { get; init; } = 0;
    public int Take { get; init; } = 50;
    public string? SearchTerm { get; init; }
    public string? Role { get; init; }
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserListItemDto>>
{
    private readonly IUserManagementService _userManagementService;

    public GetUsersQueryHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<List<UserListItemDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)                                                 
    {
        return await _userManagementService.GetUsersAsync(
            request.Skip,
            request.Take,
            request.SearchTerm,
            request.Role,
            cancellationToken
        );
    }
}

