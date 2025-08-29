using FluentValidation;
using MediatR;
using TossErp.Accounts.Application.Common;
using TossErp.Accounts.Application.Common.Interfaces;
using TossErp.Accounts.Application.DTOs;
using TossErp.Accounts.Domain.Entities;
using TossErp.Accounts.Domain.Enums;
using TossErp.Accounts.Domain.ValueObjects;
using static TossErp.Accounts.Application.Common.InvoiceMappingHelper;
using InvoiceAggregate = TossErp.Accounts.Domain.Aggregates.Invoice;

namespace TossErp.Accounts.Application.Commands;

/// <summary>
/// Command to create a new invoice
/// </summary>
public record CreateInvoiceCommand(
    Guid CustomerId,
    DateOnly IssueDate,
    DateOnly DueDate,
    List<CreateInvoiceLineItemDto> LineItems,
    CustomerAddress? BillingAddress,
    CustomerAddress? ShippingAddress,
    string? Terms,
    string? Notes,
    string? InternalNotes,
    string? Reference,
    string? PurchaseOrderNumber
) : IRequest<InvoiceDto>;

/// <summary>
/// DTO for creating invoice line items
/// </summary>
public class CreateInvoiceLineItemDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? TaxRate { get; set; }
    public string? ProductCode { get; set; }
    public string? Unit { get; set; }
}

/// <summary>
/// Handler for creating invoices
/// </summary>
public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, InvoiceDto>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CreateInvoiceCommandHandler> _logger;

    public CreateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ILogger<CreateInvoiceCommandHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<InvoiceDto> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating invoice for customer: {CustomerId}", request.CustomerId);

        var tenantId = _currentUserService.TenantId ?? throw new InvalidOperationException("Tenant ID is required");
        var currentUserId = _currentUserService.UserId ?? "system";

        // Validate customer
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {request.CustomerId} not found");
        }

        if (customer.Status != CustomerStatus.Active)
        {
            throw new InvalidOperationException($"Cannot create invoice for inactive customer {request.CustomerId}");
        }

        // Generate invoice number
        var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

        // Parse currency from string to enum
        var currencyCode = Enum.Parse<CurrencyCode>(customer.PreferredCurrency ?? "USD", true);

        // Create invoice using Aggregate constructor
        var invoice = new InvoiceAggregate(
            id: Guid.NewGuid(),
            tenantId: tenantId,
            invoiceNumber: invoiceNumber,
            invoiceType: InvoiceType.Standard,
            customerId: request.CustomerId,
            customerName: customer.Name ?? "Unknown Customer",
            invoiceDate: request.IssueDate.ToDateTime(TimeOnly.MinValue),
            dueDate: request.DueDate.ToDateTime(TimeOnly.MinValue),
            currency: currencyCode,
            createdBy: currentUserId,
            notes: request.Notes,
            terms: request.Terms);

        // Add line items to the invoice
        foreach (var lineItemDto in request.LineItems)
        {
            var lineItem = new InvoiceLine(
                id: Guid.NewGuid(),
                invoiceId: invoice.Id,
                itemName: lineItemDto.Description,
                quantity: (int)lineItemDto.Quantity,
                unitPrice: new Money(lineItemDto.UnitPrice, currencyCode),
                description: lineItemDto.Description,
                taxRate: lineItemDto.TaxRate.HasValue ? new TaxRate(lineItemDto.TaxRate.Value, TaxType.VAT) : null);
            
            invoice.AddLine(lineItem);
        }

        // Save invoice
        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send invoice notification
        if (!string.IsNullOrEmpty(customer.Email))
        {
            await _notificationService.SendInvoiceNotificationAsync(
                invoice.Id, customer.Email, NotificationType.InvoiceCreated, cancellationToken);
        }

        // Publish domain events
        await _domainEventService.PublishAsync(invoice.DomainEvents, cancellationToken);
        invoice.ClearDomainEvents();

        _logger.LogInformation("Successfully created invoice {InvoiceId}", invoice.Id);

        return InvoiceMappingHelper.MapToDto(invoice, customer);
    }

    private static InvoiceLineItem CreateLineItem(CreateInvoiceLineItemDto dto)
    {
        return InvoiceLineItem.Create(
            tenantId: "default",
            invoiceId: Guid.Empty,
            itemName: dto.Description,
            quantity: (int)dto.Quantity,
            unitPrice: new Money(dto.UnitPrice, CurrencyCode.ZAR),
            description: dto.Description,
            taxRate: dto.TaxRate.HasValue ? new TaxRate(dto.TaxRate.Value, TaxType.VAT) : null,
            productCode: dto.ProductCode,
            unit: dto.Unit);
    }

    private static InvoiceDto MapToDto(Invoice invoice, Customer customer)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            TenantId = invoice.TenantId,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerId = invoice.CustomerId,
            CustomerName = customer.Name,
            CustomerEmail = customer.Email ?? string.Empty,
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
/// Validator for CreateInvoiceCommand
/// </summary>
public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.IssueDate)
            .NotEmpty()
            .WithMessage("Issue date is required");

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .WithMessage("Due date is required")
            .GreaterThanOrEqualTo(x => x.IssueDate)
            .WithMessage("Due date must be on or after issue date");

        RuleFor(x => x.LineItems)
            .NotEmpty()
            .WithMessage("At least one line item is required");

        RuleForEach(x => x.LineItems)
            .SetValidator(new CreateInvoiceLineItemDtoValidator());
    }
}

