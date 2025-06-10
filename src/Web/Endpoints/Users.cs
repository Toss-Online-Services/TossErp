using Inventory.API.Infrastructure;
using Inventory.Infrastructure.Identity;

namespace Inventory.API.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapIdentityApi<ApplicationUser>();
    }
}
