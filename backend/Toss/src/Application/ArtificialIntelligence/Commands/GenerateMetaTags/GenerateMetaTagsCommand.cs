using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Vendors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.ArtificialIntelligence.Commands.GenerateMetaTags;

public record MetaTagsResult
{
    public string MetaTitle { get; init; } = string.Empty;
    public string MetaKeywords { get; init; } = string.Empty;
    public string MetaDescription { get; init; } = string.Empty;
}

public record GenerateMetaTagsCommand : IRequest<MetaTagsResult>
{
    public string EntityType { get; init; } = string.Empty;
    public int EntityId { get; init; }
    public int LanguageId { get; init; }
}

public class GenerateMetaTagsCommandHandler : IRequestHandler<GenerateMetaTagsCommand, MetaTagsResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IArtificialIntelligenceService _aiService;

    public GenerateMetaTagsCommandHandler(
        IApplicationDbContext context,
        IArtificialIntelligenceService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<MetaTagsResult> Handle(GenerateMetaTagsCommand request, CancellationToken cancellationToken)
    {
        var (metaTitle, metaKeywords, metaDescription) = request.EntityType switch
        {
            nameof(Product) => await GenerateProductMetaTagsAsync(request.EntityId, request.LanguageId, cancellationToken),
            nameof(ProductCategory) => await GenerateCategoryMetaTagsAsync(request.EntityId, request.LanguageId, cancellationToken),
            nameof(Vendor) => await GenerateVendorMetaTagsAsync(request.EntityId, request.LanguageId, cancellationToken),
            _ => throw new Common.Exceptions.BadRequestException($"Unsupported entity type: {request.EntityType}")
        };

        return new MetaTagsResult
        {
            MetaTitle = metaTitle,
            MetaKeywords = metaKeywords,
            MetaDescription = metaDescription
        };
    }

    private async Task<(string metaTitle, string metaKeywords, string metaDescription)> GenerateProductMetaTagsAsync(
        int productId, 
        int languageId, 
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken)
            ?? throw new Common.Exceptions.NotFoundException(nameof(Product), productId.ToString());

        return await _aiService.CreateMetaTagsAsync(product, languageId);
    }

    private async Task<(string metaTitle, string metaKeywords, string metaDescription)> GenerateCategoryMetaTagsAsync(
        int categoryId, 
        int languageId, 
        CancellationToken cancellationToken)
    {
        var category = await _context.ProductCategories
            .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken)
            ?? throw new Common.Exceptions.NotFoundException(nameof(ProductCategory), categoryId.ToString());

        return await _aiService.CreateMetaTagsForLocalizedEntityAsync(category, languageId);
    }

    private async Task<(string metaTitle, string metaKeywords, string metaDescription)> GenerateVendorMetaTagsAsync(
        int vendorId, 
        int languageId, 
        CancellationToken cancellationToken)
    {
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.Id == vendorId, cancellationToken)
            ?? throw new Common.Exceptions.NotFoundException(nameof(Vendor), vendorId.ToString());

        return await _aiService.CreateMetaTagsForLocalizedEntityAsync(vendor, languageId);
    }
}