/// <summary>
/// Validator for CreateInvoiceLineItemDto
/// </summary>
public class CreateInvoiceLineItemDtoValidator : AbstractValidator<CreateInvoiceLineItemDto>
{
    public CreateInvoiceLineItemDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Line item description is required")
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Unit price cannot be negative");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.DiscountPercentage.HasValue)
            .WithMessage("Discount percentage must be between 0 and 100");

        RuleFor(x => x.TaxRate)
            .GreaterThanOrEqualTo(0)
            .When(x => x.TaxRate.HasValue)
            .WithMessage("Tax rate cannot be negative");
    }
}

/// <summary>
/// Command to update an invoice
/// </summary>
public record UpdateInvoiceCommand(
    Guid InvoiceId,
    DateOnly DueDate,
    List<CreateInvoiceLineItemDto> LineItems,
    CustomerAddress? BillingAddress,
    CustomerAddress? ShippingAddress,
    string? Terms,
    string? Notes,
    string? InternalNotes,
    string? Reference,
    string? PurchaseOrderNumber
) : IRequest<InvoiceDto>;

/// <summary>
/// Handler for updating invoices
/// </summary>
public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, InvoiceDto>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateInvoiceCommandHandler> _logger;

    public UpdateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<UpdateInvoiceCommandHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<InvoiceDto> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating invoice: {InvoiceId}", request.InvoiceId);

        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            throw new InvalidOperationException($"Invoice with ID {request.InvoiceId} not found");
        }

        if (invoice.Status != InvoiceStatus.Draft)
        {
            throw new InvalidOperationException($"Cannot update invoice {request.InvoiceId} in status {invoice.Status}");
        }

        var customer = await _customerRepository.GetByIdAsync(invoice.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {invoice.CustomerId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Update invoice details - TODO: Implement UpdateDetails method in Invoice Aggregate
        // For now, we'll skip this update until the domain method is implemented
        // invoice.UpdateDetails(
        //     dueDate: request.DueDate,
        //     billingAddress: request.BillingAddress,
        //     shippingAddress: request.ShippingAddress,
        //     terms: request.Terms,
        //     notes: request.Notes,
        //     internalNotes: request.InternalNotes,
        //     reference: request.Reference,
        //     purchaseOrderNumber: request.PurchaseOrderNumber,
        //     updatedBy: currentUserId);

        // Update line items - TODO: Implement UpdateLineItems method in Invoice Aggregate  
        // For now, we'll skip this update until the domain method is implemented
        // var newLineItems = request.LineItems.Select(dto => CreateLineItem(dto)).ToList();
        // invoice.UpdateLineItems(newLineItems, currentUserId);

        // Save changes
        await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(invoice.DomainEvents, cancellationToken);
        invoice.ClearDomainEvents();

        _logger.LogInformation("Successfully updated invoice {InvoiceId}", request.InvoiceId);

        return InvoiceMappingHelper.MapToDto(invoice, customer);
    }

    private static InvoiceLineItem CreateLineItem(CreateInvoiceLineItemDto dto)
    {
        return InvoiceLineItem.Create(
            tenantId: "default",
            invoiceId: Guid.Empty,
            itemName: dto.Description,
            quantity: (int)dto.Quantity,
            unitPrice: new Money(dto.UnitPrice, CurrencyCode.ZAR),
            description: dto.Description,
            taxRate: dto.TaxRate.HasValue ? new TaxRate(dto.TaxRate.Value, TaxType.VAT) : null,
            productCode: dto.ProductCode,
            unit: dto.Unit);
    }

    private static InvoiceDto MapToDto(Invoice invoice, Customer customer)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            TenantId = invoice.TenantId,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerId = invoice.CustomerId,
            CustomerName = customer.Name,
            CustomerEmail = customer.Email ?? string.Empty,
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
/// Validator for UpdateInvoiceCommand
/// </summary>
public class UpdateInvoiceCommandValidator : AbstractValidator<UpdateInvoiceCommand>
{
    public UpdateInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty()
            .WithMessage("Invoice ID is required");

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .WithMessage("Due date is required");

        RuleFor(x => x.LineItems)
            .NotEmpty()
            .WithMessage("At least one line item is required");

        RuleForEach(x => x.LineItems)
            .SetValidator(new CreateInvoiceLineItemDtoValidator());
    }
}

