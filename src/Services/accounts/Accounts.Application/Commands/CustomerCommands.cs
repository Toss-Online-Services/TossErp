using FluentValidation;

namespace TossErp.Accounts.Application.Commands;

/// <summary>
/// Command to create a new customer
/// </summary>
public record CreateCustomerCommand(
    string Name,
    string? Email,
    string? Phone,
    CustomerType Type,
    string? CompanyName,
    string? TaxId,
    string? Website,
    CustomerAddress? BillingAddress,
    CustomerAddress? ShippingAddress,
    CustomerContact? PrimaryContact,
    List<CustomerContact>? AdditionalContacts,
    string? PreferredCurrency,
    string? PreferredLanguage,
    PaymentTerms? PaymentTerms,
    decimal? CreditLimit,
    string? Notes
) : IRequest<CustomerDto>;

/// <summary>
/// Handler for creating customers
/// </summary>
public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<CreateCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating customer: {CustomerName}", request.Name);

        var tenantId = _currentUserService.TenantId ?? throw new InvalidOperationException("Tenant ID is required");
        var currentUserId = _currentUserService.UserId ?? "system";

        // Validate email uniqueness
        if (!string.IsNullOrEmpty(request.Email))
        {
            var existingCustomer = await _customerRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingCustomer != null)
            {
                throw new InvalidOperationException($"Customer with email {request.Email} already exists");
            }
        }

        // Create customer
        var customer = Customer.Create(
            tenantId: tenantId,
            name: request.Name,
            email: request.Email,
            phone: request.Phone,
            type: request.Type,
            companyName: request.CompanyName,
            taxId: request.TaxId,
            website: request.Website,
            billingAddress: request.BillingAddress,
            shippingAddress: request.ShippingAddress,
            primaryContact: request.PrimaryContact,
            preferredCurrency: request.PreferredCurrency,
            preferredLanguage: request.PreferredLanguage,
            notes: request.Notes,
            createdBy: currentUserId);

        // Add additional contacts
        if (request.AdditionalContacts?.Any() == true)
        {
            foreach (var contact in request.AdditionalContacts)
            {
                customer.AddContact(contact, currentUserId);
            }
        }

        // Set payment terms
        if (request.PaymentTerms ?? PaymentTerms.Net30.HasValue)
        {
            customer.SetPaymentTerms(request.PaymentTerms ?? PaymentTerms.Net30, currentUserId);
        }

        // Set credit limit
        if (Money.Create(request.CreditLimit ?? 0, "ZAR").HasValue)
        {
            customer.SetCreditLimit(Money.Create(request.CreditLimit ?? 0, "ZAR"), currentUserId);
        }

        // Save customer
        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(customer.DomainEvents, cancellationToken);
        customer.ClearDomainEvents();

        _logger.LogInformation("Successfully created customer {CustomerId}", customer.Id);

        return MapToDto(customer);
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            TenantId = customer.TenantId,
            CustomerNumber = customer.CustomerNumber,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Status = customer.Status,
            Type = customer.Type,
            CompanyName = customer.CompanyName,
            TaxId = customer.TaxId,
            Website = customer.Website,
            BillingAddress = customer.BillingAddress != null ? MapAddressToDto(customer.BillingAddress) : null,
            ShippingAddress = customer.ShippingAddress != null ? MapAddressToDto(customer.ShippingAddress) : null,
            PrimaryContact = customer.PrimaryContact != null ? MapContactToDto(customer.PrimaryContact) : null,
            AdditionalContacts = customer.AdditionalContacts.Select(MapContactToDto).ToList(),
            PreferredCurrency = customer.PreferredCurrency,
            PreferredLanguage = customer.PreferredLanguage,
            PaymentTerms = customer.PaymentTerms,
            CreditLimit = customer.CreditLimit.Amount,
            CurrentBalance = customer.CurrentBalance.Amount,
            LastPaymentDate = customer.LastPaymentDate.HasValue ? DateOnly.FromDateTime(customer.LastPaymentDate.Value) : null,
            Notes = customer.Notes,
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
            Street = address.Street,
            Street2 = address.Street2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country
        };
    }

    private static CustomerContactDto MapContactToDto(CustomerContact contact)
    {
        return new CustomerContactDto
        {
            Name = contact.Name,
            Email = contact.Email,
            Phone = contact.Phone,
            Position = contact.Position,
            IsPrimary = contact.IsPrimary
        };
    }
}

