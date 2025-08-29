using MediatR;
using TossErp.Accounts.Application.DTOs;
using TossErp.Accounts.Domain.Entities;

namespace TossErp.Accounts.Application.Queries;

/// <summary>
/// Query to get companies with optional filtering
/// </summary>
public class GetCompaniesQuery : IRequest<PagedResult<CompanySummaryDto>>
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsGroup { get; set; }
    public string? Currency { get; set; }
    public string? Country { get; set; }
    public Guid? ParentCompanyId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

/// <summary>
/// Handler for getting companies
/// </summary>
public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, PagedResult<CompanySummaryDto>>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetCompaniesQueryHandler> _logger;

    public GetCompaniesQueryHandler(
        ICompanyRepository companyRepository,
        ICurrentUserService currentUserService,
        ILogger<GetCompaniesQueryHandler> logger)
    {
        _companyRepository = companyRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<PagedResult<CompanySummaryDto>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting companies with search term: {SearchTerm}", request.SearchTerm);

        var tenantId = _currentUserService.TenantId ?? throw new InvalidOperationException("Tenant ID is required");

        var (companies, totalCount) = await _companyRepository.GetPagedAsync(
            tenantId: tenantId,
            searchTerm: request.SearchTerm,
            isActive: request.IsActive,
            isGroup: request.IsGroup,
            currency: request.Currency,
            country: request.Country,
            parentCompanyId: request.ParentCompanyId,
            page: request.Page,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        var companySummaries = companies.Select(c => new CompanySummaryDto
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

        return new PagedResult<CompanySummaryDto>
        {
            Items = companySummaries,
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }
}

/// <summary>
/// Query to get a specific company by ID
/// </summary>
public class GetCompanyQuery : IRequest<CompanyDto?>
{
    public Guid Id { get; set; }
}

/// <summary>
/// Handler for getting a company by ID
/// </summary>
public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, CompanyDto?>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetCompanyQueryHandler> _logger;

    public GetCompanyQueryHandler(
        ICompanyRepository companyRepository,
        ICurrentUserService currentUserService,
        ILogger<GetCompanyQueryHandler> logger)
    {
        _companyRepository = companyRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CompanyDto?> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting company: {CompanyId}", request.Id);

        var company = await _companyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (company == null)
        {
            return null;
        }

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
/// Query to get all group companies (companies that can have subsidiaries)
/// </summary>
public class GetGroupCompaniesQuery : IRequest<List<CompanySummaryDto>>
{
    public bool? IsActive { get; set; } = true;
}

/// <summary>
/// Handler for getting group companies
/// </summary>
public class GetGroupCompaniesQueryHandler : IRequestHandler<GetGroupCompaniesQuery, List<CompanySummaryDto>>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetGroupCompaniesQueryHandler> _logger;

    public GetGroupCompaniesQueryHandler(
        ICompanyRepository companyRepository,
        ICurrentUserService currentUserService,
        ILogger<GetGroupCompaniesQueryHandler> logger)
    {
        _companyRepository = companyRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<List<CompanySummaryDto>> Handle(GetGroupCompaniesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting group companies");

        var tenantId = _currentUserService.TenantId ?? throw new InvalidOperationException("Tenant ID is required");

        var companies = await _companyRepository.GetGroupCompaniesAsync(tenantId, request.IsActive, cancellationToken);

        return companies.Select(c => new CompanySummaryDto
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
}

/// <summary>
/// Query to get child companies of a parent company
/// </summary>
public class GetChildCompaniesQuery : IRequest<List<CompanySummaryDto>>
{
    public Guid ParentCompanyId { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// Handler for getting child companies
/// </summary>
public class GetChildCompaniesQueryHandler : IRequestHandler<GetChildCompaniesQuery, List<CompanySummaryDto>>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger<GetChildCompaniesQueryHandler> _logger;

    public GetChildCompaniesQueryHandler(
        ICompanyRepository companyRepository,
        ILogger<GetChildCompaniesQueryHandler> logger)
    {
        _companyRepository = companyRepository;
        _logger = logger;
    }

    public async Task<List<CompanySummaryDto>> Handle(GetChildCompaniesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting child companies for parent: {ParentCompanyId}", request.ParentCompanyId);

        var companies = await _companyRepository.GetChildCompaniesAsync(request.ParentCompanyId, cancellationToken);

        if (request.IsActive.HasValue)
        {
            companies = companies.Where(c => c.IsActive == request.IsActive.Value).ToList();
        }

        return companies.Select(c => new CompanySummaryDto
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
}

/// <summary>
/// Query to get company statistics
/// </summary>
public class GetCompanyStatisticsQuery : IRequest<CompanyStatisticsDto>
{
}

/// <summary>
/// Company statistics DTO
/// </summary>
public class CompanyStatisticsDto
{
    public int TotalCompanies { get; set; }
    public int ActiveCompanies { get; set; }
    public int InactiveCompanies { get; set; }
    public int GroupCompanies { get; set; }
    public int SubsidiaryCompanies { get; set; }
    public List<CountryStatisticDto> CompaniesByCountry { get; set; } = new();
    public List<CurrencyStatisticDto> CompaniesByCurrency { get; set; } = new();
}

/// <summary>
/// Country statistic DTO
/// </summary>
public class CountryStatisticDto
{
    public string Country { get; set; } = string.Empty;
    public int CustomerCount { get; set; }
}

/// <summary>
/// Currency statistic DTO
/// </summary>
public class CurrencyStatisticDto
{
    public string Currency { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>
/// Handler for getting company statistics
/// </summary>
public class GetCompanyStatisticsQueryHandler : IRequestHandler<GetCompanyStatisticsQuery, CompanyStatisticsDto>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetCompanyStatisticsQueryHandler> _logger;

    public GetCompanyStatisticsQueryHandler(
        ICompanyRepository companyRepository,
        ICurrentUserService currentUserService,
        ILogger<GetCompanyStatisticsQueryHandler> logger)
    {
        _companyRepository = companyRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<CompanyStatisticsDto> Handle(GetCompanyStatisticsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting company statistics");

        var tenantId = _currentUserService.TenantId ?? throw new InvalidOperationException("Tenant ID is required");

        var companies = await _companyRepository.GetAllAsync(tenantId, cancellationToken);

        var statistics = new CompanyStatisticsDto
        {
            TotalCompanies = companies.Count(),
            ActiveCompanies = companies.Count(c => c.IsActive),
            InactiveCompanies = companies.Count(c => !c.IsActive),
            GroupCompanies = companies.Count(c => c.IsGroup),
            SubsidiaryCompanies = companies.Count(c => c.ParentCompanyId.HasValue),
            CompaniesByCountry = companies
                .GroupBy(c => c.Country)
                .Select(g => new CountryStatisticDto { Country = g.Key, CustomerCount = g.Count() })
                .OrderByDescending(x => x.CustomerCount)
                .ToList(),
            CompaniesByCurrency = companies
                .GroupBy(c => c.Currency)
                .Select(g => new CurrencyStatisticDto { Currency = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList()
        };

        return statistics;
    }
}
