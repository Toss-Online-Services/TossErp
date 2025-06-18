using POS.API.Application.Models;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.AggregatesModel.OrderAggregate;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.AggregatesModel.StaffAggregate;
using POS.Domain.SeedWork;
using System.Linq.Expressions;

namespace POS.API.Application.Queries;

public class POSQueries : IPOSQueries
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Sale> _saleRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Store> _storeRepository;
    private readonly IRepository<Staff> _staffRepository;
    private readonly IRepository<ProductCategory> _categoryRepository;

    public POSQueries(
        IRepository<Product> productRepository,
        IRepository<Order> orderRepository,
        IRepository<Sale> saleRepository,
        IRepository<Customer> customerRepository,
        IRepository<Store> storeRepository,
        IRepository<Staff> staffRepository,
        IRepository<ProductCategory> categoryRepository)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _saleRepository = saleRepository;
        _customerRepository = customerRepository;
        _storeRepository = storeRepository;
        _staffRepository = staffRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductDto?> GetProductAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return null;

        var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
        
        return new ProductDto
        {
            Id = product.Id,
            StoreId = product.StoreId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            CostPrice = product.CostPrice,
            SKU = product.SKU,
            Barcode = product.Barcode,
            CategoryId = product.CategoryId,
            CategoryName = category?.Name ?? "Unknown",
            IsActive = product.IsActive,
            StockQuantity = product.StockQuantity,
            LowStockThreshold = product.LowStockThreshold,
            CreatedAt = product.CreatedAt,
            LastModifiedAt = product.LastModifiedAt
        };
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync(string? searchTerm = null, Guid? categoryId = null, bool? isActive = null, int page = 1, int pageSize = 20)
    {
        Expression<Func<Product, bool>>? filter = null;
        
        if (!string.IsNullOrEmpty(searchTerm))
        {
            filter = p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm) || p.SKU.Contains(searchTerm);
        }
        
        if (categoryId.HasValue)
        {
            var categoryFilter = (Expression<Func<Product, bool>>)(p => p.CategoryId == categoryId.Value);
            filter = filter == null ? categoryFilter : CombineFilters(filter, categoryFilter);
        }
        
        if (isActive.HasValue)
        {
            var activeFilter = (Expression<Func<Product, bool>>)(p => p.IsActive == isActive.Value);
            filter = filter == null ? activeFilter : CombineFilters(filter, activeFilter);
        }

        var products = filter != null 
            ? await _productRepository.GetAsync(filter)
            : await _productRepository.GetAllAsync();
            
        var productDtos = new List<ProductDto>();

        foreach (var product in products)
        {
            var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
            productDtos.Add(new ProductDto
            {
                Id = product.Id,
                StoreId = product.StoreId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price.Amount,
                Currency = product.Price.Currency,
                CostPrice = product.CostPrice,
                SKU = product.SKU,
                Barcode = product.Barcode,
                CategoryId = product.CategoryId,
                CategoryName = category?.Name ?? "Unknown",
                IsActive = product.IsActive,
                StockQuantity = product.StockQuantity,
                LowStockThreshold = product.LowStockThreshold,
                CreatedAt = product.CreatedAt,
                LastModifiedAt = product.LastModifiedAt
            });
        }

        return productDtos;
    }

    public async Task<OrderDto?> GetOrderAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) return null;

        var customer = await _customerRepository.GetByIdAsync(order.CustomerId);

        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            CustomerName = customer != null ? $"{customer.FirstName} {customer.LastName}" : "Unknown",
            TaxRate = order.TaxRate,
            DiscountPercentage = order.DiscountPercentage,
            TaxAmount = order.TaxAmount.Amount,
            DiscountAmount = order.DiscountAmount.Amount,
            TotalAmount = order.TotalAmount.Amount,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            CompletedAt = order.CompletedAt,
            UpdatedAt = order.UpdatedAt,
            Notes = order.Notes,
            Items = order.OrderItems.Select(item => new OrderItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductSku = item.ProductSku,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice.Amount,
                TotalPrice = item.TotalPrice.Amount,
                Notes = item.Notes
            }).ToList()
        };
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersAsync(Guid? customerId = null, string? status = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 20)
    {
        var orders = await _orderRepository.GetAllAsync();
        var orderDtos = new List<OrderDto>();

        foreach (var order in orders)
        {
            // Apply filters
            if (customerId.HasValue && order.CustomerId != customerId.Value) continue;
            if (!string.IsNullOrEmpty(status) && order.Status.ToString() != status) continue;
            if (fromDate.HasValue && order.CreatedAt < fromDate.Value) continue;
            if (toDate.HasValue && order.CreatedAt > toDate.Value) continue;

            var customer = await _customerRepository.GetByIdAsync(order.CustomerId);

            orderDtos.Add(new OrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                CustomerName = customer != null ? $"{customer.FirstName} {customer.LastName}" : "Unknown",
                TaxRate = order.TaxRate,
                DiscountPercentage = order.DiscountPercentage,
                TaxAmount = order.TaxAmount.Amount,
                DiscountAmount = order.DiscountAmount.Amount,
                TotalAmount = order.TotalAmount.Amount,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                CompletedAt = order.CompletedAt,
                UpdatedAt = order.UpdatedAt,
                Notes = order.Notes,
                Items = order.OrderItems.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductSku = item.ProductSku,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice.Amount,
                    TotalPrice = item.TotalPrice.Amount,
                    Notes = item.Notes
                }).ToList()
            });
        }

        return orderDtos;
    }

    public async Task<SaleDto?> GetSaleAsync(Guid id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);
        if (sale == null) return null;

        var customer = sale.CustomerId.HasValue ? await _customerRepository.GetByIdAsync(sale.CustomerId.Value) : null;
        var staff = sale.StaffId.HasValue ? await _staffRepository.GetByIdAsync(sale.StaffId.Value) : null;

        return new SaleDto
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            StoreId = sale.StoreId,
            CustomerId = sale.CustomerId,
            StaffId = sale.StaffId,
            Subtotal = sale.Subtotal,
            Tax = sale.Tax,
            Discount = sale.Discount,
            Total = sale.Total,
            Status = sale.Status.ToString(),
            Notes = sale.Notes,
            CreatedAt = sale.CreatedAt,
            UpdatedAt = sale.UpdatedAt,
            CompletedAt = sale.CompletedAt,
            CancelledAt = sale.CancelledAt,
            CancellationReason = sale.CancellationReason,
            Items = sale.Items.Select(item => new SaleItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                Discount = item.Discount,
                TotalPrice = item.TotalPrice,
                TaxRate = item.TaxRate,
                Status = item.Status.ToString()
            }).ToList()
        };
    }

    public async Task<IEnumerable<SaleDto>> GetSalesAsync(Guid? customerId = null, Guid? storeId = null, Guid? staffId = null, string? status = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 20)
    {
        var sales = await _saleRepository.GetAllAsync();
        var saleDtos = new List<SaleDto>();

        foreach (var sale in sales)
        {
            // Apply filters
            if (customerId.HasValue && sale.CustomerId != customerId.Value) continue;
            if (storeId.HasValue && sale.StoreId != storeId.Value) continue;
            if (staffId.HasValue && sale.StaffId != staffId.Value) continue;
            if (!string.IsNullOrEmpty(status) && sale.Status.ToString() != status) continue;
            if (fromDate.HasValue && sale.CreatedAt < fromDate.Value) continue;
            if (toDate.HasValue && sale.CreatedAt > toDate.Value) continue;

            var customer = sale.CustomerId.HasValue ? await _customerRepository.GetByIdAsync(sale.CustomerId.Value) : null;
            var staff = sale.StaffId.HasValue ? await _staffRepository.GetByIdAsync(sale.StaffId.Value) : null;

            saleDtos.Add(new SaleDto
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                StoreId = sale.StoreId,
                CustomerId = sale.CustomerId,
                StaffId = sale.StaffId,
                Subtotal = sale.Subtotal,
                Tax = sale.Tax,
                Discount = sale.Discount,
                Total = sale.Total,
                Status = sale.Status.ToString(),
                Notes = sale.Notes,
                CreatedAt = sale.CreatedAt,
                UpdatedAt = sale.UpdatedAt,
                CompletedAt = sale.CompletedAt,
                CancelledAt = sale.CancelledAt,
                CancellationReason = sale.CancellationReason,
                Items = sale.Items.Select(item => new SaleItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    Discount = item.Discount,
                    TotalPrice = item.TotalPrice,
                    TaxRate = item.TaxRate,
                    Status = item.Status.ToString()
                }).ToList()
            });
        }

        return saleDtos;
    }

    public async Task<CustomerDto?> GetCustomerAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null) return null;

        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Street = customer.Address?.Street,
            City = customer.Address?.City,
            State = customer.Address?.State,
            ZipCode = customer.Address?.ZipCode,
            Country = customer.Address?.Country,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            LastModifiedAt = customer.LastModifiedAt,
            Notes = customer.Notes
        };
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersAsync(string? searchTerm = null, bool? isActive = null, int page = 1, int pageSize = 20)
    {
        Expression<Func<Customer, bool>>? filter = null;
        
        if (!string.IsNullOrEmpty(searchTerm))
        {
            filter = c => c.FirstName.Contains(searchTerm) || c.LastName.Contains(searchTerm) || c.Email.Contains(searchTerm);
        }
        
        if (isActive.HasValue)
        {
            var activeFilter = (Expression<Func<Customer, bool>>)(c => c.IsActive == isActive.Value);
            filter = filter == null ? activeFilter : CombineFilters(filter, activeFilter);
        }

        var customers = filter != null 
            ? await _customerRepository.GetAsync(filter)
            : await _customerRepository.GetAllAsync();
        var customerDtos = new List<CustomerDto>();

        foreach (var customer in customers)
        {
            customerDtos.Add(new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Street = customer.Address?.Street,
                City = customer.Address?.City,
                State = customer.Address?.State,
                ZipCode = customer.Address?.ZipCode,
                Country = customer.Address?.Country,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                LastModifiedAt = customer.LastModifiedAt,
                Notes = customer.Notes
            });
        }

        return customerDtos;
    }

    public async Task<StoreDto?> GetStoreAsync(Guid id)
    {
        var store = await _storeRepository.GetByIdAsync(id);
        if (store == null) return null;

        return new StoreDto
        {
            Id = store.Id,
            Name = store.Name,
            Code = store.Code,
            Street = store.Address.Street,
            City = store.Address.City,
            State = store.Address.State,
            ZipCode = store.Address.ZipCode,
            Country = store.Address.Country,
            Phone = store.Phone,
            Email = store.Email,
            IsActive = store.IsActive,
            CreatedAt = store.CreatedAt,
            UpdatedAt = store.UpdatedAt,
            Website = store.Website,
            Description = store.Description,
            TaxId = store.TaxId,
            LicenseNumber = store.LicenseNumber,
            LogoUrl = store.LogoUrl,
            BannerUrl = store.BannerUrl,
            SocialMediaLinks = store.SocialMediaLinks,
            TimeZone = store.TimeZone
        };
    }

    public async Task<IEnumerable<StoreDto>> GetStoresAsync(bool? isActive = null, int page = 1, int pageSize = 20)
    {
        Expression<Func<Store, bool>>? filter = null;
        
        if (isActive.HasValue)
        {
            filter = s => s.IsActive == isActive.Value;
        }

        var stores = filter != null 
            ? await _storeRepository.GetAsync(filter)
            : await _storeRepository.GetAllAsync();
        var storeDtos = new List<StoreDto>();

        foreach (var store in stores)
        {
            storeDtos.Add(new StoreDto
            {
                Id = store.Id,
                Name = store.Name,
                Code = store.Code,
                Street = store.Address.Street,
                City = store.Address.City,
                State = store.Address.State,
                ZipCode = store.Address.ZipCode,
                Country = store.Address.Country,
                Phone = store.Phone,
                Email = store.Email,
                IsActive = store.IsActive,
                CreatedAt = store.CreatedAt,
                UpdatedAt = store.UpdatedAt,
                Website = store.Website,
                Description = store.Description,
                TaxId = store.TaxId,
                LicenseNumber = store.LicenseNumber,
                LogoUrl = store.LogoUrl,
                BannerUrl = store.BannerUrl,
                SocialMediaLinks = store.SocialMediaLinks,
                TimeZone = store.TimeZone
            });
        }

        return storeDtos;
    }

    private static Expression<Func<T, bool>> CombineFilters<T>(
        Expression<Func<T, bool>> filter1,
        Expression<Func<T, bool>> filter2)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var body1 = Expression.Invoke(filter1, parameter);
        var body2 = Expression.Invoke(filter2, parameter);
        var combinedBody = Expression.AndAlso(body1, body2);
        return Expression.Lambda<Func<T, bool>>(combinedBody, parameter);
    }
} 
