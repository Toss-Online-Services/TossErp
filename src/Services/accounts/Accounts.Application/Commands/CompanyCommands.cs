using FluentValidation;
using MediatR;
using TossErp.Accounts.Application.DTOs;
using TossErp.Accounts.Domain.Entities;

namespace TossErp.Accounts.Application.Commands;

/// <summary>
/// Command to create a new company
/// </summary>
public record CreateCompanyCommand(
    string Name,
    string Abbreviation,
    string Currency,
    string Country,
    string? Domain = null,
    string? TaxId = null,
    Guid? ParentCompanyId = null,
    bool IsGroup = false,
    string? Description = null,
    string? Email = null,
    string? Phone = null,
    string? Website = null,
    string? Address = null,
    string? City = null,
    string? State = null,
    string? PostalCode = null,
    DateTime? DateOfIncorporation = null,
    DateTime? DateOfCommencement = null,
    string? RegistrationNumber = null,
    int FiscalYearStartMonth = 1
) : IRequest<CompanyDto>;

/// <summary>
/// Handler for creating companies
/// </summary>
public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CompanyDto>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateCompanyCommandHandler> _logger;

    public CreateCompanyCommandHandler(
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<CreateCompanyCommandHandler> logger)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CompanyDto> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating company: {CompanyName}", request.Name);

        var tenantId = _currentUserService.TenantId ?? throw new InvalidOperationException("Tenant ID is required");
        var currentUserId = _currentUserService.UserId ?? "system";

        // Validate name uniqueness within tenant
        var existingCompany = await _companyRepository.GetByNameAsync(tenantId, request.Name, cancellationToken);
        if (existingCompany != null)
        {
            throw new InvalidOperationException($"Company with name '{request.Name}' already exists");
        }

        // Validate abbreviation uniqueness within tenant
        var existingAbbreviation = await _companyRepository.GetByAbbreviationAsync(tenantId, request.Abbreviation, cancellationToken);
        if (existingAbbreviation != null)
        {
            throw new InvalidOperationException($"Company with abbreviation '{request.Abbreviation}' already exists");
        }

        // Validate parent company exists if specified
        if (request.ParentCompanyId.HasValue)
        {
            var parentCompany = await _companyRepository.GetByIdAsync(request.ParentCompanyId.Value, cancellationToken);
            if (parentCompany == null)
            {
                throw new InvalidOperationException($"Parent company with ID {request.ParentCompanyId} not found");
            }
            if (!parentCompany.IsGroup)
            {
                throw new InvalidOperationException("Parent company must be marked as a group company");
            }
        }

        // Create company
        var company = Company.Create(
            tenantId: tenantId,
            name: request.Name,
            abbreviation: request.Abbreviation,
            currency: request.Currency,
            country: request.Country,
            createdBy: currentUserId,
            domain: request.Domain,
            taxId: request.TaxId,
            parentCompanyId: request.ParentCompanyId,
            isGroup: request.IsGroup,
            description: request.Description);

        // Update contact information if provided
        if (!string.IsNullOrEmpty(request.Email) || !string.IsNullOrEmpty(request.Phone) || !string.IsNullOrEmpty(request.Website))
        {
            company.UpdateContactInfo(request.Email, request.Phone, request.Website, currentUserId);
        }

        // Update address if provided
        if (!string.IsNullOrEmpty(request.Address) || !string.IsNullOrEmpty(request.City) || 
            !string.IsNullOrEmpty(request.State) || !string.IsNullOrEmpty(request.PostalCode))
        {
            company.UpdateAddress(request.Address, request.City, request.State, request.PostalCode, currentUserId);
        }

        // Update tax information if provided
        if (!string.IsNullOrEmpty(request.TaxId) || !string.IsNullOrEmpty(request.RegistrationNumber) ||
            request.DateOfIncorporation.HasValue || request.DateOfCommencement.HasValue)
        {
            company.UpdateTaxInfo(request.TaxId, request.RegistrationNumber, 
                request.DateOfIncorporation, request.DateOfCommencement, currentUserId);
        }

        // Set fiscal year start month if different from default
        if (request.FiscalYearStartMonth != 1)
        {
            company.SetFiscalYearStartMonth(request.FiscalYearStartMonth, currentUserId);
        }

        // Save company
        await _companyRepository.AddAsync(company, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(company.DomainEvents, cancellationToken);
        company.ClearDomainEvents();

        _logger.LogInformation("Successfully created company {CompanyId}", company.Id);

        return await MapToDtoAsync(company, cancellationToken);
    }

    private async Task<CompanyDto> MapToDtoAsync(Company company, CancellationToken cancellationToken)
    {
        var dto = new CompanyDto
        {
            Id = company.Id,
            TenantId = company.TenantId,
            Name = company.Name,
            Abbreviation = company.Abbreviation,
            Domain = company.Domain,
            Currency = company.Currency,
            Country = company.Country,
            TaxId = company.TaxId,
            ParentCompanyId = company.ParentCompanyId,
            IsGroup = company.IsGroup,
            IsActive = company.IsActive,
            Description = company.Description,
            Logo = company.Logo,
            RegistrationDate = company.RegistrationDate,
            FiscalYearStartMonth = company.FiscalYearStartMonth,
            DateOfIncorporation = company.DateOfIncorporation,
            DateOfCommencement = company.DateOfCommencement,
            RegistrationNumber = company.RegistrationNumber,
            Website = company.Website,
            Phone = company.Phone,
            Email = company.Email,
            Address = company.Address,
            City = company.City,
            State = company.State,
            PostalCode = company.PostalCode,
            CreatedAt = company.CreatedAt,
            ModifiedAt = company.ModifiedAt,
            CreatedBy = company.CreatedBy,
            ModifiedBy = company.ModifiedBy
        };

        // Load parent company name if exists
        if (company.ParentCompanyId.HasValue)
        {
            var parentCompany = await _companyRepository.GetByIdAsync(company.ParentCompanyId.Value, cancellationToken);
            dto.ParentCompanyName = parentCompany?.Name;
        }

        // Load child companies if this is a group
        if (company.IsGroup)
        {
            var childCompanies = await _companyRepository.GetChildCompaniesAsync(company.Id, cancellationToken);
            dto.ChildCompanies = childCompanies.Select(c => new CompanySummaryDto
            {
                Id = c.Id,
                Name = c.Name,
                Abbreviation = c.Abbreviation,
                Domain = c.Domain,
                Currency = c.Currency,
                Country = c.Country,
                IsGroup = c.IsGroup,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            }).ToList();
        }

        return dto;
    }
}

