#nullable enable
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.Common;

namespace POS.Domain.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    }
} 
