using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.ProductAggregate;

public interface IProductCategoryRepository : IRepository<ProductCategory>
{
    Task<ProductCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductCategory>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductCategory>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductCategory>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
} 
