using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Models.Businesses;

namespace Toss.Application.Businesses.Members.Queries.GetBusinessMembers;

public record GetBusinessMembersQuery(int BusinessId) : IRequest<IReadOnlyList<BusinessMemberDto>>;

public class GetBusinessMembersQueryHandler : IRequestHandler<GetBusinessMembersQuery, IReadOnlyList<BusinessMemberDto>>
{
    private readonly IUserManagementService _userManagementService;

    public GetBusinessMembersQueryHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public Task<IReadOnlyList<BusinessMemberDto>> Handle(GetBusinessMembersQuery request, CancellationToken cancellationToken)
    {
        return _userManagementService.GetBusinessMembersAsync(request.BusinessId, cancellationToken);
    }
}

