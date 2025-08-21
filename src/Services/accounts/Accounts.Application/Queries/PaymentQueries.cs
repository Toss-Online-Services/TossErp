namespace TossErp.Accounts.Application.Queries;

/// <summary>
/// Query to get payments with filtering and pagination
/// </summary>
public record GetPaymentsQuery(
    Guid? CustomerId = null,
    PaymentStatus? Status = null,
    PaymentMethod? Method = null,
    DateOnly? PaymentDateFrom = null,
    DateOnly? PaymentDateTo = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null,
    string? Currency = null,
    string? Reference = null,
    int PageNumber = 1,
    int PageSize = 10,
    string SortBy = "PaymentDate",
    bool SortDescending = true
) : IRequest<PaginatedResult<PaymentDto>>;

/// <summary>
/// Handler for getting payments
/// </summary>
public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, PaginatedResult<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetPaymentsQueryHandler> _logger;

    public GetPaymentsQueryHandler(
        IPaymentRepository paymentRepository,
        ICustomerRepository customerRepository,
        ILogger<GetPaymentsQueryHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<PaginatedResult<PaymentDto>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting payments with customer: {CustomerId}, Status: {Status}", 
            request.CustomerId, request.Status);

        var filter = new PaymentFilter
        {
            CustomerId = request.CustomerId,
            Status = request.Status,
            Method = request.Method,
            PaymentDateFrom = request.PaymentDateFrom,
            PaymentDateTo = request.PaymentDateTo,
            MinAmount = request.MinAmount,
            MaxAmount = request.MaxAmount,
            Currency = request.Currency,
            Reference = request.Reference
        };

        var payments = await _paymentRepository.GetPagedAsync(
            filter,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDescending,
            cancellationToken);

        // Get customer information for mapping
        var customerIds = payments.Items.Select(p => p.CustomerId).Distinct().ToList();
        var customers = new Dictionary<Guid, Customer>();
        
        foreach (var customerId in customerIds)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer != null)
            {
                customers[customerId] = customer;
            }
        }

        var paymentDtos = payments.Items.Select(payment => MapToDto(payment, customers.GetValueOrDefault(payment.CustomerId))).ToList();

        return new PaginatedResult<PaymentDto>
        {
            Items = paymentDtos,
            TotalCount = payments.TotalCount,
            PageNumber = payments.PageNumber,
            PageSize = payments.PageSize,
            TotalPages = payments.TotalPages
        };
    }

    private static PaymentDto MapToDto(Payment payment, Customer? customer)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            TenantId = payment.TenantId,
            CustomerId = payment.CustomerId,
            CustomerName = customer?.Name ?? "Unknown Customer",
            PaymentNumber = payment.PaymentNumber.Value,
            Amount = payment.Amount.Amount,
            Currency = payment.Currency,
            Method = payment.Method,
            Status = payment.Status,
            PaymentDate = payment.PaymentDate,
            Reference = payment.Reference,
            Notes = payment.Notes,
            ExternalTransactionId = payment.ExternalTransactionId,
            RefundAmount = payment.RefundAmount?.Amount,
            RefundReason = payment.RefundReason,
            Allocations = payment.Allocations.Select(MapAllocationToDto).ToList(),
            CreatedAt = payment.CreatedAt,
            CreatedBy = payment.CreatedBy,
            LastModified = payment.LastModified,
            LastModifiedBy = payment.LastModifiedBy
        };
    }

    private static PaymentAllocationDto MapAllocationToDto(PaymentAllocation allocation)
    {
        return new PaymentAllocationDto
        {
            InvoiceId = allocation.InvoiceId,
            Amount = allocation.Amount.Amount
        };
    }
}

/// <summary>
/// Query to get a payment by ID
/// </summary>
public record GetPaymentByIdQuery(Guid PaymentId) : IRequest<PaymentDto?>;

