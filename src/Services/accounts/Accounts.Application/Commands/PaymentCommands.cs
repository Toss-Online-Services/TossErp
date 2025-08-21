using FluentValidation;

namespace TossErp.Accounts.Application.Commands;

/// <summary>
/// Command to process a payment
/// </summary>
public record ProcessPaymentCommand(
    Guid CustomerId,
    decimal Amount,
    string Currency,
    PaymentMethod Method,
    string? Reference,
    string? Notes,
    List<PaymentAllocationDto>? Allocations,
    DateOnly PaymentDate,
    string? ExternalTransactionId
) : IRequest<PaymentDto>;

/// <summary>
/// DTO for payment allocation
/// </summary>
public class PaymentAllocationDto
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
}

/// <summary>
/// Handler for processing payments
/// </summary>
public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly IPaymentProcessingService _paymentProcessingService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;

    public ProcessPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IInvoiceRepository invoiceRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        IPaymentProcessingService paymentProcessingService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ILogger<ProcessPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _paymentProcessingService = paymentProcessingService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<PaymentDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing payment for customer: {CustomerId}, Amount: {Amount} {Currency}", 
            request.CustomerId, request.Amount, request.Currency);

        var tenantId = _currentUserService.TenantId ?? throw new InvalidOperationException("Tenant ID is required");
        var currentUserId = _currentUserService.UserId ?? "system";

        // Validate customer
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {request.CustomerId} not found");
        }

        // Validate allocations if provided
        List<Invoice>? invoicesToAllocate = null;
        if (request.Allocations?.Any() == true)
        {
            invoicesToAllocate = new List<Invoice>();
            var totalAllocationAmount = request.Allocations.Sum(a => a.Amount);
            
            if (totalAllocationAmount > request.Amount)
            {
                throw new InvalidOperationException("Total allocation amount cannot exceed payment amount");
            }

            foreach (var allocation in request.Allocations)
            {
                var invoice = await _invoiceRepository.GetByIdAsync(allocation.InvoiceId, cancellationToken);
                if (invoice == null)
                {
                    throw new InvalidOperationException($"Invoice with ID {allocation.InvoiceId} not found");
                }

                if (invoice.CustomerId != request.CustomerId)
                {
                    throw new InvalidOperationException($"Invoice {allocation.InvoiceId} does not belong to customer {request.CustomerId}");
                }

                if (invoice.BalanceAmount.Amount < allocation.Amount)
                {
                    throw new InvalidOperationException($"Allocation amount {allocation.Amount} exceeds invoice balance {invoice.BalanceAmount.Amount}");
                }

                invoicesToAllocate.Add(invoice);
            }
        }

        // Process payment through payment service if required
        PaymentProcessingResult? processingResult = null;
        if (request.Method == PaymentMethod.CreditCard || request.Method == PaymentMethod.BankTransfer)
        {
            var processingRequest = new PaymentProcessingRequest
            {
                TenantId = tenantId,
                CustomerId = request.CustomerId,
                Amount = request.Amount,
                Currency = request.Currency,
                PaymentMethod = request.Method,
                Reference = request.Reference,
                ExternalTransactionId = request.ExternalTransactionId
            };

            processingResult = await _paymentProcessingService.ProcessPaymentAsync(processingRequest, cancellationToken);
            
            if (!processingResult.IsSuccessful)
            {
                throw new InvalidOperationException($"Payment processing failed: {processingResult.ErrorMessage}");
            }
        }

        // Create payment allocations
        var allocations = new List<PaymentAllocation>();
        if (invoicesToAllocate != null && request.Allocations != null)
        {
            for (int i = 0; i < invoicesToAllocate.Count; i++)
            {
                var allocation = PaymentAllocation.Create(
                    invoiceId: invoicesToAllocate[i].Id,
                    amount: request.Allocations[i].Amount);
                allocations.Add(allocation);
            }
        }

        // Create payment
        var payment = Payment.Create(
            tenantId: tenantId,
            customerId: request.CustomerId,
            amount: request.Amount,
            currency: request.Currency,
            method: request.Method,
            reference: request.Reference,
            notes: request.Notes,
            paymentDate: request.PaymentDate,
            externalTransactionId: processingResult?.TransactionId ?? request.ExternalTransactionId,
            createdBy: currentUserId);

        // Add allocations
        foreach (var allocation in allocations)
        {
            payment.AddAllocation(allocation, currentUserId);
        }

        // Save payment
        await _paymentRepository.AddAsync(payment, cancellationToken);

        // Update invoice statuses if allocated
        if (invoicesToAllocate != null && request.Allocations != null)
        {
            for (int i = 0; i < invoicesToAllocate.Count; i++)
            {
                var invoice = invoicesToAllocate[i];
                var allocation = request.Allocations[i];
                
                invoice.ApplyPayment(allocation.Amount, payment.Id, currentUserId);
                await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send payment confirmation
        if (!string.IsNullOrEmpty(customer.Email))
        {
            await _notificationService.SendPaymentNotificationAsync(
                payment.Id, customer.Email, NotificationType.PaymentReceived, cancellationToken);
        }

        // Publish domain events
        await _domainEventService.PublishAsync(payment.DomainEvents, cancellationToken);
        payment.ClearDomainEvents();

        _logger.LogInformation("Successfully processed payment {PaymentId}", payment.Id);

        return MapToDto(payment, customer);
    }

    private static PaymentDto MapToDto(Payment payment, Customer customer)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            TenantId = payment.TenantId,
            CustomerId = payment.CustomerId,
            CustomerName = customer.Name,
            PaymentNumber = payment.PaymentNumber.Value,
            Amount = payment.Amount.Amount,
            Currency = payment.Currency,
            Method = payment.Method,
            Status = payment.Status,
            PaymentDate = payment.PaymentDate,
            Reference = payment.Reference,
            Notes = payment.Notes,
            ExternalTransactionId = payment.ExternalTransactionId,
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
/// Validator for ProcessPaymentCommand
/// </summary>
public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Payment amount must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be a 3-letter code");

        RuleFor(x => x.Method)
            .IsInEnum()
            .WithMessage("Valid payment method is required");

        RuleFor(x => x.PaymentDate)
            .NotEmpty()
            .WithMessage("Payment date is required")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Payment date cannot be in the future");

        RuleForEach(x => x.Allocations)
            .SetValidator(new PaymentAllocationDtoValidator())
            .When(x => x.Allocations != null);
    }
}

