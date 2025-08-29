using TossErp.Accounts.Application.Common;

namespace TossErp.Accounts.Application.Queries;

/// <summary>
/// Query to get invoices with filtering and pagination
/// </summary>
public record GetInvoicesQuery(
    Guid? CustomerId = null,
    InvoiceStatus? Status = null,
    string? InvoiceNumber = null,
    DateOnly? IssueDateFrom = null,
    DateOnly? IssueDateTo = null,
    DateOnly? DueDateFrom = null,
    DateOnly? DueDateTo = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null,
    string? Currency = null,
    int PageNumber = 1,
    int PageSize = 10,
    string SortBy = "IssueDate",
    bool SortDescending = true
) : IRequest<PaginatedResult<InvoiceDto>>;

/// <summary>
/// Handler for getting invoices
/// </summary>
public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, PaginatedResult<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetInvoicesQueryHandler> _logger;

    public GetInvoicesQueryHandler(
        IInvoiceRepository invoiceRepository,
        ICustomerRepository customerRepository,
        ILogger<GetInvoicesQueryHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<PaginatedResult<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting invoices with customer: {CustomerId}, Status: {Status}", 
            request.CustomerId, request.Status);

        var filter = new InvoiceFilter
        {
            CustomerId = request.CustomerId,
            Status = request.Status,
            InvoiceNumber = request.InvoiceNumber,
            IssueDateFrom = request.IssueDateFrom,
            IssueDateTo = request.IssueDateTo,
            DueDateFrom = request.DueDateFrom,
            DueDateTo = request.DueDateTo,
            MinAmount = request.MinAmount,
            MaxAmount = request.MaxAmount,
            Currency = request.Currency
        };

        var (invoiceList, totalCount) = await _invoiceRepository.GetPagedAsync(
            filter,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDescending,
            cancellationToken);

        // Get customer information for mapping
        var customerIds = invoiceList.Select(i => i.CustomerId).Distinct().ToList();
        var customers = new Dictionary<Guid, Customer>();
        
        foreach (var customerId in customerIds)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer != null)
            {
                customers[customerId] = customer;
            }
        }

        var invoiceDtos = invoiceList.Select(invoice => InvoiceMappingHelper.MapToDto(invoice, customers.GetValueOrDefault(invoice.CustomerId))).ToList();

        return new PaginatedResult<InvoiceDto>
        {
            Items = invoiceDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    private static InvoiceDto MapToDto(Invoice invoice, Customer? customer)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            TenantId = invoice.TenantId,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerId = invoice.CustomerId,
            CustomerName = customer?.Name ?? "Unknown Customer",
            CustomerEmail = customer?.Email ?? string.Empty,
            Status = invoice.Status,
            IssueDate = DateOnly.FromDateTime(invoice.IssueDate),
            DueDate = DateOnly.FromDateTime(invoice.DueDate),
            PaidDate = invoice.PaidDate.HasValue ? DateOnly.FromDateTime(invoice.PaidDate.Value) : null,
            SubtotalAmount = invoice.SubtotalAmount.Amount,
            TaxAmount = invoice.TaxAmount.Amount,
            DiscountAmount = invoice.DiscountAmount.Amount,
            TotalAmount = invoice.TotalAmount.Amount,
            PaidAmount = invoice.PaidAmount.Amount,
            BalanceAmount = invoice.BalanceAmount.Amount,
            Currency = invoice.Currency,
            LineItems = invoice.LineItems.Select(MapLineItemToDto).ToList(),
            BillingAddress = invoice.BillingAddress != null ? MapAddressToDto(invoice.BillingAddress) : null,
            ShippingAddress = invoice.ShippingAddress != null ? MapAddressToDto(invoice.ShippingAddress) : null,
            Terms = invoice.Terms,
            Notes = invoice.Notes,
            InternalNotes = invoice.InternalNotes,
            Reference = invoice.Reference,
            PurchaseOrderNumber = invoice.PurchaseOrderNumber,
            CreatedAt = invoice.CreatedAt,
            CreatedBy = invoice.CreatedBy,
            LastModified = invoice.LastModified,
            LastModifiedBy = invoice.LastModifiedBy
        };
    }

    private static InvoiceLineItemDto MapLineItemToDto(InvoiceLineItem lineItem)
    {
        return new InvoiceLineItemDto
        {
            Id = lineItem.Id,
            Description = lineItem.Description,
            Quantity = lineItem.Quantity,
            UnitPrice = lineItem.UnitPrice.Amount,
            LineTotal = lineItem.LineTotal.Amount,
            DiscountPercentage = lineItem.DiscountPercentage,
            DiscountAmount = lineItem.DiscountAmount?.Amount,
            TaxRate = lineItem.TaxRate?.Rate,
            TaxAmount = lineItem.TaxAmount?.Amount,
            ProductCode = lineItem.ProductCode,
            Unit = lineItem.Unit
        };
    }

    private static CustomerAddressDto MapAddressToDto(CustomerAddress address)
    {
        return new CustomerAddressDto
        {
            Street = address.Street,
            Street2 = address.Street2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country
        };
    }

    private static CustomerAddressDto MapAddressToDto(Address address)
    {
        return new CustomerAddressDto
        {
            Street = address.Street,
            Street2 = null, // Address value object doesn't have Street2
            City = address.City,
            State = address.Province, // Map Province to State
            PostalCode = address.PostalCode,
            Country = address.Country,
            Type = "General", // Default type for value object addresses
            IsPrimary = false
        };
    }
}

