using Catalog.Domain.AggregatesModel.CatalogAggregate;
using Pgvector;

namespace Catalog.Domain.Interfaces;

public interface ICatalogAI
{
    bool IsEnabled { get; }
    Task<IReadOnlyList<Vector>> GetEmbeddingsAsync(IEnumerable<CatalogItem> items);
    Task<IEnumerable<CatalogItem>> SearchProductsAsync(string query);
    Task<IEnumerable<CatalogItem>> GetSimilarProductsAsync(int productId);
} 
