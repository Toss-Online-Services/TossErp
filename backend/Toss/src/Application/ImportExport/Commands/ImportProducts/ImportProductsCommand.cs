using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Inventory.Commands.CreateProduct;
using Toss.Domain.Entities.Catalog;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.ImportExport.Commands.ImportProducts;

public record ImportProductsCommand : IRequest<ImportResult>
{
    public Stream CsvStream { get; init; } = null!;
    public bool SkipHeader { get; init; } = true;
}

public record ImportResult
{
    public int TotalRows { get; init; }
    public int SuccessCount { get; init; }
    public int ErrorCount { get; init; }
    public List<ImportError> Errors { get; init; } = new();
}

public record ImportError
{
    public int RowNumber { get; init; }
    public string? Field { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class ProductCsvRecord
{
    public string? SKU { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Unit { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? BasePrice { get; set; }
    public int? MinimumStockLevel { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
}

public sealed class ProductCsvRecordMap : ClassMap<ProductCsvRecord>
{
    public ProductCsvRecordMap()
    {
        Map(m => m.SKU).Name("SKU", "Code", "Product Code");
        Map(m => m.Name).Name("Name", "Product Name");
        Map(m => m.Category).Name("Category", "Product Category");
        Map(m => m.Unit).Name("Unit", "Unit of Measure", "UOM");
        Map(m => m.CostPrice).Name("Cost Price", "Cost", "Purchase Price");
        Map(m => m.BasePrice).Name("Base Price", "Selling Price", "Price", "Sale Price");
        Map(m => m.MinimumStockLevel).Name("Min Stock", "Minimum Stock Level", "Min Qty", "Reorder Point");
        Map(m => m.Barcode).Name("Barcode", "Barcode Number");
        Map(m => m.Description).Name("Description", "Notes");
    }
}

public class ImportProductsCommandHandler : IRequestHandler<ImportProductsCommand, ImportResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly ISender _sender;

    public ImportProductsCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        ISender sender)
    {
        _context = context;
        _businessContext = businessContext;
        _sender = sender;
    }

    public async Task<ImportResult> Handle(ImportProductsCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;
        var errors = new List<ImportError>();
        int successCount = 0;
        int rowNumber = request.SkipHeader ? 1 : 0;

        using var reader = new StreamReader(request.CsvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null,
            TrimOptions = TrimOptions.Trim
        });

        csv.Context.RegisterClassMap<ProductCsvRecordMap>();

        // Get existing products for duplicate checking
        var existingProducts = await _context.Products
            .Where(p => p.BusinessId == businessId)
            .ToListAsync(cancellationToken);

        var existingSKUs = existingProducts.Select(p => p.SKU).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existingBarcodes = existingProducts
            .Where(p => !string.IsNullOrWhiteSpace(p.Barcode))
            .Select(p => p.Barcode!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        await foreach (var record in csv.GetRecordsAsync<ProductCsvRecord>(cancellationToken))
        {
            rowNumber++;

            try
            {
                if (string.IsNullOrWhiteSpace(record.Name))
                {
                    errors.Add(new ImportError
                    {
                        RowNumber = rowNumber,
                        Field = "Name",
                        Message = "Product name is required"
                    });
                    continue;
                }

                if (string.IsNullOrWhiteSpace(record.SKU))
                {
                    errors.Add(new ImportError
                    {
                        RowNumber = rowNumber,
                        Field = "SKU",
                        Message = "Product SKU is required"
                    });
                    continue;
                }

                // Check for duplicates
                if (existingSKUs.Contains(record.SKU))
                {
                    errors.Add(new ImportError
                    {
                        RowNumber = rowNumber,
                        Field = "SKU",
                        Message = $"Product with SKU '{record.SKU}' already exists"
                    });
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(record.Barcode) && existingBarcodes.Contains(record.Barcode))
                {
                    errors.Add(new ImportError
                    {
                        RowNumber = rowNumber,
                        Field = "Barcode",
                        Message = $"Product with barcode '{record.Barcode}' already exists"
                    });
                    continue;
                }

                // Get category ID if category name provided
                int? categoryId = null;
                if (!string.IsNullOrWhiteSpace(record.Category))
                {
                    var category = await _context.ProductCategories
                        .FirstOrDefaultAsync(c => c.Name == record.Category, cancellationToken);
                    categoryId = category?.Id;
                }

                // Create product
                var productId = await _sender.Send(new CreateProductCommand
                {
                    SKU = record.SKU,
                    Name = record.Name,
                    CategoryId = categoryId,
                    Unit = record.Unit ?? "pcs",
                    CostPrice = record.CostPrice,
                    BasePrice = record.BasePrice ?? 0,
                    MinimumStockLevel = record.MinimumStockLevel ?? 10,
                    Barcode = record.Barcode,
                    Description = record.Description
                }, cancellationToken);

                existingSKUs.Add(record.SKU);
                if (!string.IsNullOrWhiteSpace(record.Barcode))
                {
                    existingBarcodes.Add(record.Barcode);
                }

                successCount++;
            }
            catch (Exception ex)
            {
                errors.Add(new ImportError
                {
                    RowNumber = rowNumber,
                    Message = ex.Message
                });
            }
        }

        return new ImportResult
        {
            TotalRows = rowNumber,
            SuccessCount = successCount,
            ErrorCount = errors.Count,
            Errors = errors
        };
    }
}