/// <summary>
/// Query to get an invoice by ID
/// </summary>
public record GetInvoiceByIdQuery(Guid InvoiceId) : IRequest<InvoiceDto?>;

/// <summary>
/// Handler for getting an invoice by ID
/// </summary>
public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto?>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetInvoiceByIdQueryHandler> _logger;

    public GetInvoiceByIdQueryHandler(
        IInvoiceRepository invoiceRepository,
        ICustomerRepository customerRepository,
        ILogger<GetInvoiceByIdQueryHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<InvoiceDto?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting invoice by ID: {InvoiceId}", request.InvoiceId);

        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return null;
        }

        var customer = await _customerRepository.GetByIdAsync(invoice.CustomerId, cancellationToken);

        return InvoiceMappingHelper.MapToDto(invoice, customer);
    }

    private static InvoiceDto MapToDto(Invoice invoice, Customer? customer)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            TenantId = invoice.TenantId,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerId = invoice.CustomerId,
            CustomerName = customer?.Name ?? "Unknown Customer",
            CustomerEmail = customer?.Email ?? string.Empty,
            Status = invoice.Status,
            IssueDate = DateOnly.FromDateTime(invoice.IssueDate),
            DueDate = DateOnly.FromDateTime(invoice.DueDate),
            PaidDate = invoice.PaidDate.HasValue ? DateOnly.FromDateTime(invoice.PaidDate.Value) : null,
            SubtotalAmount = invoice.SubtotalAmount.Amount,
            TaxAmount = invoice.TaxAmount.Amount,
            DiscountAmount = invoice.DiscountAmount.Amount,
            TotalAmount = invoice.TotalAmount.Amount,
            PaidAmount = invoice.PaidAmount.Amount,
            BalanceAmount = invoice.BalanceAmount.Amount,
            Currency = invoice.Currency,
            LineItems = invoice.LineItems.Select(MapLineItemToDto).ToList(),
            BillingAddress = invoice.BillingAddress != null ? MapAddressToDto(invoice.BillingAddress) : null,
            ShippingAddress = invoice.ShippingAddress != null ? MapAddressToDto(invoice.ShippingAddress) : null,
            Terms = invoice.Terms,
            Notes = invoice.Notes,
            InternalNotes = invoice.InternalNotes,
            Reference = invoice.Reference,
            PurchaseOrderNumber = invoice.PurchaseOrderNumber,
            CreatedAt = invoice.CreatedAt,
            CreatedBy = invoice.CreatedBy,
            LastModified = invoice.LastModified,
            LastModifiedBy = invoice.LastModifiedBy
        };
    }

    private static InvoiceLineItemDto MapLineItemToDto(InvoiceLineItem lineItem)
    {
        return new InvoiceLineItemDto
        {
            Id = lineItem.Id,
            Description = lineItem.Description,
            Quantity = lineItem.Quantity,
            UnitPrice = lineItem.UnitPrice.Amount,
            LineTotal = lineItem.LineTotal.Amount,
            DiscountPercentage = lineItem.DiscountPercentage,
            DiscountAmount = lineItem.DiscountAmount?.Amount,
            TaxRate = lineItem.TaxRate?.Rate,
            TaxAmount = lineItem.TaxAmount?.Amount,
            ProductCode = lineItem.ProductCode,
            Unit = lineItem.Unit
        };
    }

    private static CustomerAddressDto MapAddressToDto(CustomerAddress address)
    {
        return new CustomerAddressDto
        {
            Street = address.Street,
            Street2 = address.Street2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country
        };
    }

    private static CustomerAddressDto MapAddressToDto(Address address)
    {
        return new CustomerAddressDto
        {
            Street = address.Street,
            Street2 = null, // Address value object doesn't have Street2
            City = address.City,
            State = address.Province, // Map Province to State
            PostalCode = address.PostalCode,
            Country = address.Country,
            Type = "General", // Default type for value object addresses
            IsPrimary = false
        };
    }
}

