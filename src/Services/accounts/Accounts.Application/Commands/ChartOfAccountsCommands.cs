using FluentValidation;

namespace TossErp.Accounts.Application.Commands;

/// <summary>
/// Command to create a chart of accounts entry
/// </summary>
public record CreateChartOfAccountsCommand(
    string AccountCode,
    string AccountName,
    AccountType AccountType,
    Guid? ParentAccountId,
    string? Description,
    bool IsActive = true
) : IRequest<ChartOfAccountsDto>;

/// <summary>
/// Handler for creating chart of accounts entries
/// </summary>
public class CreateChartOfAccountsCommandHandler : IRequestHandler<CreateChartOfAccountsCommand, ChartOfAccountsDto>
{
    private readonly IChartOfAccountsRepository _chartOfAccountsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateChartOfAccountsCommandHandler> _logger;

    public CreateChartOfAccountsCommandHandler(
        IChartOfAccountsRepository chartOfAccountsRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<CreateChartOfAccountsCommandHandler> logger)
    {
        _chartOfAccountsRepository = chartOfAccountsRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ChartOfAccountsDto> Handle(CreateChartOfAccountsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating chart of accounts entry: {AccountCode} - {AccountName}", 
            request.AccountCode, request.AccountName);

        var tenantId = _currentUserService.TenantId ?? throw new InvalidOperationException("Tenant ID is required");
        var currentUserId = _currentUserService.UserId ?? "system";

        // Check if account code already exists
        var existingAccount = await _chartOfAccountsRepository.GetByAccountCodeAsync(request.AccountCode, cancellationToken);
        if (existingAccount != null)
        {
            throw new InvalidOperationException($"Account with code '{request.AccountCode}' already exists");
        }

        // Validate parent account if specified
        ChartOfAccounts? parentAccount = null;
        if (request.ParentAccountId.HasValue)
        {
            parentAccount = await _chartOfAccountsRepository.GetByIdAsync(request.ParentAccountId.Value, cancellationToken);
            if (parentAccount == null)
            {
                throw new InvalidOperationException($"Parent account with ID {request.ParentAccountId} not found");
            }

            // Validate parent account type compatibility
            if (!IsValidParentAccountType(request.AccountType, parentAccount.AccountType))
            {
                throw new InvalidOperationException(
                    $"Account type {request.AccountType} cannot have parent of type {parentAccount.AccountType}");
            }
        }

        // Create chart of accounts entry
        var chartOfAccounts = ChartOfAccounts.Create(
            tenantId: tenantId,
            accountCode: request.AccountCode,
            accountName: request.AccountName,
            accountType: request.AccountType,
            parentAccountId: request.ParentAccountId,
            description: request.Description,
            isActive: request.IsActive,
            createdBy: currentUserId);

        // Save entry
        await _chartOfAccountsRepository.AddAsync(chartOfAccounts, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(chartOfAccounts.DomainEvents, cancellationToken);
        chartOfAccounts.ClearDomainEvents();

        _logger.LogInformation("Successfully created chart of accounts entry {ChartOfAccountsId} with code {AccountCode}", 
            chartOfAccounts.Id, request.AccountCode);

        return MapToDto(chartOfAccounts, parentAccount);
    }

    private static bool IsValidParentAccountType(AccountType childType, AccountType parentType)
    {
        // Define valid parent-child relationships
        return (childType, parentType) switch
        {
            // Assets can have Asset parents
            (AccountType.CurrentAssets, AccountType.Assets) => true,
            (AccountType.FixedAssets, AccountType.Assets) => true,
            (AccountType.OtherAssets, AccountType.Assets) => true,
            
            // Liabilities can have Liability parents
            (AccountType.CurrentLiabilities, AccountType.Liabilities) => true,
            (AccountType.LongTermLiabilities, AccountType.Liabilities) => true,
            
            // Equity accounts can have Equity parents
            (AccountType.Equity, AccountType.Equity) => true,
            
            // Income accounts can have Income parents
            (AccountType.Revenue, AccountType.Income) => true,
            (AccountType.OtherIncome, AccountType.Income) => true,
            
            // Expense accounts can have Expense parents
            (AccountType.CostOfGoodsSold, AccountType.Expenses) => true,
            (AccountType.OperatingExpenses, AccountType.Expenses) => true,
            (AccountType.OtherExpenses, AccountType.Expenses) => true,
            
            // Same type relationships
            _ when childType == parentType => true,
            
            _ => false
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
/// Validator for CreateChartOfAccountsCommand
/// </summary>
public class CreateChartOfAccountsCommandValidator : AbstractValidator<CreateChartOfAccountsCommand>
{
    public CreateChartOfAccountsCommandValidator()
    {
        RuleFor(x => x.AccountCode)
            .NotEmpty()
            .WithMessage("Account code is required")
            .MaximumLength(20)
            .WithMessage("Account code cannot exceed 20 characters")
            .Matches(@"^[A-Z0-9\-\.]+$")
            .WithMessage("Account code can only contain uppercase letters, numbers, hyphens, and periods");

        RuleFor(x => x.AccountName)
            .NotEmpty()
            .WithMessage("Account name is required")
            .MaximumLength(100)
            .WithMessage("Account name cannot exceed 100 characters");

        RuleFor(x => x.AccountType)
            .IsInEnum()
            .WithMessage("Valid account type is required");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 500 characters");
    }
}

/// <summary>
/// Command to update a chart of accounts entry
/// </summary>
public record UpdateChartOfAccountsCommand(
    Guid AccountId,
    string AccountName,
    string? Description,
    bool IsActive
) : IRequest<ChartOfAccountsDto>;

/// <summary>
/// Handler for updating chart of accounts entries
/// </summary>
public class UpdateChartOfAccountsCommandHandler : IRequestHandler<UpdateChartOfAccountsCommand, ChartOfAccountsDto>
{
    private readonly IChartOfAccountsRepository _chartOfAccountsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateChartOfAccountsCommandHandler> _logger;

    public UpdateChartOfAccountsCommandHandler(
        IChartOfAccountsRepository chartOfAccountsRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<UpdateChartOfAccountsCommandHandler> logger)
    {
        _chartOfAccountsRepository = chartOfAccountsRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ChartOfAccountsDto> Handle(UpdateChartOfAccountsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating chart of accounts entry: {AccountId}", request.AccountId);

        var chartOfAccounts = await _chartOfAccountsRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (chartOfAccounts == null)
        {
            throw new InvalidOperationException($"Chart of accounts entry with ID {request.AccountId} not found");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Get parent account for DTO mapping
        ChartOfAccounts? parentAccount = null;
        if (chartOfAccounts.ParentAccountId.HasValue)
        {
            parentAccount = await _chartOfAccountsRepository.GetByIdAsync(chartOfAccounts.ParentAccountId.Value, cancellationToken);
        }

        // Update account
        chartOfAccounts.UpdateAccount(
            accountName: request.AccountName,
            description: request.Description,
            isActive: request.IsActive,
            updatedBy: currentUserId);

        // Save changes
        await _chartOfAccountsRepository.UpdateAsync(chartOfAccounts, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(chartOfAccounts.DomainEvents, cancellationToken);
        chartOfAccounts.ClearDomainEvents();

        _logger.LogInformation("Successfully updated chart of accounts entry {AccountId}", request.AccountId);

        return MapToDto(chartOfAccounts, parentAccount);
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
/// Validator for UpdateChartOfAccountsCommand
/// </summary>
public class UpdateChartOfAccountsCommandValidator : AbstractValidator<UpdateChartOfAccountsCommand>
{
    public UpdateChartOfAccountsCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID is required");

        RuleFor(x => x.AccountName)
            .NotEmpty()
            .WithMessage("Account name is required")
            .MaximumLength(100)
            .WithMessage("Account name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 500 characters");
    }
}

/// <summary>
/// Command to deactivate a chart of accounts entry
/// </summary>
public record DeactivateChartOfAccountsCommand(
    Guid AccountId,
    string Reason
) : IRequest<ChartOfAccountsDto>;

/// <summary>
/// Handler for deactivating chart of accounts entries
/// </summary>
public class DeactivateChartOfAccountsCommandHandler : IRequestHandler<DeactivateChartOfAccountsCommand, ChartOfAccountsDto>
{
    private readonly IChartOfAccountsRepository _chartOfAccountsRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeactivateChartOfAccountsCommandHandler> _logger;

    public DeactivateChartOfAccountsCommandHandler(
        IChartOfAccountsRepository chartOfAccountsRepository,
        IJournalEntryRepository journalEntryRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<DeactivateChartOfAccountsCommandHandler> logger)
    {
        _chartOfAccountsRepository = chartOfAccountsRepository;
        _journalEntryRepository = journalEntryRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<ChartOfAccountsDto> Handle(DeactivateChartOfAccountsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deactivating chart of accounts entry: {AccountId}", request.AccountId);

        var chartOfAccounts = await _chartOfAccountsRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (chartOfAccounts == null)
        {
            throw new InvalidOperationException($"Chart of accounts entry with ID {request.AccountId} not found");
        }

        if (!chartOfAccounts.IsActive)
        {
            throw new InvalidOperationException($"Account {chartOfAccounts.AccountCode} is already inactive");
        }

        // Check if account has a non-zero balance
        if (chartOfAccounts.CurrentBalance.Amount != 0)
        {
            throw new InvalidOperationException(
                $"Cannot deactivate account {chartOfAccounts.AccountCode} with non-zero balance {chartOfAccounts.CurrentBalance.Amount}");
        }

        // Check if account has child accounts
        var childAccounts = await _chartOfAccountsRepository.GetChildAccountsAsync(request.AccountId, cancellationToken);
        if (childAccounts.Any(c => c.IsActive))
        {
            throw new InvalidOperationException(
                $"Cannot deactivate account {chartOfAccounts.AccountCode} while it has active child accounts");
        }

        // Check for recent transactions (last 30 days)
        var recentTransactions = await _journalEntryRepository.GetByAccountIdAsync(
            request.AccountId, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow, cancellationToken);
        
        if (recentTransactions.Any())
        {
            throw new InvalidOperationException(
                $"Cannot deactivate account {chartOfAccounts.AccountCode} with recent transactions in the last 30 days");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Get parent account for DTO mapping
        ChartOfAccounts? parentAccount = null;
        if (chartOfAccounts.ParentAccountId.HasValue)
        {
            parentAccount = await _chartOfAccountsRepository.GetByIdAsync(chartOfAccounts.ParentAccountId.Value, cancellationToken);
        }

        // Deactivate account
        chartOfAccounts.Deactivate(request.Reason, currentUserId);

        // Save changes
        await _chartOfAccountsRepository.UpdateAsync(chartOfAccounts, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(chartOfAccounts.DomainEvents, cancellationToken);
        chartOfAccounts.ClearDomainEvents();

        _logger.LogInformation("Successfully deactivated chart of accounts entry {AccountId}", request.AccountId);

        return MapToDto(chartOfAccounts, parentAccount);
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
/// Validator for DeactivateChartOfAccountsCommand
/// </summary>
public class DeactivateChartOfAccountsCommandValidator : AbstractValidator<DeactivateChartOfAccountsCommand>
{
    public DeactivateChartOfAccountsCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Deactivation reason is required")
            .MaximumLength(500)
            .WithMessage("Deactivation reason cannot exceed 500 characters");
    }
}

/// <summary>
/// Command to initialize default chart of accounts
/// </summary>
public record InitializeDefaultChartOfAccountsCommand(
    string CountryCode = "US"
) : IRequest<List<ChartOfAccountsDto>>;

/// <summary>
/// Handler for initializing default chart of accounts
/// </summary>
public class InitializeDefaultChartOfAccountsCommandHandler : IRequestHandler<InitializeDefaultChartOfAccountsCommand, List<ChartOfAccountsDto>>
{
    private readonly IChartOfAccountsRepository _chartOfAccountsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<InitializeDefaultChartOfAccountsCommandHandler> _logger;

    public InitializeDefaultChartOfAccountsCommandHandler(
        IChartOfAccountsRepository chartOfAccountsRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<InitializeDefaultChartOfAccountsCommandHandler> logger)
    {
        _chartOfAccountsRepository = chartOfAccountsRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<List<ChartOfAccountsDto>> Handle(InitializeDefaultChartOfAccountsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing default chart of accounts for country: {CountryCode}", request.CountryCode);

        var tenantId = _currentUserService.TenantId ?? throw new InvalidOperationException("Tenant ID is required");
        var currentUserId = _currentUserService.UserId ?? "system";

        // Check if chart of accounts already exists
        var existingAccounts = await _chartOfAccountsRepository.GetAllAsync(cancellationToken);
        if (existingAccounts.Any())
        {
            throw new InvalidOperationException("Chart of accounts already exists. Cannot initialize default accounts.");
        }

        var defaultAccounts = GetDefaultAccounts(request.CountryCode);
        var createdAccounts = new List<ChartOfAccounts>();

        // Create accounts in dependency order (parents first)
        foreach (var accountData in defaultAccounts.OrderBy(a => a.Level))
        {
            var parentAccount = accountData.ParentCode != null 
                ? createdAccounts.FirstOrDefault(a => a.AccountCode == accountData.ParentCode)
                : null;

            var account = ChartOfAccounts.Create(
                tenantId: tenantId,
                accountCode: accountData.Code,
                accountName: accountData.Name,
                accountType: accountData.Type,
                parentAccountId: parentAccount?.Id,
                description: accountData.Description,
                isActive: true,
                createdBy: currentUserId);

            await _chartOfAccountsRepository.AddAsync(account, cancellationToken);
            createdAccounts.Add(account);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        foreach (var account in createdAccounts)
        {
            await _domainEventService.PublishAsync(account.DomainEvents, cancellationToken);
            account.ClearDomainEvents();
        }

        _logger.LogInformation("Successfully initialized {Count} default chart of accounts entries", createdAccounts.Count);

        return createdAccounts.Select(account => MapToDto(account, null)).ToList();
    }

    private static List<DefaultAccountData> GetDefaultAccounts(string countryCode)
    {
        // Default US chart of accounts structure
        return new List<DefaultAccountData>
        {
            // Assets
            new("1000", "Assets", AccountType.Assets, null, "All company assets", 1),
            new("1100", "Current Assets", AccountType.CurrentAssets, "1000", "Assets expected to be converted to cash within one year", 2),
            new("1110", "Cash and Cash Equivalents", AccountType.CurrentAssets, "1100", "Cash on hand and in bank accounts", 3),
            new("1120", "Accounts Receivable", AccountType.CurrentAssets, "1100", "Money owed by customers", 3),
            new("1130", "Inventory", AccountType.CurrentAssets, "1100", "Goods held for sale", 3),
            new("1140", "Prepaid Expenses", AccountType.CurrentAssets, "1100", "Expenses paid in advance", 3),
            new("1200", "Fixed Assets", AccountType.FixedAssets, "1000", "Long-term tangible assets", 2),
            new("1210", "Property, Plant & Equipment", AccountType.FixedAssets, "1200", "Buildings, equipment, and vehicles", 3),
            new("1220", "Accumulated Depreciation", AccountType.FixedAssets, "1200", "Accumulated depreciation on fixed assets", 3),

            // Liabilities
            new("2000", "Liabilities", AccountType.Liabilities, null, "All company liabilities", 1),
            new("2100", "Current Liabilities", AccountType.CurrentLiabilities, "2000", "Debts due within one year", 2),
            new("2110", "Accounts Payable", AccountType.CurrentLiabilities, "2100", "Money owed to suppliers", 3),
            new("2120", "Accrued Expenses", AccountType.CurrentLiabilities, "2100", "Expenses incurred but not yet paid", 3),
            new("2130", "Short-term Debt", AccountType.CurrentLiabilities, "2100", "Loans and debt due within one year", 3),
            new("2200", "Long-term Liabilities", AccountType.LongTermLiabilities, "2000", "Debts due after one year", 2),
            new("2210", "Long-term Debt", AccountType.LongTermLiabilities, "2200", "Loans and debt due after one year", 3),

            // Equity
            new("3000", "Equity", AccountType.Equity, null, "Owner's equity in the company", 1),
            new("3100", "Owner's Equity", AccountType.Equity, "3000", "Owner's investment and retained earnings", 2),
            new("3110", "Retained Earnings", AccountType.Equity, "3100", "Accumulated profits retained in the business", 3),

            // Income
            new("4000", "Income", AccountType.Income, null, "All company income", 1),
            new("4100", "Revenue", AccountType.Revenue, "4000", "Income from primary business activities", 2),
            new("4110", "Sales Revenue", AccountType.Revenue, "4100", "Revenue from sales of goods or services", 3),
            new("4200", "Other Income", AccountType.OtherIncome, "4000", "Income from non-primary activities", 2),
            new("4210", "Interest Income", AccountType.OtherIncome, "4200", "Income from interest on investments", 3),

            // Expenses
            new("5000", "Expenses", AccountType.Expenses, null, "All company expenses", 1),
            new("5100", "Cost of Goods Sold", AccountType.CostOfGoodsSold, "5000", "Direct costs of producing goods or services", 2),
            new("5110", "Materials and Supplies", AccountType.CostOfGoodsSold, "5100", "Cost of materials used in production", 3),
            new("5200", "Operating Expenses", AccountType.OperatingExpenses, "5000", "Expenses from normal business operations", 2),
            new("5210", "Salaries and Wages", AccountType.OperatingExpenses, "5200", "Employee compensation", 3),
            new("5220", "Rent Expense", AccountType.OperatingExpenses, "5200", "Cost of renting facilities", 3),
            new("5230", "Utilities", AccountType.OperatingExpenses, "5200", "Electricity, water, gas, internet", 3),
            new("5240", "Insurance", AccountType.OperatingExpenses, "5200", "Business insurance premiums", 3),
            new("5300", "Other Expenses", AccountType.OtherExpenses, "5000", "Non-operating expenses", 2),
            new("5310", "Interest Expense", AccountType.OtherExpenses, "5300", "Interest paid on loans and debt", 3)
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

    private record DefaultAccountData(
        string Code,
        string Name,
        AccountType Type,
        string? ParentCode,
        string? Description,
        int Level);
}

/// <summary>
/// Validator for InitializeDefaultChartOfAccountsCommand
/// </summary>
public class InitializeDefaultChartOfAccountsCommandValidator : AbstractValidator<InitializeDefaultChartOfAccountsCommand>
{
    public InitializeDefaultChartOfAccountsCommandValidator()
    {
        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .WithMessage("Country code is required")
            .Length(2)
            .WithMessage("Country code must be 2 characters")
            .Matches(@"^[A-Z]{2}$")
            .WithMessage("Country code must be uppercase letters");
    }
}
