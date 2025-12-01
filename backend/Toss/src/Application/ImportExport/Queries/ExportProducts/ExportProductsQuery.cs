using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Catalog;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.ImportExport.Queries.ExportProducts;

public record ExportProductsQuery : IRequest<byte[]>
{
}

public record ProductExportDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Category { get; init; }
    public string Unit { get; init; } = string.Empty;
    public decimal CostPrice { get; init; }
    public decimal SellingPrice { get; init; }
    public int MinQty { get; init; }
    public string? Barcode { get; init; }
    public string? Description { get; init; }
}

public class ExportProductsQueryHandler : IRequestHandler<ExportProductsQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public ExportProductsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<byte[]> Handle(ExportProductsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.BusinessId == businessId)
            .Select(p => new ProductExportDto
            {
                Code = p.SKU,
                Name = p.Name,
                Category = p.Category != null ? p.Category.Name : null,
                Unit = p.Unit ?? "pcs",
                CostPrice = p.CostPrice ?? 0,
                SellingPrice = p.BasePrice,
                MinQty = p.MinimumStockLevel,
                Barcode = p.Barcode,
                Description = p.Description
            })
            .ToListAsync(cancellationToken);

        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Encoding = Encoding.UTF8
        });

        csv.WriteRecords(products);
        await writer.FlushAsync(cancellationToken);

        return memoryStream.ToArray();
    }
}
