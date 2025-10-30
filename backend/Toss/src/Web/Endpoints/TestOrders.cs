using Toss.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Toss.Web.Endpoints;

public class TestOrders : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapGet("test-orders", TestOrdersEndpoint)
            .WithName("TestOrders")
            .WithSummary("Test orders data existence");
    }

    public async Task<IResult> TestOrdersEndpoint(IApplicationDbContext context)
    {
        try
        {
            // Simple count queries
            var totalOrders = await context.Orders.CountAsync();
            var nonDeletedOrders = await context.Orders.Where(o => !o.Deleted).CountAsync();
            var totalCustomers = await context.Customers.CountAsync();
            var customersWithShop1 = await context.Customers.Where(c => c.ShopId == 1).CountAsync();
            
            // Get first 3 orders with basic info
            var sampleOrders = await context.Orders
                .Where(o => !o.Deleted)
                .OrderByDescending(o => o.Created)
                .Take(3)
                .Select(o => new
                {
                    o.Id,
                    o.CustomerId,
                    o.OrderStatus,
                    o.OrderTotal,
                    o.Created
                })
                .ToListAsync();

            var result = new
            {
                TotalOrders = totalOrders,
                NonDeletedOrders = nonDeletedOrders,
                TotalCustomers = totalCustomers,
                CustomersWithShop1 = customersWithShop1,
                SampleOrders = sampleOrders,
                Message = "Orders data test successful"
            };

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error testing orders: {ex.Message}");
        }
    }
}
