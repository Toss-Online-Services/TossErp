using System.Collections.Generic;
using Toss.Application.Sales.Commands.CreateSale;
using Toss.Application.Sales.Commands.GenerateReceipt;
using Toss.Application.Sales.Commands.VoidSale;
using Toss.Application.Sales.Commands.UpdateSaleStatus;
using Toss.Application.Sales.Commands.ProcessRefund;
using Toss.Application.Sales.Commands.HoldSale;
using Toss.Application.Sales.Commands.RetrieveHeldSale;
using Toss.Application.Sales.Commands.DeleteHeldSale;
using Toss.Application.Sales.Commands.CreateInvoice;
using Toss.Application.Sales.Commands.UpdateInvoiceStatus;
using Toss.Application.Sales.Commands.PayInvoice;
using Toss.Application.Sales.Queries.GetDailySummary;
using Toss.Application.Sales.Queries.GetSaleById;
using Toss.Application.Sales.Queries.GetSales;
using Toss.Application.Sales.Queries.GetHeldSales;
using Toss.Application.Sales.Queries.GetInvoices;
using Toss.Application.Sales.Queries.GetQueueOrders;
using Toss.Application.Sales.Commands.CreateSalesDocument;
using Toss.Application.Sales.Commands.CreateDeliveryNote;
using Toss.Domain.Enums;
using Toss.Application.Sales.Queries.GetSalesDocuments;
using Toss.Application.Sales.Queries.GenerateInvoicePdf;
using Toss.Domain.Constants;

namespace Toss.Web.Endpoints;

