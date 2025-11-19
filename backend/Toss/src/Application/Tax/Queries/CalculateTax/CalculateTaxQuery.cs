using Toss.Application.Common.Interfaces;
using Toss.Domain.Enums;

namespace Toss.Application.Tax.Queries.CalculateTax;

public record TaxCalculationDto
{
    public decimal TaxAmount { get; init; }
    public decimal TaxRate { get; init; }
    public string TaxCategoryName { get; init; } = string.Empty;
}

public record CalculateTaxQuery : IRequest<TaxCalculationDto?>
{
    public decimal Amount { get; init; }
    public int TaxCategoryId { get; init; }
    public int? CountryId { get; init; }
    public int? StateProvinceId { get; init; }
    public string? ZipPostalCode { get; init; }
}

public class CalculateTaxQueryHandler : IRequestHandler<CalculateTaxQuery, TaxCalculationDto?>
{
    private readonly IApplicationDbContext _context;

    public CalculateTaxQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaxCalculationDto?> Handle(CalculateTaxQuery request, CancellationToken cancellationToken)
    {
        // Get tax category
        var taxCategory = await _context.TaxCategories
            .FirstOrDefaultAsync(tc => tc.Id == request.TaxCategoryId, cancellationToken);

        if (taxCategory == null)
            return null;

        // Find matching tax rate based on location
        var query = _context.TaxRates
            .Where(tr => tr.TaxCategoryId == request.TaxCategoryId);

        if (request.CountryId.HasValue)
        {
            query = query.Where(tr => tr.CountryId == request.CountryId.Value);
        }

        if (request.StateProvinceId.HasValue)
        {
            query = query.Where(tr => tr.StateProvinceId == request.StateProvinceId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.ZipPostalCode))
        {
            query = query.Where(tr => tr.Zip == request.ZipPostalCode || tr.Zip == null);
        }

        var taxRate = await query
            .OrderByDescending(tr => tr.Percentage)
            .FirstOrDefaultAsync(cancellationToken);

        if (taxRate == null)
            return new TaxCalculationDto
            {
                TaxAmount = 0,
                TaxRate = 0,
                TaxCategoryName = taxCategory.Name
            };

        var taxAmount = request.Amount * (taxRate.Percentage / 100);

        return new TaxCalculationDto
        {
            TaxAmount = taxAmount,
            TaxRate = taxRate.Percentage,
            TaxCategoryName = taxCategory.Name
        };
    }
}