/// <summary>
/// Validator for CreateCustomerCommand
/// </summary>
public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Customer name is required")
            .MaximumLength(200)
            .WithMessage("Customer name cannot exceed 200 characters");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Valid email address is required");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Valid customer type is required");

        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .When(x => x.Type == CustomerType.Business)
            .WithMessage("Company name is required for business customers");

        RuleFor(x => x.CreditLimit)
            .GreaterThanOrEqualTo(0)
            .When(x => x.CreditLimit.HasValue)
            .WithMessage("Credit limit cannot be negative");
    }
}

/// <summary>
/// Command to update an existing customer
/// </summary>
public record UpdateCustomerCommand(
    Guid CustomerId,
    string Name,
    string? Email,
    string? Phone,
    string? CompanyName,
    string? TaxId,
    string? Website,
    CustomerAddress? BillingAddress,
    CustomerAddress? ShippingAddress,
    CustomerContact? PrimaryContact,
    string? PreferredCurrency,
    string? PreferredLanguage,
    PaymentTerms? PaymentTerms,
    decimal? CreditLimit,
    string? Notes
) : IRequest<CustomerDto>;

/// <summary>
/// Handler for updating customers
/// </summary>
public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateCustomerCommandHandler> _logger;

    public UpdateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<UpdateCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CustomerDto> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating customer: {CustomerId}", request.CustomerId);

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {request.CustomerId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Validate email uniqueness if changed
        if (!string.IsNullOrEmpty(request.Email) && request.Email != customer.Email)
        {
            var existingCustomer = await _customerRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingCustomer != null)
            {
                throw new InvalidOperationException($"Customer with email {request.Email} already exists");
            }
        }

        // Update basic information
        customer.UpdateBasicInfo(
            name: request.Name,
            email: request.Email,
            phone: request.Phone,
            companyName: request.CompanyName,
            taxId: request.TaxId,
            website: request.Website,
            notes: request.Notes,
            updatedBy: currentUserId);

        // Update addresses
        if (request.BillingAddress != null)
        {
            customer.UpdateBillingAddress(request.BillingAddress, currentUserId);
        }

        if (request.ShippingAddress != null)
        {
            customer.UpdateShippingAddress(request.ShippingAddress, currentUserId);
        }

        // Update primary contact
        if (request.PrimaryContact != null)
        {
            customer.UpdatePrimaryContact(request.PrimaryContact, currentUserId);
        }

        // Update preferences
        if (!string.IsNullOrEmpty(request.PreferredCurrency))
        {
            customer.SetPreferredCurrency(request.PreferredCurrency, currentUserId);
        }

        if (!string.IsNullOrEmpty(request.PreferredLanguage))
        {
            customer.SetPreferredLanguage(request.PreferredLanguage, currentUserId);
        }

        // Update payment terms
        if (request.PaymentTerms ?? PaymentTerms.Net30.HasValue)
        {
            customer.SetPaymentTerms(request.PaymentTerms ?? PaymentTerms.Net30, currentUserId);
        }

        // Update credit limit
        if (Money.Create(request.CreditLimit ?? 0, "ZAR").HasValue)
        {
            customer.SetCreditLimit(Money.Create(request.CreditLimit ?? 0, "ZAR"), currentUserId);
        }

        // Save changes
        await _customerRepository.UpdateAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(customer.DomainEvents, cancellationToken);
        customer.ClearDomainEvents();

        _logger.LogInformation("Successfully updated customer {CustomerId}", request.CustomerId);

        return MapToDto(customer);
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            TenantId = customer.TenantId,
            CustomerNumber = customer.CustomerNumber,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Status = customer.Status,
            Type = customer.Type,
            CompanyName = customer.CompanyName,
            TaxId = customer.TaxId,
            Website = customer.Website,
            BillingAddress = customer.BillingAddress != null ? MapAddressToDto(customer.BillingAddress) : null,
            ShippingAddress = customer.ShippingAddress != null ? MapAddressToDto(customer.ShippingAddress) : null,
            PrimaryContact = customer.PrimaryContact != null ? MapContactToDto(customer.PrimaryContact) : null,
            AdditionalContacts = customer.AdditionalContacts.Select(MapContactToDto).ToList(),
            PreferredCurrency = customer.PreferredCurrency,
            PreferredLanguage = customer.PreferredLanguage,
            PaymentTerms = customer.PaymentTerms,
            CreditLimit = customer.CreditLimit.Amount,
            CurrentBalance = customer.CurrentBalance.Amount,
            LastPaymentDate = customer.LastPaymentDate.HasValue ? DateOnly.FromDateTime(customer.LastPaymentDate.Value) : null,
            Notes = customer.Notes,
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
            Street = address.Street,
            Street2 = address.Street2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country
        };
    }

    private static CustomerContactDto MapContactToDto(CustomerContact contact)
    {
        return new CustomerContactDto
        {
            Name = contact.Name,
            Email = contact.Email,
            Phone = contact.Phone,
            Position = contact.Position,
            IsPrimary = contact.IsPrimary
        };
    }
}