public class Sales : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequirePosAccess);
        group.MapPost(string.Empty, CreateSale)
            .WithName("CreateSale");

        group.MapGet(string.Empty, GetSales)
            .WithName("GetSales");

        // Specific routes must come before generic {id} routes
        group.MapGet("daily-summary", GetDailySummary)
            .WithName("GetDailySummary");
        
        group.MapPost("hold", HoldSale)
            .WithName("HoldSale");
        
        group.MapGet("held", GetHeldSales)
            .WithName("GetHeldSales");
        
        // Queue-based order endpoints
        group.MapGet("queue", GetQueueOrders)
            .WithName("GetQueueOrders");

        // Generic {id} route comes after specific routes
        group.MapGet("{id}", GetSaleById)
            .WithName("GetSaleById");
        
        group.MapPost("{id}/void", VoidSale)
            .WithName("VoidSale");
        
        group.MapPost("{id}/receipt", GenerateReceipt)
            .WithName("GenerateReceipt");
        
        group.MapPost("{id}/status", UpdateSaleStatus)
            .WithName("UpdateSaleStatus");
        
        group.MapPost("{id}/refund", ProcessRefund)
            .WithName("ProcessRefund");
        
        group.MapPost("{id}/retrieve", RetrieveHeldSale)
            .WithName("RetrieveHeldSale");
        
        group.MapDelete("{id}/held", DeleteHeldSale)
            .WithName("DeleteHeldSale");

        // Invoice endpoints
        group.MapGet("invoices", GetInvoices)
            .WithName("GetInvoices");
        
        group.MapPost("invoices", CreateInvoice)
            .WithName("CreateInvoice");
        
        group.MapPost("invoices/{id}/status", UpdateInvoiceStatus)
            .WithName("UpdateInvoiceStatus");
        
        group.MapPost("invoices/{id}/pay", PayInvoice)
            .WithName("PayInvoice");
        
        group.MapGet("invoices/{id}/pdf", GetInvoicePdf)
            .WithName("GetInvoicePdf");

        // Unified sales documents endpoint (long-term API)
        group.MapPost("documents", CreateSalesDocument)
            .WithName("CreateSalesDocument");

        group.MapGet("documents", GetSalesDocuments)
            .WithName("GetSalesDocuments");

        group.MapPost("quotes", CreateQuoteDocument)
            .WithName("CreateSalesQuote")
            .RequireAuthorization(Policies.RequireOwnerOrManager);

        group.MapPost("orders", CreateSalesOrderDocument)
            .WithName("CreateSalesOrderDocument")
            .RequireAuthorization(Policies.RequireOwnerOrManager);

        group.MapPost("delivery-notes", CreateDeliveryNoteDocument)
            .WithName("CreateDeliveryNoteDocument")
            .RequireAuthorization(Policies.RequireOwnerOrManager);
    }

    public async Task<IResult> CreateSale(ISender sender, CreateSaleCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/sales/{id}", new { id });
    }

    public async Task<IResult> GetSales(
        ISender sender,
        [AsParameters] GetSalesQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetSaleById(ISender sender, int id)
    {
        var result = await sender.Send(new GetSaleByIdQuery { Id = id });
        return Results.Ok(result);
    }

    public async Task<IResult> GetDailySummary(
        ISender sender,
        [AsParameters] GetDailySummaryQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> VoidSale(ISender sender, int id, VoidSaleCommand command)
    {
        var result = await sender.Send(command with { SaleId = id });
        return result ? Results.Ok() : Results.BadRequest("Sale cannot be voided");
    }

    public async Task<IResult> GenerateReceipt(ISender sender, int id)
    {
        var result = await sender.Send(new GenerateReceiptCommand { SaleId = id });
        return Results.Ok(result);
    }

    public async Task<IResult> UpdateSaleStatus(ISender sender, int id, UpdateSaleStatusCommand command)
    {
        var result = await sender.Send(command with { SaleId = id });
        return result ? Results.Ok() : Results.BadRequest("Status update failed");
    }

    public async Task<IResult> ProcessRefund(ISender sender, int id, ProcessRefundCommand command)
    {
        var refundId = await sender.Send(command with { SaleId = id });
        return Results.Ok(new { refundId });
    }

    public async Task<IResult> HoldSale(ISender sender, HoldSaleCommand command)
    {
        var saleId = await sender.Send(command);
        return Results.Created($"/api/sales/{saleId}", new { id = saleId });
    }

    public async Task<IResult> GetHeldSales(
        ISender sender,
        [AsParameters] GetHeldSalesQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> RetrieveHeldSale(ISender sender, int id)
    {
        var result = await sender.Send(new RetrieveHeldSaleCommand { SaleId = id });
        return result ? Results.Ok() : Results.NotFound("Held sale not found");
    }

    public async Task<IResult> DeleteHeldSale(ISender sender, int id)
    {
        var result = await sender.Send(new DeleteHeldSaleCommand { SaleId = id });
        return result ? Results.Ok() : Results.NotFound("Held sale not found");
    }

    public async Task<IResult> GetInvoices(
        ISender sender,
        [AsParameters] GetInvoicesQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> CreateInvoice(ISender sender, CreateInvoiceCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/sales/invoices/{id}", new { id });
    }

    public async Task<IResult> UpdateInvoiceStatus(ISender sender, int id, UpdateInvoiceStatusCommand command)
    {
        var result = await sender.Send(command with { InvoiceId = id });
        return result ? Results.Ok() : Results.BadRequest("Invoice status update failed");
    }

    public async Task<IResult> PayInvoice(ISender sender, int id, PayInvoiceCommand command)
    {
        var result = await sender.Send(command with { InvoiceId = id });
        return Results.Ok(result);
    }

    public async Task<IResult> GetInvoicePdf(ISender sender, int id)
    {
        var result = await sender.Send(new GenerateInvoicePdfQuery { InvoiceId = id });
        return Results.Ok(result);
    }

    public async Task<IResult> CreateSalesDocument(ISender sender, CreateSalesDocumentCommand command)
    {
        // Note: validates per type; idempotent for one doc per sale/type
        var id = await sender.Send(command);
        return Results.Created($"/api/sales/documents/{id}", new { id });
    }

    public async Task<IResult> GetSalesDocuments(
        ISender sender,
        [AsParameters] GetSalesDocumentsQuery query)
    {
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public record CreateSalesWorkflowDocumentRequest
    {
        public int SaleId { get; init; }
        public DateTimeOffset? DueDate { get; init; }
        public string? Notes { get; init; }
    }

    public record CreateDeliveryNoteRequest
    {
        public int SaleId { get; init; }
        public int ShopId { get; init; }
        public string? Notes { get; init; }
        public List<DeliveryNoteLineDto> Lines { get; init; } = new();
    }

    public async Task<IResult> CreateQuoteDocument(ISender sender, CreateSalesWorkflowDocumentRequest request)
    {
        var id = await sender.Send(new CreateSalesDocumentCommand
        {
            SaleId = request.SaleId,
            DocumentType = SalesDocumentType.Quote,
            DueDate = request.DueDate,
            Notes = request.Notes
        });

        return Results.Created($"/api/sales/documents/{id}", new { id });
    }

    public async Task<IResult> CreateSalesOrderDocument(ISender sender, CreateSalesWorkflowDocumentRequest request)
    {
        var id = await sender.Send(new CreateSalesDocumentCommand
        {
            SaleId = request.SaleId,
            DocumentType = SalesDocumentType.SalesOrder,
            DueDate = request.DueDate,
            Notes = request.Notes
        });

        return Results.Created($"/api/sales/documents/{id}", new { id });
    }

    public async Task<IResult> CreateDeliveryNoteDocument(ISender sender, CreateDeliveryNoteRequest request)
    {
        var id = await sender.Send(new CreateDeliveryNoteCommand
        {
            SaleId = request.SaleId,
            ShopId = request.ShopId,
            Notes = request.Notes,
            Lines = request.Lines
        });

        return Results.Created($"/api/sales/delivery-notes/{id}", new { id });
    }
    
    public async Task<IResult> GetQueueOrders(ISender sender, int shopId)
    {
        var result = await sender.Send(new GetQueueOrdersQuery { ShopId = shopId });
        return TypedResults.Ok(result);
    }
}