/// <summary>
/// Validator for PaymentAllocationDto
/// </summary>
public class PaymentAllocationDtoValidator : AbstractValidator<PaymentAllocationDto>
{
    public PaymentAllocationDtoValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty()
            .WithMessage("Invoice ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Allocation amount must be greater than 0");
    }
}

/// <summary>
/// Command to refund a payment
/// </summary>
public record RefundPaymentCommand(
    Guid PaymentId,
    decimal RefundAmount,
    string Reason,
    bool AdjustInvoices = true
) : IRequest<PaymentDto>;

/// <summary>
/// Handler for refunding payments
/// </summary>
public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly IPaymentProcessingService _paymentProcessingService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<RefundPaymentCommandHandler> _logger;

    public RefundPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IInvoiceRepository invoiceRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        IPaymentProcessingService paymentProcessingService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ILogger<RefundPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _paymentProcessingService = paymentProcessingService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<PaymentDto> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing refund for payment: {PaymentId}, Amount: {RefundAmount}", 
            request.PaymentId, request.RefundAmount);

        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null)
        {
            throw new InvalidOperationException($"Payment with ID {request.PaymentId} not found");
        }

        var customer = await _customerRepository.GetByIdAsync(payment.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {payment.CustomerId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Process refund through payment service if required
        PaymentProcessingResult? refundResult = null;
        if (payment.Method == PaymentMethod.CreditCard || payment.Method == PaymentMethod.BankTransfer)
        {
            var refundRequest = new PaymentProcessingRequest
            {
                TenantId = payment.TenantId,
                CustomerId = payment.CustomerId,
                Amount = request.RefundAmount,
                Currency = payment.Currency,
                PaymentMethod = payment.Method,
                Reference = payment.Reference,
                ExternalTransactionId = payment.ExternalTransactionId
            };

            refundResult = await _paymentProcessingService.ProcessRefundAsync(refundRequest, cancellationToken);
            
            if (!refundResult.IsSuccessful)
            {
                throw new InvalidOperationException($"Refund processing failed: {refundResult.ErrorMessage}");
            }
        }

        // Apply refund
        payment.Refund(request.RefundAmount, request.Reason, currentUserId);

        // Adjust allocated invoices if requested
        if (request.AdjustInvoices)
        {
            var invoiceIds = payment.Allocations.Select(a => a.InvoiceId).Distinct();
            foreach (var invoiceId in invoiceIds)
            {
                var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
                if (invoice != null)
                {
                    var allocationAmount = payment.Allocations
                        .Where(a => a.InvoiceId == invoiceId)
                        .Sum(a => a.Amount.Amount);
                    
                    var refundProportion = request.RefundAmount / payment.Amount.Amount;
                    var invoiceRefundAmount = allocationAmount * refundProportion;
                    
                    invoice.ApplyRefund(invoiceRefundAmount, payment.Id, currentUserId);
                    await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
                }
            }
        }

        // Save changes
        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send refund notification
        if (!string.IsNullOrEmpty(customer.Email))
        {
            await _notificationService.SendPaymentNotificationAsync(
                payment.Id, customer.Email, NotificationType.PaymentRefunded, cancellationToken);
        }

        // Publish domain events
        await _domainEventService.PublishAsync(payment.DomainEvents, cancellationToken);
        payment.ClearDomainEvents();

        _logger.LogInformation("Successfully processed refund for payment {PaymentId}", request.PaymentId);

        return MapToDto(payment, customer);
    }

    private static PaymentDto MapToDto(Payment payment, Customer customer)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            TenantId = payment.TenantId,
            CustomerId = payment.CustomerId,
            CustomerName = customer.Name,
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
/// Validator for RefundPaymentCommand
/// </summary>
public class RefundPaymentCommandValidator : AbstractValidator<RefundPaymentCommand>
{
    public RefundPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty()
            .WithMessage("Payment ID is required");

        RuleFor(x => x.RefundAmount)
            .GreaterThan(0)
            .WithMessage("Refund amount must be greater than 0");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Refund reason is required")
            .MaximumLength(500)
            .WithMessage("Refund reason cannot exceed 500 characters");
    }
}

