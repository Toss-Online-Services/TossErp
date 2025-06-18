using POS.API.Application.Models;

namespace POS.API.Application.Queries;

public interface IPOSQueries
{
    Task<ProductDto?> GetProductAsync(Guid id);
    Task<IEnumerable<ProductDto>> GetProductsAsync(string? searchTerm = null, Guid? categoryId = null, bool? isActive = null, int page = 1, int pageSize = 20);
    
    Task<OrderDto?> GetOrderAsync(Guid id);
    Task<IEnumerable<OrderDto>> GetOrdersAsync(Guid? customerId = null, string? status = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 20);
    
    Task<SaleDto?> GetSaleAsync(Guid id);
    Task<IEnumerable<SaleDto>> GetSalesAsync(Guid? customerId = null, Guid? storeId = null, Guid? staffId = null, string? status = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 20);
    
    Task<CustomerDto?> GetCustomerAsync(Guid id);
    Task<IEnumerable<CustomerDto>> GetCustomersAsync(string? searchTerm = null, bool? isActive = null, int page = 1, int pageSize = 20);
    
    Task<StoreDto?> GetStoreAsync(Guid id);
    Task<IEnumerable<StoreDto>> GetStoresAsync(bool? isActive = null, int page = 1, int pageSize = 20);
} 
