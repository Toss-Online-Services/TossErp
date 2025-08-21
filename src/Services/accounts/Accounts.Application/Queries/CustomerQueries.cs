namespace TossErp.Accounts.Application.Queries;

/// <summary>
/// Query to get customers with filtering and pagination
/// </summary>
public record GetCustomersQuery(
    string? SearchTerm = null,
    CustomerStatus? Status = null,
    string? Country = null,
    DateTime? CreatedAfter = null,
    DateTime? CreatedBefore = null,
    int PageNumber = 1,
    int PageSize = 10,
    string SortBy = "Name",
    bool SortDescending = false
) : IRequest<PaginatedResult<CustomerDto>>;

/// <summary>
/// Handler for getting customers
/// </summary>
public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PaginatedResult<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetCustomersQueryHandler> _logger;

    public GetCustomersQueryHandler(
        ICustomerRepository customerRepository,
        ILogger<GetCustomersQueryHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<PaginatedResult<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting customers with search term: {SearchTerm}, Status: {Status}", 
            request.SearchTerm, request.Status);

        var filter = new CustomerFilter
        {
            SearchTerm = request.SearchTerm,
            Status = request.Status,
            Country = request.Country,
            CreatedAfter = request.CreatedAfter,
            CreatedBefore = request.CreatedBefore
        };

        var customers = await _customerRepository.GetPagedAsync(
            filter,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDescending,
            cancellationToken);

        var customerDtos = customers.Items.Select(MapToDto).ToList();

        return new PaginatedResult<CustomerDto>
        {
            Items = customerDtos,
            TotalCount = customers.TotalCount,
            PageNumber = customers.PageNumber,
            PageSize = customers.PageSize,
            TotalPages = customers.TotalPages
        };
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            TenantId = customer.TenantId,
            Name = customer.Name,
            Email = customer.Email ?? string.Empty,
            Phone = customer.Phone,
            Status = customer.Status,
            CustomerType = customer.CustomerType,
            TaxNumber = customer.TaxNumber,
            Website = customer.Website,
            PreferredCurrency = customer.PreferredCurrency,
            CreditLimit = customer.CreditLimit?.Amount,
            PaymentTerms = customer.PaymentTerms,
            Notes = customer.Notes,
            Addresses = customer.Addresses.Select(MapAddressToDto).ToList(),
            Contacts = customer.Contacts.Select(MapContactToDto).ToList(),
            CreatedAt = customer.CreatedAt,
            CreatedBy = customer.CreatedBy,
            LastModified = customer.LastModified,
            LastModifiedBy = customer.LastModifiedBy
        };
    }

    private static CustomerAddressDto MapAddressToDto(CustomerAddress address)
    {
        return new CustomerAddressDto
        {
            Type = address.Type,
            Street = address.Street,
            Street2 = address.Street2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country,
            IsPrimary = address.IsPrimary
        };
    }

    private static CustomerContactDto MapContactToDto(CustomerContact contact)
    {
        return new CustomerContactDto
        {
            Name = contact.Name,
            Title = contact.Title,
            Email = contact.Email,
            Phone = contact.Phone,
            IsPrimary = contact.IsPrimary
        };
    }
}

/// <summary>
/// Query to get a customer by ID
/// </summary>
public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<CustomerDto?>;

/// <summary>
/// Handler for getting a customer by ID
/// </summary>
public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetCustomerByIdQueryHandler> _logger;

    public GetCustomerByIdQueryHandler(
        ICustomerRepository customerRepository,
        ILogger<GetCustomerByIdQueryHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting customer by ID: {CustomerId}", request.CustomerId);

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            return null;
        }

        return MapToDto(customer);
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            TenantId = customer.TenantId,
            Name = customer.Name,
            Email = customer.Email ?? string.Empty,
            Phone = customer.Phone,
            Status = customer.Status,
            CustomerType = customer.CustomerType,
            TaxNumber = customer.TaxNumber,
            Website = customer.Website,
            PreferredCurrency = customer.PreferredCurrency,
            CreditLimit = customer.CreditLimit?.Amount,
            PaymentTerms = customer.PaymentTerms,
            Notes = customer.Notes,
            Addresses = customer.Addresses.Select(MapAddressToDto).ToList(),
            Contacts = customer.Contacts.Select(MapContactToDto).ToList(),
            CreatedAt = customer.CreatedAt,
            CreatedBy = customer.CreatedBy,
            LastModified = customer.LastModified,
            LastModifiedBy = customer.LastModifiedBy
        };
    }

    private static CustomerAddressDto MapAddressToDto(CustomerAddress address)
    {
        return new CustomerAddressDto
        {
            Type = address.Type,
            Street = address.Street,
            Street2 = address.Street2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country,
            IsPrimary = address.IsPrimary
        };
    }

    private static CustomerContactDto MapContactToDto(CustomerContact contact)
    {
        return new CustomerContactDto
        {
            Name = contact.Name,
            Title = contact.Title,
            Email = contact.Email,
            Phone = contact.Phone,
            IsPrimary = contact.IsPrimary
        };
    }
}

/// <summary>
/// Query to get customer summary statistics
/// </summary>
public record GetCustomerSummaryQuery() : IRequest<CustomerSummaryDto>;

/// <summary>
/// Handler for getting customer summary
/// </summary>
public class GetCustomerSummaryQueryHandler : IRequestHandler<GetCustomerSummaryQuery, CustomerSummaryDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetCustomerSummaryQueryHandler> _logger;

    public GetCustomerSummaryQueryHandler(
        ICustomerRepository customerRepository,
        ILogger<GetCustomerSummaryQueryHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<CustomerSummaryDto> Handle(GetCustomerSummaryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting customer summary statistics");

        var summary = await _customerRepository.GetSummaryAsync(cancellationToken);

        return new CustomerSummaryDto
        {
            TotalCustomers = summary.TotalCustomers,
            ActiveCustomers = summary.ActiveCustomers,
            NewCustomersThisMonth = summary.NewCustomersThisMonth,
            TopCountries = summary.TopCountries.Select(c => new CountryStatisticDto
            {
                Country = c.Country,
                CustomerCount = c.CustomerCount
            }).ToList()
        };
    }
}
