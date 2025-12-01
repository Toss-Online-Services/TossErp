using Toss.Application.ImportExport.Commands.ImportProducts;
using Toss.Application.ImportExport.Commands.ImportCustomers;
using Toss.Application.ImportExport.Commands.ImportVendors;
using Toss.Application.ImportExport.Queries.ExportProducts;
using Toss.Application.ImportExport.Queries.ExportCustomers;
using Toss.Application.ImportExport.Queries.ExportVendors;
using Toss.Domain.Constants;
using Microsoft.AspNetCore.Http;

namespace Toss.Web.Endpoints;

public class ImportExport : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(Policies.RequireOwnerOrManager);

        // Import endpoints
        group.MapPost("import/products", ImportProducts)
            .WithName("ImportProducts")
            .DisableAntiforgery(); // File uploads typically don't use antiforgery

        group.MapPost("import/customers", ImportCustomers)
            .WithName("ImportCustomers")
            .DisableAntiforgery();

        group.MapPost("import/vendors", ImportVendors)
            .WithName("ImportVendors")
            .DisableAntiforgery();

        // Export endpoints
        group.MapGet("export/products", ExportProducts)
            .WithName("ExportProducts");

        group.MapGet("export/customers", ExportCustomers)
            .WithName("ExportCustomers");

        group.MapGet("export/vendors", ExportVendors)
            .WithName("ExportVendors");
    }

    public async Task<IResult> ImportProducts(ISender sender, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Results.BadRequest(new { error = "No file uploaded" });
        }

        using var stream = file.OpenReadStream();
        var result = await sender.Send(new ImportProductsCommand
        {
            CsvStream = stream,
            SkipHeader = true
        });

        return Results.Ok(result);
    }

    public async Task<IResult> ImportCustomers(ISender sender, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Results.BadRequest(new { error = "No file uploaded" });
        }

        using var stream = file.OpenReadStream();
        var result = await sender.Send(new ImportCustomersCommand
        {
            CsvStream = stream,
            SkipHeader = true
        });

        return Results.Ok(result);
    }

    public async Task<IResult> ImportVendors(ISender sender, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Results.BadRequest(new { error = "No file uploaded" });
        }

        using var stream = file.OpenReadStream();
        var result = await sender.Send(new ImportVendorsCommand
        {
            CsvStream = stream,
            SkipHeader = true
        });

        return Results.Ok(result);
    }

    public async Task<IResult> ExportProducts(ISender sender)
    {
        var csvBytes = await sender.Send(new ExportProductsQuery());
        return Results.File(csvBytes, "text/csv", "products.csv");
    }

    public async Task<IResult> ExportCustomers(ISender sender)
    {
        var csvBytes = await sender.Send(new ExportCustomersQuery());
        return Results.File(csvBytes, "text/csv", "customers.csv");
    }

    public async Task<IResult> ExportVendors(ISender sender)
    {
        var csvBytes = await sender.Send(new ExportVendorsQuery());
        return Results.File(csvBytes, "text/csv", "vendors.csv");
    }
}