/// <summary>
/// Query to get overdue invoices
/// </summary>
public record GetOverdueInvoicesQuery(
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedResult<InvoiceDto>>;

/// <summary>
/// Handler for getting overdue invoices
/// </summary>
public class GetOverdueInvoicesQueryHandler : IRequestHandler<GetOverdueInvoicesQuery, PaginatedResult<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetOverdueInvoicesQueryHandler> _logger;

    public GetOverdueInvoicesQueryHandler(
        IInvoiceRepository invoiceRepository,
        ICustomerRepository customerRepository,
        ILogger<GetOverdueInvoicesQueryHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<PaginatedResult<InvoiceDto>> Handle(GetOverdueInvoicesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting overdue invoices");

        var allOverdueInvoices = await _invoiceRepository.GetOverdueInvoicesAsync(cancellationToken);
        var totalCount = allOverdueInvoices.Count();
        
        // Manual pagination
        var pagedInvoices = allOverdueInvoices
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Get customer information for mapping
        var customerIds = pagedInvoices.Select(i => i.CustomerId).Distinct().ToList();
        var customers = new Dictionary<Guid, Customer>();
        
        foreach (var customerId in customerIds)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer != null)
            {
                customers[customerId] = customer;
            }
        }

        var invoiceDtos = pagedInvoices.Select(invoice => InvoiceMappingHelper.MapToDto(invoice, customers.GetValueOrDefault(invoice.CustomerId))).ToList();

        return new PaginatedResult<InvoiceDto>
        {
            Items = invoiceDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    private static InvoiceDto MapToDto(Invoice invoice, Customer? customer)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            TenantId = invoice.TenantId,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerId = invoice.CustomerId,
            CustomerName = customer?.Name ?? "Unknown Customer",
            CustomerEmail = customer?.Email ?? string.Empty,
            Status = invoice.Status,
            IssueDate = DateOnly.FromDateTime(invoice.IssueDate),
            DueDate = DateOnly.FromDateTime(invoice.DueDate),
            PaidDate = invoice.PaidDate.HasValue ? DateOnly.FromDateTime(invoice.PaidDate.Value) : null,
            SubtotalAmount = invoice.SubtotalAmount.Amount,
            TaxAmount = invoice.TaxAmount.Amount,
            DiscountAmount = invoice.DiscountAmount.Amount,
            TotalAmount = invoice.TotalAmount.Amount,
            PaidAmount = invoice.PaidAmount.Amount,
            BalanceAmount = invoice.BalanceAmount.Amount,
            Currency = invoice.Currency,
            LineItems = invoice.LineItems.Select(MapLineItemToDto).ToList(),
            BillingAddress = invoice.BillingAddress != null ? MapAddressToDto(invoice.BillingAddress) : null,
            ShippingAddress = invoice.ShippingAddress != null ? MapAddressToDto(invoice.ShippingAddress) : null,
            Terms = invoice.Terms,
            Notes = invoice.Notes,
            InternalNotes = invoice.InternalNotes,
            Reference = invoice.Reference,
            PurchaseOrderNumber = invoice.PurchaseOrderNumber,
            CreatedAt = invoice.CreatedAt,
            CreatedBy = invoice.CreatedBy,
            LastModified = invoice.LastModified,
            LastModifiedBy = invoice.LastModifiedBy
        };
    }

    private static InvoiceLineItemDto MapLineItemToDto(InvoiceLineItem lineItem)
    {
        return new InvoiceLineItemDto
        {
            Id = lineItem.Id,
            Description = lineItem.Description,
            Quantity = lineItem.Quantity,
            UnitPrice = lineItem.UnitPrice.Amount,
            LineTotal = lineItem.LineTotal.Amount,
            DiscountPercentage = lineItem.DiscountPercentage,
            DiscountAmount = lineItem.DiscountAmount?.Amount,
            TaxRate = lineItem.TaxRate?.Rate,
            TaxAmount = lineItem.TaxAmount?.Amount,
            ProductCode = lineItem.ProductCode,
            Unit = lineItem.Unit
        };
    }

    private static CustomerAddressDto MapAddressToDto(CustomerAddress address)
    {
        return new CustomerAddressDto
        {
            Street = address.Street,
            Street2 = address.Street2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country
        };
    }

    private static CustomerAddressDto MapAddressToDto(Address address)
    {
        return new CustomerAddressDto
        {
            Street = address.Street,
            Street2 = null, // Address value object doesn't have Street2
            City = address.City,
            State = address.Province, // Map Province to State
            PostalCode = address.PostalCode,
            Country = address.Country,
            Type = "General", // Default type for value object addresses
            IsPrimary = false
        };
    }
}
