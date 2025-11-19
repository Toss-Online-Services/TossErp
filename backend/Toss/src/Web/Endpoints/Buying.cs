using Toss.Application.Buying.Commands.ApprovePurchaseOrder;
using Toss.Application.Buying.Commands.CreatePurchaseOrder;
using Toss.Application.Buying.Commands.UpdatePurchaseOrderStatus;
using Toss.Application.Buying.Commands.ReceiveGoods;
using Toss.Application.Buying.Queries.GetPurchaseOrderById;
using Toss.Application.Buying.Queries.GetPurchaseOrders;
using Toss.Application.Buying.Queries.GetVendorInvoices;
using Toss.Application.Buying.Commands.CreateVendorInvoice;
using Toss.Application.Buying.Commands.UpdateVendorInvoiceStatus;

namespace Toss.Web.Endpoints;

public class Buying : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapGet("purchase-orders", GetPurchaseOrders);
        group.MapPost("purchase-orders", CreatePurchaseOrder);
        group.MapGet("purchase-orders/{id}", GetPurchaseOrderById);
        group.MapPost("purchase-orders/{id}/approve", ApprovePurchaseOrder);
        group.MapPost("purchase-orders/{id}/status", UpdatePurchaseOrderStatus);
        group.MapPost("purchase-orders/{id}/receive", ReceiveGoods);

        // Vendor Invoices
        group.MapGet("invoices", GetVendorInvoices)
            .WithName("GetVendorInvoices");
        group.MapPost("invoices", CreateVendorInvoice)
            .WithName("CreateVendorInvoice");
        group.MapPost("invoices/{id}/status", UpdateVendorInvoiceStatus)
            .WithName("UpdateVendorInvoiceStatus");
    }

    public async Task<IResult> GetPurchaseOrders(ISender sender, int? shopId, string? status, int? skip, int? take)
    {
        var query = new GetPurchaseOrdersQuery
        {
            ShopId = shopId,
            Status = status,
            Skip = skip ?? 0,
            Take = take ?? 50
        };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> CreatePurchaseOrder(ISender sender, CreatePurchaseOrderCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/buying/purchase-orders/{id}", new { id });
    }

    public async Task<IResult> GetPurchaseOrderById(ISender sender, int id)
    {
        var result = await sender.Send(new GetPurchaseOrderByIdQuery { Id = id });
        return Results.Ok(result);
    }

    public async Task<IResult> ApprovePurchaseOrder(ISender sender, int id, ApprovePurchaseOrderCommand command)
    {
        var result = await sender.Send(command with { PurchaseOrderId = id });
        return result ? Results.Ok() : Results.BadRequest("PO cannot be approved");
    }

    public async Task<IResult> UpdatePurchaseOrderStatus(ISender sender, int id, UpdatePurchaseOrderStatusCommand command)
    {
        var result = await sender.Send(command with { PurchaseOrderId = id });
        return result ? Results.Ok() : Results.BadRequest("Status update failed");
    }

    public async Task<IResult> ReceiveGoods(ISender sender, int id, ReceiveGoodsCommand command)
    {
        var result = await sender.Send(command with { PurchaseOrderId = id });
        return result ? Results.Ok() : Results.BadRequest("Goods receipt failed");
    }

    public async Task<IResult> GetVendorInvoices(ISender sender, int? shopId, int? vendorId, string? status, int? pageNumber, int? pageSize)
    {
        var query = new GetVendorInvoicesQuery
        {
            ShopId = shopId,
            VendorId = vendorId,
            Status = status,
            PageNumber = pageNumber ?? 1,
            PageSize = pageSize ?? 50
        };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> CreateVendorInvoice(ISender sender, CreateVendorInvoiceCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/buying/invoices/{id}", new { id });
    }

    public async Task<IResult> UpdateVendorInvoiceStatus(ISender sender, int id, UpdateVendorInvoiceStatusCommand command)
    {
        var ok = await sender.Send(command with { Id = id });
        return ok ? Results.Ok() : Results.BadRequest("Status update failed");
    }
}