/// <summary>
/// Command to allocate payments to invoices
/// </summary>
public record AllocatePaymentCommand(
    Guid PaymentId,
    List<PaymentAllocationDto> Allocations
) : IRequest<PaymentDto>;

/// <summary>
/// Handler for allocating payments
/// </summary>
public class AllocatePaymentCommandHandler : IRequestHandler<AllocatePaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AllocatePaymentCommandHandler> _logger;

    public AllocatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IInvoiceRepository invoiceRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<AllocatePaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<PaymentDto> Handle(AllocatePaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Allocating payment: {PaymentId}", request.PaymentId);

        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null)
        {
            throw new InvalidOperationException($"Payment with ID {request.PaymentId} not found");
        }

        var customer = await _customerRepository.GetByIdAsync(payment.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {payment.CustomerId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Validate allocations
        var totalAllocationAmount = request.Allocations.Sum(a => a.Amount);
        var availableAmount = payment.Amount.Amount - payment.Allocations.Sum(a => a.Amount.Amount);
        
        if (totalAllocationAmount > availableAmount)
        {
            throw new InvalidOperationException($"Total allocation amount {totalAllocationAmount} exceeds available amount {availableAmount}");
        }

        // Validate invoices
        var invoices = new List<Invoice>();
        foreach (var allocationDto in request.Allocations)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(allocationDto.InvoiceId, cancellationToken);
            if (invoice == null)
            {
                throw new InvalidOperationException($"Invoice with ID {allocationDto.InvoiceId} not found");
            }

            if (invoice.CustomerId != payment.CustomerId)
            {
                throw new InvalidOperationException($"Invoice {allocationDto.InvoiceId} does not belong to the same customer");
            }

            if (invoice.BalanceAmount.Amount < allocationDto.Amount)
            {
                throw new InvalidOperationException($"Allocation amount {allocationDto.Amount} exceeds invoice balance {invoice.BalanceAmount.Amount}");
            }

            invoices.Add(invoice);
        }

        // Add allocations to payment
        for (int i = 0; i < request.Allocations.Count; i++)
        {
            var allocationDto = request.Allocations[i];
            var allocation = PaymentAllocation.Create(
                invoiceId: allocationDto.InvoiceId,
                amount: allocationDto.Amount);
            
            payment.AddAllocation(allocation, currentUserId);

            // Update invoice
            var invoice = invoices[i];
            invoice.ApplyPayment(allocationDto.Amount, payment.Id, currentUserId);
            await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
        }

        // Save changes
        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(payment.DomainEvents, cancellationToken);
        payment.ClearDomainEvents();

        _logger.LogInformation("Successfully allocated payment {PaymentId}", request.PaymentId);

        return MapToDto(payment, customer);
    }

    private static PaymentDto MapToDto(Payment payment, Customer customer)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            TenantId = payment.TenantId,
            CustomerId = payment.CustomerId,
            CustomerName = customer.Name,
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
/// Validator for AllocatePaymentCommand
/// </summary>
public class AllocatePaymentCommandValidator : AbstractValidator<AllocatePaymentCommand>
{
    public AllocatePaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty()
            .WithMessage("Payment ID is required");

        RuleFor(x => x.Allocations)
            .NotEmpty()
            .WithMessage("At least one allocation is required");

        RuleForEach(x => x.Allocations)
            .SetValidator(new PaymentAllocationDtoValidator());
    }
}
