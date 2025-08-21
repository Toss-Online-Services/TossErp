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

        var invoices = await _invoiceRepository.GetPagedAsync(
            filter,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDescending,
            cancellationToken);

        // Get customer information for mapping
        var customerIds = invoices.Items.Select(i => i.CustomerId).Distinct().ToList();
        var customers = new Dictionary<Guid, Customer>();
        
        foreach (var customerId in customerIds)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer != null)
            {
                customers[customerId] = customer;
            }
        }

        var invoiceDtos = invoices.Items.Select(invoice => MapToDto(invoice, customers.GetValueOrDefault(invoice.CustomerId))).ToList();

        return new PaginatedResult<InvoiceDto>
        {
            Items = invoiceDtos,
            TotalCount = invoices.TotalCount,
            PageNumber = invoices.PageNumber,
            PageSize = invoices.PageSize,
            TotalPages = invoices.TotalPages
        };
    }

    private static InvoiceDto MapToDto(Invoice invoice, Customer? customer)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            TenantId = invoice.TenantId,
            InvoiceNumber = invoice.InvoiceNumber.Value,
            CustomerId = invoice.CustomerId,
            CustomerName = customer?.Name ?? "Unknown Customer",
            CustomerEmail = customer?.Email ?? string.Empty,
            Status = invoice.Status,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            PaidDate = invoice.PaidDate,
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
            TaxRate = lineItem.TaxRate,
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

        return MapToDto(invoice, customer);
    }

    private static InvoiceDto MapToDto(Invoice invoice, Customer? customer)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            TenantId = invoice.TenantId,
            InvoiceNumber = invoice.InvoiceNumber.Value,
            CustomerId = invoice.CustomerId,
            CustomerName = customer?.Name ?? "Unknown Customer",
            CustomerEmail = customer?.Email ?? string.Empty,
            Status = invoice.Status,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            PaidDate = invoice.PaidDate,
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
            TaxRate = lineItem.TaxRate,
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

        var overdueInvoices = await _invoiceRepository.GetOverdueInvoicesAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        // Get customer information for mapping
        var customerIds = overdueInvoices.Items.Select(i => i.CustomerId).Distinct().ToList();
        var customers = new Dictionary<Guid, Customer>();
        
        foreach (var customerId in customerIds)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer != null)
            {
                customers[customerId] = customer;
            }
        }

        var invoiceDtos = overdueInvoices.Items.Select(invoice => MapToDto(invoice, customers.GetValueOrDefault(invoice.CustomerId))).ToList();

        return new PaginatedResult<InvoiceDto>
        {
            Items = invoiceDtos,
            TotalCount = overdueInvoices.TotalCount,
            PageNumber = overdueInvoices.PageNumber,
            PageSize = overdueInvoices.PageSize,
            TotalPages = overdueInvoices.TotalPages
        };
    }

    private static InvoiceDto MapToDto(Invoice invoice, Customer? customer)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            TenantId = invoice.TenantId,
            InvoiceNumber = invoice.InvoiceNumber.Value,
            CustomerId = invoice.CustomerId,
            CustomerName = customer?.Name ?? "Unknown Customer",
            CustomerEmail = customer?.Email ?? string.Empty,
            Status = invoice.Status,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            PaidDate = invoice.PaidDate,
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
            TaxRate = lineItem.TaxRate,
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
}