/// <summary>
/// Validator for CreateCompanyCommand
/// </summary>
public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Company name is required")
            .MaximumLength(200)
            .WithMessage("Company name cannot exceed 200 characters");

        RuleFor(x => x.Abbreviation)
            .NotEmpty()
            .WithMessage("Company abbreviation is required")
            .MaximumLength(10)
            .WithMessage("Company abbreviation cannot exceed 10 characters");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be a 3-letter code (e.g., USD, EUR, ZAR)");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required")
            .MaximumLength(100)
            .WithMessage("Country cannot exceed 100 characters");

        RuleFor(x => x.FiscalYearStartMonth)
            .InclusiveBetween(1, 12)
            .WithMessage("Fiscal year start month must be between 1 and 12");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Valid email address is required");

        RuleFor(x => x.Domain)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Domain))
            .WithMessage("Domain cannot exceed 100 characters");

        RuleFor(x => x.TaxId)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.TaxId))
            .WithMessage("Tax ID cannot exceed 50 characters");
    }
}

/// <summary>
/// Command to update an existing company
/// </summary>
public record UpdateCompanyCommand(
    Guid CompanyId,
    string Name,
    string Abbreviation,
    string Currency,
    string Country,
    string? Domain = null,
    string? TaxId = null,
    Guid? ParentCompanyId = null,
    bool IsGroup = false,
    string? Description = null,
    string? Email = null,
    string? Phone = null,
    string? Website = null,
    string? Address = null,
    string? City = null,
    string? State = null,
    string? PostalCode = null,
    DateTime? DateOfIncorporation = null,
    DateTime? DateOfCommencement = null,
    string? RegistrationNumber = null,
    int FiscalYearStartMonth = 1
) : IRequest<CompanyDto>;

