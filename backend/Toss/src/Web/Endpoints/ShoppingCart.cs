using Microsoft.EntityFrameworkCore;
using Toss.Application.Common.Interfaces;
using Toss.Application.ShoppingCart.Commands.AddToCart;
using Toss.Application.ShoppingCart.Commands.UpdateCartItem;
using Toss.Application.ShoppingCart.Commands.Checkout;
using Toss.Application.ShoppingCart.Queries.GetCart;

namespace Toss.Web.Endpoints;

public class ShoppingCart : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {

        group.MapPost(AddToCart, "add")
            .Produces<AddToCartResult>()
            .ProducesValidationProblem()
            .WithName("AddToCart")
            .WithSummary("Add product to shopping cart")
            .WithOpenApi();

        group.MapPut(UpdateCartItem, "update")
            .Produces<UpdateCartItemResult>()
            .ProducesValidationProblem()
            .WithName("UpdateCartItem")
            .WithSummary("Update cart item quantity")
            .WithOpenApi();

        group.MapPost(GetCart, "get")
            .Produces<GetCartResult>()
            .WithName("GetCart")
            .WithSummary("Get shopping cart contents")
            .WithOpenApi();

        group.MapPost(Checkout, "checkout")
            .Produces<CheckoutResult>()
            .ProducesValidationProblem()
            .WithName("Checkout")
            .WithSummary("Complete checkout and create sale")
            .WithOpenApi();

        group.MapDelete(ClearCart, "clear")
            .Produces<bool>()
            .WithName("ClearCart")
            .WithSummary("Clear shopping cart")
            .WithOpenApi();
    }

    private async Task<AddToCartResult> AddToCart(ISender sender, AddToCartCommand command)
    {
        return await sender.Send(command);
    }

    private async Task<UpdateCartItemResult> UpdateCartItem(ISender sender, UpdateCartItemCommand command)
    {
        return await sender.Send(command);
    }

    private async Task<GetCartResult> GetCart(ISender sender, GetCartQuery query)
    {
        return await sender.Send(query);
    }

    private async Task<CheckoutResult> Checkout(ISender sender, CheckoutCommand command)
    {
        return await sender.Send(command);
    }

    private async Task<bool> ClearCart(ISender sender, IApplicationDbContext context, string sessionId, int shopId, CancellationToken cancellationToken = default)
    {
        var items = await context.ShoppingCartItems
            .Where(c => c.SessionId == sessionId && c.ShopId == shopId && c.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            item.IsActive = false;
        }

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

