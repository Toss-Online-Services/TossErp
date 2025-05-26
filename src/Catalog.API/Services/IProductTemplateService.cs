using Catalog.Domain.Entities;

namespace Catalog.API.Services;

public interface IProductTemplateService
{
    Task<ProductTemplate?> GetProductTemplateByIdAsync(int productTemplateId);
    Task<IEnumerable<ProductTemplate>> GetAllProductTemplatesAsync();
    Task<IEnumerable<ProductTemplate>> GetProductTemplatesByNameAsync(string name);
    Task<ProductTemplate> InsertProductTemplateAsync(ProductTemplate productTemplate);
    Task<ProductTemplate> UpdateProductTemplateAsync(ProductTemplate productTemplate);
    Task DeleteProductTemplateAsync(ProductTemplate productTemplate);
    Task DeleteProductTemplatesAsync(IList<ProductTemplate> productTemplates);
    Task<IList<ProductTemplate>> GetAllProductTemplatesAsync(bool showHidden = false);
    Task<IList<ProductTemplate>> GetProductTemplatesByNameAsync(string name, bool showHidden = false);
    Task<IList<ProductTemplate>> GetAllProductTemplatesAsync(bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<IList<ProductTemplate>> GetProductTemplatesByNameAsync(string name, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<IList<Product>> GetProductsByTemplateIdAsync(int templateId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);
    Task<int> GetProductCountByTemplateIdAsync(int templateId, bool showHidden = false);
} 
