using Toss.Application.Registration.Commands.RegisterStoreOwner;
using Toss.Application.Registration.Commands.RegisterVendor;
using Toss.Application.Registration.Commands.RegisterDriver;
using Toss.Infrastructure.Identity;

namespace Toss.Web.Endpoints;

public class Registration : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost("store-owner", RegisterStoreOwner)
            .WithName("RegisterStoreOwner")
            .AllowAnonymous();

        group.MapPost("vendor", RegisterVendor)
            .WithName("RegisterVendor")
            .AllowAnonymous();

        group.MapPost("driver", RegisterDriver)
            .WithName("RegisterDriver")
            .AllowAnonymous();
    }

    /// <summary>
    /// Register a new store owner with their shop
    /// </summary>
    public async Task<IResult> RegisterStoreOwner(ISender sender, RegisterStoreOwnerCommand command)
    {
        var result = await sender.Send(command);
        return Results.Ok(result);
    }

    /// <summary>
    /// Register a new vendor/supplier
    /// </summary>
    public async Task<IResult> RegisterVendor(ISender sender, RegisterVendorCommand command)
    {
        var result = await sender.Send(command);
        return Results.Ok(result);
    }

    /// <summary>
    /// Register a new delivery driver
    /// </summary>
    public async Task<IResult> RegisterDriver(ISender sender, RegisterDriverCommand command)
    {
        var result = await sender.Send(command);
        return Results.Ok(result);
    }
}

