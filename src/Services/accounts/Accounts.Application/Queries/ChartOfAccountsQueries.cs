using TossErp.Accounts.Domain.Aggregates;
using TossErp.Accounts.Domain.Enums;
using TossErp.Accounts.Application.DTOs;

namespace TossErp.Accounts.Application.Queries;

/// <summary>
/// Query to get chart of accounts with filtering and pagination
/// </summary>
public record GetChartOfAccountsQuery(
    AccountType? AccountType = null,
    Guid? ParentAccountId = null,
    bool? IsActive = null,
    string? SearchTerm = null,
    int PageNumber = 1,
    int PageSize = 50,
    string SortBy = "AccountCode",
    bool SortDescending = false
) : IRequest<PaginatedResult<ChartOfAccountsDto>>;

/// <summary>
/// Handler for getting chart of accounts
/// </summary>
public class GetChartOfAccountsQueryHandler : IRequestHandler<GetChartOfAccountsQuery, PaginatedResult<ChartOfAccountsDto>>
{
    private readonly IChartOfAccountsRepository _chartOfAccountsRepository;
    private readonly ILogger<GetChartOfAccountsQueryHandler> _logger;

    public GetChartOfAccountsQueryHandler(
        IChartOfAccountsRepository chartOfAccountsRepository,
        ILogger<GetChartOfAccountsQueryHandler> logger)
    {
        _chartOfAccountsRepository = chartOfAccountsRepository;
        _logger = logger;
    }

    public async Task<PaginatedResult<ChartOfAccountsDto>> Handle(GetChartOfAccountsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting chart of accounts with account type: {AccountType}, Active: {IsActive}", 
            request.AccountType, request.IsActive);

        var filter = new ChartOfAccountsFilter
        {
            AccountType = request.AccountType,
            ParentAccountId = request.ParentAccountId,
            IsActive = request.IsActive,
            SearchTerm = request.SearchTerm
        };

        var accounts = await _chartOfAccountsRepository.GetPagedAsync(
            filter,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDescending,
            cancellationToken);

        // Get parent account information for mapping
        var parentAccountIds = accounts.Items
            .Where(a => a.ParentAccountId.HasValue)
            .Select(a => a.ParentAccountId!.Value)
            .Distinct()
            .ToList();

        var parentAccounts = new Dictionary<Guid, ChartOfAccounts>();
        foreach (var parentId in parentAccountIds)
        {
            var parent = await _chartOfAccountsRepository.GetByIdAsync(parentId, cancellationToken);
            if (parent != null)
            {
                parentAccounts[parentId] = parent;
            }
        }

        var accountDtos = accounts.Items.Select(account => MapToDto(account, parentAccounts.GetValueOrDefault(account.ParentAccountId ?? Guid.Empty))).ToList();

        return new PaginatedResult<ChartOfAccountsDto>
        {
            Items = accountDtos,
            TotalCount = accounts.TotalCount,
            PageNumber = accounts.PageNumber,
            PageSize = accounts.PageSize,
            TotalPages = accounts.TotalPages
        };
    }

    private static ChartOfAccountsDto MapToDto(ChartOfAccounts chartOfAccounts, ChartOfAccounts? parentAccount)
    {
        return new ChartOfAccountsDto
        {
            Id = chartOfAccounts.Id,
            TenantId = chartOfAccounts.TenantId,
            AccountCode = chartOfAccounts.AccountCode,
            AccountName = chartOfAccounts.AccountName,
            AccountType = chartOfAccounts.AccountType,
            ParentAccountId = chartOfAccounts.ParentAccountId,
            ParentAccountCode = parentAccount?.AccountCode,
            ParentAccountName = parentAccount?.AccountName,
            Description = chartOfAccounts.Description,
            IsActive = chartOfAccounts.IsActive,
            CurrentBalance = chartOfAccounts.CurrentBalance.Amount,
            CreatedAt = chartOfAccounts.CreatedAt,
            CreatedBy = chartOfAccounts.CreatedBy,
            LastModified = chartOfAccounts.LastModified,
            LastModifiedBy = chartOfAccounts.LastModifiedBy
        };
    }
}

/// <summary>
/// Query to get a chart of accounts entry by ID
/// </summary>
public record GetChartOfAccountsByIdQuery(Guid AccountId) : IRequest<ChartOfAccountsDto?>;

/// <summary>
/// Handler for getting a chart of accounts entry by ID
/// </summary>
public class GetChartOfAccountsByIdQueryHandler : IRequestHandler<GetChartOfAccountsByIdQuery, ChartOfAccountsDto?>
{
    private readonly IChartOfAccountsRepository _chartOfAccountsRepository;
    private readonly ILogger<GetChartOfAccountsByIdQueryHandler> _logger;

    public GetChartOfAccountsByIdQueryHandler(
        IChartOfAccountsRepository chartOfAccountsRepository,
        ILogger<GetChartOfAccountsByIdQueryHandler> logger)
    {
        _chartOfAccountsRepository = chartOfAccountsRepository;
        _logger = logger;
    }

    public async Task<ChartOfAccountsDto?> Handle(GetChartOfAccountsByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting chart of accounts entry by ID: {AccountId}", request.AccountId);

        var account = await _chartOfAccountsRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account == null)
        {
            return null;
        }