/// <summary>
/// Handler for updating companies
/// </summary>
public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, CompanyDto>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateCompanyCommandHandler> _logger;

    public UpdateCompanyCommandHandler(
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<UpdateCompanyCommandHandler> logger)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CompanyDto> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating company: {CompanyId}", request.CompanyId);

        var company = await _companyRepository.GetByIdAsync(request.CompanyId, cancellationToken);
        if (company == null)
        {
            throw new InvalidOperationException($"Company with ID {request.CompanyId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Validate name uniqueness if changed
        if (request.Name != company.Name)
        {
            var existingCompany = await _companyRepository.GetByNameAsync(company.TenantId, request.Name, cancellationToken);
            if (existingCompany != null)
            {
                throw new InvalidOperationException($"Company with name '{request.Name}' already exists");
            }
        }

        // Validate abbreviation uniqueness if changed
        if (request.Abbreviation != company.Abbreviation)
        {
            var existingAbbreviation = await _companyRepository.GetByAbbreviationAsync(company.TenantId, request.Abbreviation, cancellationToken);
            if (existingAbbreviation != null)
            {
                throw new InvalidOperationException($"Company with abbreviation '{request.Abbreviation}' already exists");
            }
        }

        // Validate parent company if changed
        if (request.ParentCompanyId != company.ParentCompanyId)
        {
            if (request.ParentCompanyId.HasValue)
            {
                var parentCompany = await _companyRepository.GetByIdAsync(request.ParentCompanyId.Value, cancellationToken);
                if (parentCompany == null)
                {
                    throw new InvalidOperationException($"Parent company with ID {request.ParentCompanyId} not found");
                }
                if (!parentCompany.IsGroup)
                {
                    throw new InvalidOperationException("Parent company must be marked as a group company");
                }
                // Prevent circular references
                if (request.ParentCompanyId == company.Id)
                {
                    throw new InvalidOperationException("Company cannot be its own parent");
                }
            }
        }

        // Update basic information
        company.UpdateBasicInfo(
            name: request.Name,
            abbreviation: request.Abbreviation,
            currency: request.Currency,
            country: request.Country,
            modifiedBy: currentUserId,
            domain: request.Domain,
            description: request.Description);

        // Update contact information
        company.UpdateContactInfo(request.Email, request.Phone, request.Website, currentUserId);

        // Update address
        company.UpdateAddress(request.Address, request.City, request.State, request.PostalCode, currentUserId);

        // Update tax information
        company.UpdateTaxInfo(request.TaxId, request.RegistrationNumber, 
            request.DateOfIncorporation, request.DateOfCommencement, currentUserId);

        // Update parent company if changed
        if (request.ParentCompanyId != company.ParentCompanyId)
        {
            company.SetParentCompany(request.ParentCompanyId, currentUserId);
        }

        // Update group status if changed
        if (request.IsGroup != company.IsGroup)
        {
            company.SetAsGroup(request.IsGroup, currentUserId);
        }

        // Update fiscal year start month if changed
        if (request.FiscalYearStartMonth != company.FiscalYearStartMonth)
        {
            company.SetFiscalYearStartMonth(request.FiscalYearStartMonth, currentUserId);
        }

        // Save changes
        await _companyRepository.UpdateAsync(company, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(company.DomainEvents, cancellationToken);
        company.ClearDomainEvents();

        _logger.LogInformation("Successfully updated company {CompanyId}", request.CompanyId);

        return await MapToDtoAsync(company, cancellationToken);
    }

    private async Task<CompanyDto> MapToDtoAsync(Company company, CancellationToken cancellationToken)
    {
        var dto = new CompanyDto
        {
            Id = company.Id,
            TenantId = company.TenantId,
            Name = company.Name,
            Abbreviation = company.Abbreviation,
            Domain = company.Domain,
            Currency = company.Currency,
            Country = company.Country,
            TaxId = company.TaxId,
            ParentCompanyId = company.ParentCompanyId,
            IsGroup = company.IsGroup,
            IsActive = company.IsActive,
            Description = company.Description,
            Logo = company.Logo,
            RegistrationDate = company.RegistrationDate,
            FiscalYearStartMonth = company.FiscalYearStartMonth,
            DateOfIncorporation = company.DateOfIncorporation,
            DateOfCommencement = company.DateOfCommencement,
            RegistrationNumber = company.RegistrationNumber,
            Website = company.Website,
            Phone = company.Phone,
            Email = company.Email,
            Address = company.Address,
            City = company.City,
            State = company.State,
            PostalCode = company.PostalCode,
            CreatedAt = company.CreatedAt,
            ModifiedAt = company.ModifiedAt,
            CreatedBy = company.CreatedBy,
            ModifiedBy = company.ModifiedBy
        };

        // Load parent company name if exists
        if (company.ParentCompanyId.HasValue)
        {
            var parentCompany = await _companyRepository.GetByIdAsync(company.ParentCompanyId.Value, cancellationToken);
            dto.ParentCompanyName = parentCompany?.Name;
        }

        // Load child companies if this is a group
        if (company.IsGroup)
        {
            var childCompanies = await _companyRepository.GetChildCompaniesAsync(company.Id, cancellationToken);
            dto.ChildCompanies = childCompanies.Select(c => new CompanySummaryDto
            {
                Id = c.Id,
                Name = c.Name,
                Abbreviation = c.Abbreviation,
                Domain = c.Domain,
                Currency = c.Currency,
                Country = c.Country,
                IsGroup = c.IsGroup,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            }).ToList();
        }

        return dto;
    }
}

/// <summary>
/// Validator for UpdateCompanyCommand
/// </summary>
public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("Company ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Company name is required")
            .MaximumLength(200)
            .WithMessage("Company name cannot exceed 200 characters");

        RuleFor(x => x.Abbreviation)
            .NotEmpty()
            .WithMessage("Company abbreviation is required")
            .MaximumLength(10)
            .WithMessage("Company abbreviation cannot exceed 10 characters");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be a 3-letter code (e.g., USD, EUR, ZAR)");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required")
            .MaximumLength(100)
            .WithMessage("Country cannot exceed 100 characters");

        RuleFor(x => x.FiscalYearStartMonth)
            .InclusiveBetween(1, 12)
            .WithMessage("Fiscal year start month must be between 1 and 12");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Valid email address is required");
    }
}