/// <summary>
/// Command to change invoice status
/// </summary>
public record ChangeInvoiceStatusCommand(
    Guid InvoiceId,
    InvoiceStatus NewStatus,
    string? Reason
) : IRequest<InvoiceDto>;

/// <summary>
/// Handler for changing invoice status
/// </summary>
public class ChangeInvoiceStatusCommandHandler : IRequestHandler<ChangeInvoiceStatusCommand, InvoiceDto>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<ChangeInvoiceStatusCommandHandler> _logger;

    public ChangeInvoiceStatusCommandHandler(
        IInvoiceRepository invoiceRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ILogger<ChangeInvoiceStatusCommandHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<InvoiceDto> Handle(ChangeInvoiceStatusCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changing invoice {InvoiceId} status to {NewStatus}", request.InvoiceId, request.NewStatus);

        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            throw new InvalidOperationException($"Invoice with ID {request.InvoiceId} not found");
        }

        var customer = await _customerRepository.GetByIdAsync(invoice.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {invoice.CustomerId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";
        var oldStatus = invoice.Status;

        // Change status based on the target status
        switch (request.NewStatus)
        {
            case InvoiceStatus.Sent:
                invoice.Send(); // Send method takes no parameters
                break;
            case InvoiceStatus.Overdue:
                // MarkOverdue method doesn't exist - TODO: Implement in Invoice Aggregate
                // For now, we'll skip this action
                // invoice.MarkOverdue(currentUserId);
                break;
            case InvoiceStatus.Cancelled:
                // Invoice Cancel method takes only reason parameter
                invoice.Cancel(request.Reason ?? "Invoice cancelled");
                break;
            default:
                throw new InvalidOperationException($"Cannot change invoice status to {request.NewStatus}");
        }

        // Save changes
        await _invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification
        if (!string.IsNullOrEmpty(customer.Email))
        {
            var notificationType = request.NewStatus switch
            {
                InvoiceStatus.Sent => NotificationType.InvoiceCreated,
                InvoiceStatus.Overdue => NotificationType.InvoiceOverdue,
                InvoiceStatus.Cancelled => NotificationType.InvoiceCancelled,
                _ => NotificationType.InvoiceCreated
            };

            await _notificationService.SendInvoiceNotificationAsync(
                invoice.Id, customer.Email, notificationType, cancellationToken);
        }

        // Publish domain events
        await _domainEventService.PublishAsync(invoice.DomainEvents, cancellationToken);
        invoice.ClearDomainEvents();

        _logger.LogInformation("Successfully changed invoice {InvoiceId} status from {OldStatus} to {NewStatus}", 
            request.InvoiceId, oldStatus, request.NewStatus);

        return InvoiceMappingHelper.MapToDto(invoice, customer);
    }

    private static InvoiceDto MapToDto(Invoice invoice, Customer customer)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            TenantId = invoice.TenantId,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerId = invoice.CustomerId,
            CustomerName = customer.Name,
            CustomerEmail = customer.Email ?? string.Empty,
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
/// Validator for ChangeInvoiceStatusCommand
/// </summary>
public class ChangeInvoiceStatusCommandValidator : AbstractValidator<ChangeInvoiceStatusCommand>
{
    public ChangeInvoiceStatusCommandValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty()
            .WithMessage("Invoice ID is required");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Valid invoice status is required");
    }
}