        // Get parent account if exists
        ChartOfAccounts? parentAccount = null;
        if (account.ParentAccountId.HasValue)
        {
            parentAccount = await _chartOfAccountsRepository.GetByIdAsync(account.ParentAccountId.Value, cancellationToken);
        }

        return MapToDto(account, parentAccount);
    }

    private static ChartOfAccountsDto MapToDto(ChartOfAccounts chartOfAccounts, ChartOfAccounts? parentAccount)
    {
        return new ChartOfAccountsDto
        {
            Id = chartOfAccounts.Id,
            TenantId = chartOfAccounts.TenantId,
            AccountCode = chartOfAccounts.AccountCode,
            AccountName = chartOfAccounts.AccountName,
            AccountType = chartOfAccounts.AccountType,
            ParentAccountId = chartOfAccounts.ParentAccountId,
            ParentAccountCode = parentAccount?.AccountCode,
            ParentAccountName = parentAccount?.AccountName,
            Description = chartOfAccounts.Description,
            IsActive = chartOfAccounts.IsActive,
            CurrentBalance = chartOfAccounts.CurrentBalance.Amount,
            CreatedAt = chartOfAccounts.CreatedAt,
            CreatedBy = chartOfAccounts.CreatedBy,
            LastModified = chartOfAccounts.LastModified,
            LastModifiedBy = chartOfAccounts.LastModifiedBy
        };
    }
}

/// <summary>
/// Query to get chart of accounts hierarchy
/// </summary>
public record GetChartOfAccountsHierarchyQuery(
    AccountType? RootAccountType = null
) : IRequest<List<ChartOfAccountsHierarchyDto>>;

/// <summary>
/// DTO for chart of accounts hierarchy
/// </summary>
public class ChartOfAccountsHierarchyDto
{
    public Guid Id { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public decimal CurrentBalance { get; set; }
    public bool IsActive { get; set; }
    public int Level { get; set; }
    public List<ChartOfAccountsHierarchyDto> Children { get; set; } = new();
}

/// <summary>
/// Handler for getting chart of accounts hierarchy
/// </summary>
public class GetChartOfAccountsHierarchyQueryHandler : IRequestHandler<GetChartOfAccountsHierarchyQuery, List<ChartOfAccountsHierarchyDto>>
{
    private readonly IChartOfAccountsRepository _chartOfAccountsRepository;
    private readonly ILogger<GetChartOfAccountsHierarchyQueryHandler> _logger;

    public GetChartOfAccountsHierarchyQueryHandler(
        IChartOfAccountsRepository chartOfAccountsRepository,
        ILogger<GetChartOfAccountsHierarchyQueryHandler> logger)
    {
        _chartOfAccountsRepository = chartOfAccountsRepository;
        _logger = logger;
    }

    public async Task<List<ChartOfAccountsHierarchyDto>> Handle(GetChartOfAccountsHierarchyQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting chart of accounts hierarchy for account type: {AccountType}", request.RootAccountType);

        var allAccounts = await _chartOfAccountsRepository.GetAllAsync(cancellationToken);
        
        // Filter by root account type if specified
        if (request.RootAccountType.HasValue)
        {
            allAccounts = allAccounts.Where(a => a.AccountType == request.RootAccountType.Value || 
                                                 IsChildOfAccountType(a, allAccounts, request.RootAccountType.Value)).ToList();
        }

        // Build hierarchy starting from root accounts (no parent)
        var rootAccounts = allAccounts.Where(a => a.ParentAccountId == null).ToList();
        var hierarchy = new List<ChartOfAccountsHierarchyDto>();

        foreach (var rootAccount in rootAccounts.OrderBy(a => a.AccountCode))
        {
            var hierarchyDto = BuildHierarchy(rootAccount, allAccounts, 0);
            hierarchy.Add(hierarchyDto);
        }

        return hierarchy;
    }

    private static bool IsChildOfAccountType(ChartOfAccounts account, IEnumerable<ChartOfAccounts> allAccounts, AccountType targetType)
    {
        var current = account;
        while (current.ParentAccountId.HasValue)
        {
            current = allAccounts.FirstOrDefault(a => a.Id == current.ParentAccountId.Value);
            if (current == null) break;
            if (current.AccountType == targetType) return true;
        }
        return false;
    }

    private static ChartOfAccountsHierarchyDto BuildHierarchy(ChartOfAccounts account, IEnumerable<ChartOfAccounts> allAccounts, int level)
    {
        var dto = new ChartOfAccountsHierarchyDto
        {
            Id = account.Id,
            AccountCode = account.AccountCode,
            AccountName = account.AccountName,
            AccountType = account.AccountType,
            CurrentBalance = account.CurrentBalance.Amount,
            IsActive = account.IsActive,
            Level = level
        };

        // Get child accounts
        var children = allAccounts.Where(a => a.ParentAccountId == account.Id).OrderBy(a => a.AccountCode);
        foreach (var child in children)
        {
            var childDto = BuildHierarchy(child, allAccounts, level + 1);
            dto.Children.Add(childDto);
        }

        return dto;
    }
}