/// <summary>
/// Validator for UpdateCustomerCommand
/// </summary>
public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Customer name is required")
            .MaximumLength(200)
            .WithMessage("Customer name cannot exceed 200 characters");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Valid email address is required");

        RuleFor(x => x.CreditLimit)
            .GreaterThanOrEqualTo(0)
            .When(x => x.CreditLimit.HasValue)
            .WithMessage("Credit limit cannot be negative");
    }
}

/// <summary>
/// Command to change customer status
/// </summary>
public record ChangeCustomerStatusCommand(
    Guid CustomerId,
    CustomerStatus NewStatus,
    string? Reason
) : IRequest<CustomerDto>;

/// <summary>
/// Handler for changing customer status
/// </summary>
public class ChangeCustomerStatusCommandHandler : IRequestHandler<ChangeCustomerStatusCommand, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<ChangeCustomerStatusCommandHandler> _logger;

    public ChangeCustomerStatusCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ILogger<ChangeCustomerStatusCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<CustomerDto> Handle(ChangeCustomerStatusCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changing customer {CustomerId} status to {NewStatus}", request.CustomerId, request.NewStatus);

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {request.CustomerId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";
        var oldStatus = customer.Status;

        // Change status
        customer.ChangeStatus(request.NewStatus, currentUserId);

        // Save changes
        await _customerRepository.UpdateAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification if customer email is available
        if (!string.IsNullOrEmpty(customer.Email))
        {
            // Note: This would be implemented based on specific notification requirements
            // await _notificationService.SendCustomerStatusChangeNotificationAsync(customer.Id, customer.Email, oldStatus, request.NewStatus, cancellationToken);
        }

        // Publish domain events
        await _domainEventService.PublishAsync(customer.DomainEvents, cancellationToken);
        customer.ClearDomainEvents();

        _logger.LogInformation("Successfully changed customer {CustomerId} status from {OldStatus} to {NewStatus}", 
            request.CustomerId, oldStatus, request.NewStatus);

        return MapToDto(customer);
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            TenantId = customer.TenantId,
            CustomerNumber = customer.CustomerNumber,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Status = customer.Status,
            Type = customer.Type,
            CompanyName = customer.CompanyName,
            TaxId = customer.TaxId,
            Website = customer.Website,
            BillingAddress = customer.BillingAddress != null ? MapAddressToDto(customer.BillingAddress) : null,
            ShippingAddress = customer.ShippingAddress != null ? MapAddressToDto(customer.ShippingAddress) : null,
            PrimaryContact = customer.PrimaryContact != null ? MapContactToDto(customer.PrimaryContact) : null,
            AdditionalContacts = customer.AdditionalContacts.Select(MapContactToDto).ToList(),
            PreferredCurrency = customer.PreferredCurrency,
            PreferredLanguage = customer.PreferredLanguage,
            PaymentTerms = customer.PaymentTerms,
            CreditLimit = customer.CreditLimit.Amount,
            CurrentBalance = customer.CurrentBalance.Amount,
            LastPaymentDate = customer.LastPaymentDate.HasValue ? DateOnly.FromDateTime(customer.LastPaymentDate.Value) : null,
            Notes = customer.Notes,
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
            Street = address.Street,
            Street2 = address.Street2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country
        };
    }

    private static CustomerContactDto MapContactToDto(CustomerContact contact)
    {
        return new CustomerContactDto
        {
            Name = contact.Name,
            Email = contact.Email,
            Phone = contact.Phone,
            Position = contact.Position,
            IsPrimary = contact.IsPrimary
        };
    }
}

/// <summary>
/// Validator for ChangeCustomerStatusCommand
/// </summary>
public class ChangeCustomerStatusCommandValidator : AbstractValidator<ChangeCustomerStatusCommand>
{
    public ChangeCustomerStatusCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Valid customer status is required");
    }
}
