using MediatR;
using TossErp.Procurement.Application.Common.DTOs;
using TossErp.Procurement.Application.Common.Interfaces;
using TossErp.Procurement.Domain.Common;
using TossErp.Procurement.Domain.Entities;

namespace TossErp.Procurement.Application.Commands.CreateSupplier;

/// <summary>
/// Handler for CreateSupplierCommand
/// </summary>
public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, SupplierDto>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;

    public CreateSupplierCommandHandler(
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
    }

    public async Task<SupplierDto> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        // Check if supplier code already exists
        var existingSupplier = await _supplierRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (existingSupplier != null)
            throw new InvalidOperationException($"Supplier with code '{request.Code}' already exists");

        // Get tenant ID from current user
        var tenantId = _currentUserService.TenantId ?? "default-tenant";
        var currentUser = _currentUserService.UserName ?? "system";

        // Create supplier
        var supplier = Supplier.Create(request.Name, request.Code, tenantId);

        // Update contact information if provided
        if (!string.IsNullOrWhiteSpace(request.ContactPerson) || 
            !string.IsNullOrWhiteSpace(request.Email) || 
            !string.IsNullOrWhiteSpace(request.Phone))
        {
            supplier.UpdateContactInfo(
                request.ContactPerson,
                request.Email,
                request.Phone);
        }

        // Update business information if provided
        if (!string.IsNullOrWhiteSpace(request.TaxNumber) || !string.IsNullOrWhiteSpace(request.RegistrationNumber))
        {
            supplier.UpdateBusinessInfo(null, null, null, null, null, request.TaxNumber);
        }

        // Update financial information if provided
        if (request.CreditLimit.HasValue || request.PaymentTermsDays.HasValue)
        {
            supplier.UpdateFinancialInfo(null, null, null, null);
        }

        // Update operational information if provided
        if (request.LeadTimeDays.HasValue)
        {
            supplier.UpdateOperationalInfo(request.LeadTimeDays, currentUser);
        }

        // Add notes if provided
        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            supplier.AddNotes(request.Notes);
        }

        // Save supplier
        await _supplierRepository.AddAsync(supplier, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(supplier.DomainEvents, cancellationToken);
        supplier.ClearDomainEvents();

        // Return DTO
        return MapToDto(supplier);
    }

    private static SupplierDto MapToDto(Supplier supplier)
    {
        return new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Code = supplier.Code,
            ContactPerson = supplier.ContactPerson,
            Email = supplier.Email,
            Phone = supplier.Phone,
            Address = supplier.Address,
            City = supplier.City,
            PostalCode = supplier.PostalCode,
            Country = supplier.Country,
            TaxNumber = supplier.TaxNumber,
            RegistrationNumber = supplier.RegistrationNumber,
            Status = supplier.Status,
            Notes = supplier.Notes,
            CreditLimit = supplier.CreditLimit,
            PaymentTermsDays = supplier.PaymentTermsDays,
            LeadTimeDays = supplier.LeadTimeDays,
            CreatedAt = supplier.CreatedAt,
            UpdatedAt = supplier.UpdatedAt,
            CreatedBy = supplier.CreatedBy,
            UpdatedBy = supplier.UpdatedBy,
            TenantId = supplier.TenantId
        };
    }
}