/// <summary>
/// Command to delete a company
/// </summary>
public record DeleteCompanyCommand(Guid CompanyId) : IRequest<bool>;

/// <summary>
/// Handler for deleting companies
/// </summary>
public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, bool>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteCompanyCommandHandler> _logger;

    public DeleteCompanyCommandHandler(
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<DeleteCompanyCommandHandler> logger)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting company: {CompanyId}", request.CompanyId);

        var company = await _companyRepository.GetByIdAsync(request.CompanyId, cancellationToken);
        if (company == null)
        {
            return false;
        }

        // Check if company has child companies
        var childCompanies = await _companyRepository.GetChildCompaniesAsync(company.Id, cancellationToken);
        if (childCompanies.Any())
        {
            throw new InvalidOperationException("Cannot delete company with subsidiaries. Please delete or reassign subsidiaries first.");
        }

        // Soft delete by deactivating
        var currentUserId = _currentUserService.UserId ?? "system";
        company.Deactivate(currentUserId);

        await _companyRepository.UpdateAsync(company, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(company.DomainEvents, cancellationToken);
        company.ClearDomainEvents();

        _logger.LogInformation("Successfully deleted company {CompanyId}", request.CompanyId);

        return true;
    }
}

/// <summary>
/// Command to change company status
/// </summary>
public record ChangeCompanyStatusCommand(Guid CompanyId, bool IsActive) : IRequest<CompanyDto>;

/// <summary>
/// Handler for changing company status
/// </summary>
public class ChangeCompanyStatusCommandHandler : IRequestHandler<ChangeCompanyStatusCommand, CompanyDto>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ChangeCompanyStatusCommandHandler> _logger;

    public ChangeCompanyStatusCommandHandler(
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<ChangeCompanyStatusCommandHandler> logger)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CompanyDto> Handle(ChangeCompanyStatusCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changing company {CompanyId} status to {IsActive}", request.CompanyId, request.IsActive);

        var company = await _companyRepository.GetByIdAsync(request.CompanyId, cancellationToken);
        if (company == null)
        {
            throw new InvalidOperationException($"Company with ID {request.CompanyId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        if (request.IsActive)
        {
            company.Activate(currentUserId);
        }
        else
        {
            company.Deactivate(currentUserId);
        }

        await _companyRepository.UpdateAsync(company, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(company.DomainEvents, cancellationToken);
        company.ClearDomainEvents();

        _logger.LogInformation("Successfully changed company {CompanyId} status to {IsActive}", request.CompanyId, request.IsActive);

        return await MapToDtoAsync(company, cancellationToken);
    }

    private async Task<CompanyDto> MapToDtoAsync(Company company, CancellationToken cancellationToken)
    {
        var dto = new CompanyDto
        {
            Id = company.Id,
            TenantId = company.TenantId,
            Name = company.Name,
            Abbreviation = company.Abbreviation,
            Domain = company.Domain,
            Currency = company.Currency,
            Country = company.Country,
            TaxId = company.TaxId,
            ParentCompanyId = company.ParentCompanyId,
            IsGroup = company.IsGroup,
            IsActive = company.IsActive,
            Description = company.Description,
            Logo = company.Logo,
            RegistrationDate = company.RegistrationDate,
            FiscalYearStartMonth = company.FiscalYearStartMonth,
            DateOfIncorporation = company.DateOfIncorporation,
            DateOfCommencement = company.DateOfCommencement,
            RegistrationNumber = company.RegistrationNumber,
            Website = company.Website,
            Phone = company.Phone,
            Email = company.Email,
            Address = company.Address,
            City = company.City,
            State = company.State,
            PostalCode = company.PostalCode,
            CreatedAt = company.CreatedAt,
            ModifiedAt = company.ModifiedAt,
            CreatedBy = company.CreatedBy,
            ModifiedBy = company.ModifiedBy
        };

        // Load parent company name if exists
        if (company.ParentCompanyId.HasValue)
        {
            var parentCompany = await _companyRepository.GetByIdAsync(company.ParentCompanyId.Value, cancellationToken);
            dto.ParentCompanyName = parentCompany?.Name;
        }

        return dto;
    }
}