/// <summary>
/// Handler for getting a payment by ID
/// </summary>
public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto?>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetPaymentByIdQueryHandler> _logger;

    public GetPaymentByIdQueryHandler(
        IPaymentRepository paymentRepository,
        ICustomerRepository customerRepository,
        ILogger<GetPaymentByIdQueryHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<PaymentDto?> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting payment by ID: {PaymentId}", request.PaymentId);

        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null)
        {
            return null;
        }

        var customer = await _customerRepository.GetByIdAsync(payment.CustomerId, cancellationToken);

        return MapToDto(payment, customer);
    }

    private static PaymentDto MapToDto(Payment payment, Customer? customer)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            TenantId = payment.TenantId,
            CustomerId = payment.CustomerId,
            CustomerName = customer?.Name ?? "Unknown Customer",
            PaymentNumber = payment.PaymentNumber.Value,
            Amount = payment.Amount.Amount,
            Currency = payment.Currency,
            Method = payment.Method,
            Status = payment.Status,
            PaymentDate = payment.PaymentDate,
            Reference = payment.Reference,
            Notes = payment.Notes,
            ExternalTransactionId = payment.ExternalTransactionId,
            RefundAmount = payment.RefundAmount?.Amount,
            RefundReason = payment.RefundReason,
            Allocations = payment.Allocations.Select(MapAllocationToDto).ToList(),
            CreatedAt = payment.CreatedAt,
            CreatedBy = payment.CreatedBy,
            LastModified = payment.LastModified,
            LastModifiedBy = payment.LastModifiedBy
        };
    }

    private static PaymentAllocationDto MapAllocationToDto(PaymentAllocation allocation)
    {
        return new PaymentAllocationDto
        {
            InvoiceId = allocation.InvoiceId,
            Amount = allocation.Amount.Amount
        };
    }
}

/// <summary>
/// Query to get unallocated payments
/// </summary>
public record GetUnallocatedPaymentsQuery(
    Guid? CustomerId = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedResult<PaymentDto>>;

/// <summary>
/// Handler for getting unallocated payments
/// </summary>
public class GetUnallocatedPaymentsQueryHandler : IRequestHandler<GetUnallocatedPaymentsQuery, PaginatedResult<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetUnallocatedPaymentsQueryHandler> _logger;

    public GetUnallocatedPaymentsQueryHandler(
        IPaymentRepository paymentRepository,
        ICustomerRepository customerRepository,
        ILogger<GetUnallocatedPaymentsQueryHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<PaginatedResult<PaymentDto>> Handle(GetUnallocatedPaymentsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting unallocated payments for customer: {CustomerId}", request.CustomerId);

        var unallocatedPayments = await _paymentRepository.GetUnallocatedPaymentsAsync(
            request.CustomerId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        // Get customer information for mapping
        var customerIds = unallocatedPayments.Items.Select(p => p.CustomerId).Distinct().ToList();
        var customers = new Dictionary<Guid, Customer>();
        
        foreach (var customerId in customerIds)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer != null)
            {
                customers[customerId] = customer;
            }
        }

        var paymentDtos = unallocatedPayments.Items.Select(payment => MapToDto(payment, customers.GetValueOrDefault(payment.CustomerId))).ToList();

        return new PaginatedResult<PaymentDto>
        {
            Items = paymentDtos,
            TotalCount = unallocatedPayments.TotalCount,
            PageNumber = unallocatedPayments.PageNumber,
            PageSize = unallocatedPayments.PageSize,
            TotalPages = unallocatedPayments.TotalPages
        };
    }

    private static PaymentDto MapToDto(Payment payment, Customer? customer)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            TenantId = payment.TenantId,
            CustomerId = payment.CustomerId,
            CustomerName = customer?.Name ?? "Unknown Customer",
            PaymentNumber = payment.PaymentNumber.Value,
            Amount = payment.Amount.Amount,
            Currency = payment.Currency,
            Method = payment.Method,
            Status = payment.Status,
            PaymentDate = payment.PaymentDate,
            Reference = payment.Reference,
            Notes = payment.Notes,
            ExternalTransactionId = payment.ExternalTransactionId,
            RefundAmount = payment.RefundAmount?.Amount,
            RefundReason = payment.RefundReason,
            Allocations = payment.Allocations.Select(MapAllocationToDto).ToList(),
            CreatedAt = payment.CreatedAt,
            CreatedBy = payment.CreatedBy,
            LastModified = payment.LastModified,
            LastModifiedBy = payment.LastModifiedBy
        };
    }

    private static PaymentAllocationDto MapAllocationToDto(PaymentAllocation allocation)
    {
        return new PaymentAllocationDto
        {
            InvoiceId = allocation.InvoiceId,
            Amount = allocation.Amount.Amount
        };
    }
}
