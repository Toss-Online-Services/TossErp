#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TossErp.POS.Domain.AggregatesModel.ProductAggregate;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    }
} 
