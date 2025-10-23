using Toss.Infrastructure.Identity;

namespace Toss.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapIdentityApi<ApplicationUser>();
    }
}
