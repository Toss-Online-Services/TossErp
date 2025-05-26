using Catalog.Domain.Entities;
using Pgvector;

namespace Catalog.Infrastructure;

public interface ICatalogAI
{
    bool IsEnabled { get; }
    Task<IReadOnlyList<Vector>> GetEmbeddingsAsync(IEnumerable<Product> products);
} 
