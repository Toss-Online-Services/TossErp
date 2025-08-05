using TossErp.Stock.Infrastructure.Identity;
namespace TossErp.Stock.API.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        // Temporarily disabled to test API startup
        // app.MapGroup(this)
        //     .MapIdentityApi<ApplicationUser>();
    }
}
