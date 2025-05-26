#pragma warning disable SKEXP0011
using Catalog.Domain.AggregatesModel.CatalogAggregate;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Catalog.Infrastructure.Services;

public class CatalogAI : ICatalogAI
{
    private readonly CatalogContext _context;
    private readonly OpenAITextEmbeddingGenerationService _embeddingService;

    public CatalogAI(CatalogContext context, string openAiApiKey)
    {
        _context = context;
        _embeddingService = new OpenAITextEmbeddingGenerationService("text-embedding-3-small", openAiApiKey);
    }

    public bool IsEnabled => true;

    public async Task<IReadOnlyList<Vector>> GetEmbeddingsAsync(IEnumerable<CatalogItem> items)
    {
        var embeddings = new List<Vector>();
        foreach (var item in items)
        {
            var text = $"{item.Name} {item.Description}";
            var embedding = await _embeddingService.GenerateEmbeddingAsync(text);
            embeddings.Add(new Vector(embedding));
        }
        return embeddings;
    }

    public async Task<IEnumerable<CatalogItem>> SearchProductsAsync(string query)
    {
        var embedding = await _embeddingService.GenerateEmbeddingAsync(query);
        var vector = new Vector(embedding);

        return await _context.CatalogItems
            .Where(p => p.Embedding != null)
            .OrderBy(p => p.Embedding!.CosineDistance(vector))
            .Take(5)
            .Include(p => p.CatalogBrand)
            .Include(p => p.CatalogType)
            .ToListAsync();
    }

    public async Task<IEnumerable<CatalogItem>> GetSimilarProductsAsync(int productId)
    {
        var product = await _context.CatalogItems.FindAsync(productId);
        if (product?.Embedding == null)
            return Enumerable.Empty<CatalogItem>();

        return await _context.CatalogItems
            .Where(p => p.Id != productId && p.Embedding != null)
            .OrderBy(p => p.Embedding!.CosineDistance(product.Embedding))
            .Take(5)
            .Include(p => p.CatalogBrand)
            .Include(p => p.CatalogType)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Vector>> GetEmbeddingsAsync(IEnumerable<Product> products)
    {
        var embeddings = new List<Vector>();
        foreach (var product in products)
        {
            var embedding = await GenerateEmbeddingAsync(product.Name);
            embeddings.Add(embedding);
        }
        return embeddings.AsReadOnly();
    }

    private async Task<Vector> GenerateEmbeddingAsync(string text)
    {
        var embeddings = await _embeddingService.GenerateEmbeddingsAsync(new[] { text }, default);
        var embedding = embeddings.First();
        return new Vector(embedding.ToArray());
    }

    [SuppressMessage("Usage", "SKEXP0011:Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.", Justification = "Using for evaluation")]
    public static async Task<IReadOnlyList<Vector>> GetEmbeddingsAsync(IEnumerable<string> texts, string openAiApiKey)
    {
        var embeddingService = new OpenAITextEmbeddingGenerationService("text-embedding-3-small", openAiApiKey);
        var results = await embeddingService.GenerateEmbeddingsAsync(texts.ToList(), default);
        var embeddings = results.Select(e => new Vector(e.ToArray())).ToList();
        return embeddings.AsReadOnly();
    }
} 
