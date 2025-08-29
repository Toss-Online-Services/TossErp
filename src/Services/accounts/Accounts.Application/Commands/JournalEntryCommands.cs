using FluentValidation;
using TossErp.Accounts.Domain.Aggregates;
using TossErp.Accounts.Domain.Enums;
using TossErp.Accounts.Domain.ValueObjects;

namespace TossErp.Accounts.Application.Commands;

/// <summary>
/// Command to create a journal entry
/// </summary>
public record CreateJournalEntryCommand(
    DateOnly EntryDate,
    string Reference,
    string Description,
    List<CreateJournalEntryLineDto> Lines,
    string? Notes
) : IRequest<JournalEntryDto>;

/// <summary>
/// DTO for creating journal entry lines
/// </summary>
public class CreateJournalEntryLineDto
{
    public Guid AccountId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Reference { get; set; }
}

/// <summary>
/// Handler for creating journal entries
/// </summary>
public class CreateJournalEntryCommandHandler : IRequestHandler<CreateJournalEntryCommand, JournalEntryDto>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IChartOfAccountsRepository _chartOfAccountsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateJournalEntryCommandHandler> _logger;

    public CreateJournalEntryCommandHandler(
        IJournalEntryRepository journalEntryRepository,
        IChartOfAccountsRepository chartOfAccountsRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<CreateJournalEntryCommandHandler> logger)
    {
        _journalEntryRepository = journalEntryRepository;
        _chartOfAccountsRepository = chartOfAccountsRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<JournalEntryDto> Handle(CreateJournalEntryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating journal entry with reference: {Reference}", request.Reference);

        var tenantId = _currentUserService.TenantId ?? throw new InvalidOperationException("Tenant ID is required");
        var currentUserId = _currentUserService.UserId ?? "system";

        // Validate accounts exist
        var accountIds = request.Lines.Select(l => l.AccountId).Distinct().ToList();
        var accounts = new Dictionary<Guid, ChartOfAccounts>();
        
        foreach (var accountId in accountIds)
        {
            var account = await _chartOfAccountsRepository.GetByIdAsync(accountId, cancellationToken);
            if (account == null)
            {
                throw new InvalidOperationException($"Account with ID {accountId} not found");
            }

            if (!account.IsActive)
            {
                throw new InvalidOperationException($"Account {account.AccountCode} - {account.AccountName} is inactive");
            }

            accounts[accountId] = account;
        }

        // Validate debits equal credits
        var totalDebits = request.Lines.Sum(l => l.DebitAmount);
        var totalCredits = request.Lines.Sum(l => l.CreditAmount);
        
        if (Math.Abs(totalDebits - totalCredits) > 0.01m)
        {
            throw new InvalidOperationException($"Journal entry is not balanced. Debits: {totalDebits}, Credits: {totalCredits}");
        }

        // Check reference uniqueness
        var existingEntry = await _journalEntryRepository.GetByReferenceAsync(request.Reference, cancellationToken);
        if (existingEntry != null)
        {
            throw new InvalidOperationException($"Journal entry with reference '{request.Reference}' already exists");
        }

        // Create journal entry lines
        var lines = request.Lines.Select(dto => CreateJournalEntryLine(dto, accounts[dto.AccountId])).ToList();

        // Create journal entry
        var journalEntry = JournalEntry.Create(
            tenantId: tenantId,
            entryDate: request.EntryDate.ToDateTime(TimeOnly.MinValue),
            referenceNumber: request.Reference,
            description: request.Description,
            lines: lines,
            notes: request.Notes,
            createdBy: currentUserId);

        // Save journal entry
        await _journalEntryRepository.AddAsync(journalEntry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(journalEntry.DomainEvents, cancellationToken);
        journalEntry.ClearDomainEvents();

        _logger.LogInformation("Successfully created journal entry {JournalEntryId} with reference {Reference}", 
            journalEntry.Id, request.Reference);

        return MapToDto(journalEntry, accounts);
    }

    private static JournalEntryLine CreateJournalEntryLine(CreateJournalEntryLineDto dto, ChartOfAccounts account)
    {
        return JournalEntryLine.Create(
            accountId: dto.AccountId,
            accountCode: account.AccountCode,
            accountName: account.AccountName,
            description: dto.Description,
            debitAmount: dto.DebitAmount,
            creditAmount: dto.CreditAmount,
            reference: dto.Reference);
    }

    private static JournalEntryDto MapToDto(JournalEntry journalEntry, Dictionary<Guid, ChartOfAccounts> accounts)
    {
        return new JournalEntryDto
        {
            Id = journalEntry.Id,
            TenantId = journalEntry.TenantId,
            EntryNumber = journalEntry.EntryNumber,
            EntryDate = journalEntry.EntryDate,
            Reference = journalEntry.Reference,
            Description = journalEntry.Description,
            Status = journalEntry.Status.ToString(),
            TotalDebitAmount = journalEntry.TotalDebitAmount.Amount,
            TotalCreditAmount = journalEntry.TotalCreditAmount.Amount,
            Lines = journalEntry.Lines.Select(MapLineToDto).ToList(),
            Notes = journalEntry.Notes,
            PostedAt = journalEntry.PostedAt,
            PostedBy = journalEntry.PostedBy,
            CreatedAt = DateOnly.FromDateTime(journalEntry.CreatedAt),
            CreatedBy = journalEntry.CreatedBy,
            LastModified = journalEntry.LastModified,
            LastModifiedBy = journalEntry.LastModifiedBy
        };
    }

    private static JournalEntryLineDto MapLineToDto(JournalEntryLine line)
    {
        return new JournalEntryLineDto
        {
            Id = line.Id,
            AccountId = line.AccountId,
            AccountCode = line.AccountCode,
            AccountName = line.AccountName,
            Description = line.Description,
            DebitAmount = line.DebitAmount?.Amount ?? 0,
            CreditAmount = line.CreditAmount?.Amount ?? 0,
            Reference = line.Reference
        };
    }
}

/// <summary>
/// Validator for CreateJournalEntryCommand
/// </summary>
public class CreateJournalEntryCommandValidator : AbstractValidator<CreateJournalEntryCommand>
{
    public CreateJournalEntryCommandValidator()
    {
        RuleFor(x => x.EntryDate)
            .NotEmpty()
            .WithMessage("Entry date is required");

        RuleFor(x => x.Reference)
            .NotEmpty()
            .WithMessage("Reference is required")
            .MaximumLength(50)
            .WithMessage("Reference cannot exceed 50 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("At least one journal entry line is required")
            .Must(lines => lines.Count >= 2)
            .WithMessage("Journal entry must have at least 2 lines");

        RuleForEach(x => x.Lines)
            .SetValidator(new CreateJournalEntryLineDtoValidator());

        RuleFor(x => x.Lines)
            .Must(ValidateBalancedEntry)
            .WithMessage("Journal entry must be balanced (debits must equal credits)");
    }

    private static bool ValidateBalancedEntry(List<CreateJournalEntryLineDto> lines)
    {
        var totalDebits = lines.Sum(l => l.DebitAmount);
        var totalCredits = lines.Sum(l => l.CreditAmount);
        return Math.Abs(totalDebits - totalCredits) <= 0.01m;
    }
}

/// <summary>
/// Validator for CreateJournalEntryLineDto
/// </summary>
public class CreateJournalEntryLineDtoValidator : AbstractValidator<CreateJournalEntryLineDto>
{
    public CreateJournalEntryLineDtoValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID is required");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Line description is required")
            .MaximumLength(500)
            .WithMessage("Line description cannot exceed 500 characters");

        RuleFor(x => x.DebitAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Debit amount cannot be negative");

        RuleFor(x => x.CreditAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Credit amount cannot be negative");

        RuleFor(x => x)
            .Must(x => x.DebitAmount > 0 || x.CreditAmount > 0)
            .WithMessage("Either debit or credit amount must be greater than 0");

        RuleFor(x => x)
            .Must(x => x.DebitAmount == 0 || x.CreditAmount == 0)
            .WithMessage("A line cannot have both debit and credit amounts");
    }
}

/// <summary>
/// Command to post a journal entry
/// </summary>
public record PostJournalEntryCommand(
    Guid JournalEntryId
) : IRequest<JournalEntryDto>;

/// <summary>
/// Handler for posting journal entries
/// </summary>
public class PostJournalEntryCommandHandler : IRequestHandler<PostJournalEntryCommand, JournalEntryDto>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IChartOfAccountsRepository _chartOfAccountsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<PostJournalEntryCommandHandler> _logger;

    public PostJournalEntryCommandHandler(
        IJournalEntryRepository journalEntryRepository,
        IChartOfAccountsRepository chartOfAccountsRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<PostJournalEntryCommandHandler> logger)
    {
        _journalEntryRepository = journalEntryRepository;
        _chartOfAccountsRepository = chartOfAccountsRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<JournalEntryDto> Handle(PostJournalEntryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Posting journal entry: {JournalEntryId}", request.JournalEntryId);

        var journalEntry = await _journalEntryRepository.GetByIdAsync(request.JournalEntryId, cancellationToken);
        if (journalEntry == null)
        {
            throw new InvalidOperationException($"Journal entry with ID {request.JournalEntryId} not found");
        }

        if (journalEntry.Status != JournalEntryStatus.Draft)
        {
            throw new InvalidOperationException($"Cannot post journal entry {request.JournalEntryId} in status {journalEntry.Status}");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Validate all accounts are still active
        var accountIds = journalEntry.Lines.Select(l => l.AccountId).Distinct();
        foreach (var accountId in accountIds)
        {
            var account = await _chartOfAccountsRepository.GetByIdAsync(accountId, cancellationToken);
            if (account == null || !account.IsActive)
            {
                throw new InvalidOperationException($"Account with ID {accountId} is not found or inactive");
            }
        }

        // Post the journal entry
        journalEntry.Post(currentUserId);

        // Save changes
        await _journalEntryRepository.UpdateAsync(journalEntry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(journalEntry.DomainEvents, cancellationToken);
        journalEntry.ClearDomainEvents();

        _logger.LogInformation("Successfully posted journal entry {JournalEntryId}", request.JournalEntryId);

        // Get accounts for DTO mapping
        var accounts = new Dictionary<Guid, ChartOfAccounts>();
        foreach (var accountId in accountIds)
        {
            var account = await _chartOfAccountsRepository.GetByIdAsync(accountId, cancellationToken);
            if (account != null)
            {
                accounts[accountId] = account;
            }
        }

        return MapToDto(journalEntry, accounts);
    }

    private static JournalEntryDto MapToDto(JournalEntry journalEntry, Dictionary<Guid, ChartOfAccounts> accounts)
    {
        return new JournalEntryDto
        {
            Id = journalEntry.Id,
            TenantId = journalEntry.TenantId,
            EntryNumber = journalEntry.EntryNumber,
            EntryDate = journalEntry.EntryDate,
            Reference = journalEntry.Reference,
            Description = journalEntry.Description,
            Status = journalEntry.Status.ToString(),
            TotalDebitAmount = journalEntry.TotalDebitAmount.Amount,
            TotalCreditAmount = journalEntry.TotalCreditAmount.Amount,
            Lines = journalEntry.Lines.Select(MapLineToDto).ToList(),
            Notes = journalEntry.Notes,
            PostedAt = journalEntry.PostedAt,
            PostedBy = journalEntry.PostedBy,
            CreatedAt = DateOnly.FromDateTime(journalEntry.CreatedAt),
            CreatedBy = journalEntry.CreatedBy,
            LastModified = journalEntry.LastModified,
            LastModifiedBy = journalEntry.LastModifiedBy
        };
    }

    private static JournalEntryLineDto MapLineToDto(JournalEntryLine line)
    {
        return new JournalEntryLineDto
        {
            Id = line.Id,
            AccountId = line.AccountId,
            AccountCode = line.AccountCode,
            AccountName = line.AccountName,
            Description = line.Description,
            DebitAmount = line.DebitAmount?.Amount ?? 0,
            CreditAmount = line.CreditAmount?.Amount ?? 0,
            Reference = line.Reference
        };
    }
}

/// <summary>
/// Validator for PostJournalEntryCommand
/// </summary>
public class PostJournalEntryCommandValidator : AbstractValidator<PostJournalEntryCommand>
{
    public PostJournalEntryCommandValidator()
    {
        RuleFor(x => x.JournalEntryId)
            .NotEmpty()
            .WithMessage("Journal entry ID is required");
    }
}

/// <summary>
/// Command to reverse a journal entry
/// </summary>
public record ReverseJournalEntryCommand(
    Guid JournalEntryId,
    DateOnly ReversalDate,
    string Reason
) : IRequest<JournalEntryDto>;

/// <summary>
/// Handler for reversing journal entries
/// </summary>
public class ReverseJournalEntryCommandHandler : IRequestHandler<ReverseJournalEntryCommand, JournalEntryDto>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IChartOfAccountsRepository _chartOfAccountsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ReverseJournalEntryCommandHandler> _logger;

    public ReverseJournalEntryCommandHandler(
        IJournalEntryRepository journalEntryRepository,
        IChartOfAccountsRepository chartOfAccountsRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService,
        ILogger<ReverseJournalEntryCommandHandler> logger)
    {
        _journalEntryRepository = journalEntryRepository;
        _chartOfAccountsRepository = chartOfAccountsRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<JournalEntryDto> Handle(ReverseJournalEntryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Reversing journal entry: {JournalEntryId}", request.JournalEntryId);

        var originalEntry = await _journalEntryRepository.GetByIdAsync(request.JournalEntryId, cancellationToken);
        if (originalEntry == null)
        {
            throw new InvalidOperationException($"Journal entry with ID {request.JournalEntryId} not found");
        }

        if (originalEntry.Status != JournalEntryStatus.Posted)
        {
            throw new InvalidOperationException($"Can only reverse posted journal entries. Current status: {originalEntry.Status}");
        }

        var currentUserId = _currentUserService.UserId ?? "system";

        // Create reversal lines (switch debits and credits)
        var reversalLines = new List<JournalEntryLine>();
        foreach (var originalLine in originalEntry.Lines)
        {
            var reversalLine = JournalEntryLine.Create(
                accountId: originalLine.AccountId,
                accountCode: originalLine.AccountCode,
                accountName: originalLine.AccountName,
                description: $"Reversal: {originalLine.Description}",
                debitAmount: originalLine.CreditAmount?.Amount ?? 0,
                creditAmount: originalLine.DebitAmount?.Amount ?? 0,
                reference: originalLine.Reference);
            
            reversalLines.Add(reversalLine);
        }

        // Create reversal journal entry
        var reversalEntry = JournalEntry.Create(
            tenantId: originalEntry.TenantId,
            entryDate: request.ReversalDate,
            reference: $"REV-{originalEntry.Reference}",
            description: $"Reversal of {originalEntry.Reference}: {request.Reason}",
            lines: reversalLines,
            notes: $"Reversal of journal entry {originalEntry.EntryNumber}. Reason: {request.Reason}",
            createdBy: currentUserId);

        // Post the reversal entry immediately
        reversalEntry.Post(currentUserId);

        // Mark original entry as reversed
        originalEntry.Reverse(reversalEntry.Id, request.Reason, currentUserId);

        // Save both entries
        await _journalEntryRepository.AddAsync(reversalEntry, cancellationToken);
        await _journalEntryRepository.UpdateAsync(originalEntry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(reversalEntry.DomainEvents, cancellationToken);
        await _domainEventService.PublishAsync(originalEntry.DomainEvents, cancellationToken);
        reversalEntry.ClearDomainEvents();
        originalEntry.ClearDomainEvents();

        _logger.LogInformation("Successfully created reversal entry {ReversalJournalId} for original entry {OriginalEntryId}", 
            reversalEntry.Id, request.JournalEntryId);

        // Get accounts for DTO mapping
        var accountIds = reversalEntry.Lines.Select(l => l.AccountId).Distinct();
        var accounts = new Dictionary<Guid, ChartOfAccounts>();
        foreach (var accountId in accountIds)
        {
            var account = await _chartOfAccountsRepository.GetByIdAsync(accountId, cancellationToken);
            if (account != null)
            {
                accounts[accountId] = account;
            }
        }

        return MapToDto(reversalEntry, accounts);
    }

    private static JournalEntryDto MapToDto(JournalEntry journalEntry, Dictionary<Guid, ChartOfAccounts> accounts)
    {
        return new JournalEntryDto
        {
            Id = journalEntry.Id,
            TenantId = journalEntry.TenantId,
            EntryNumber = journalEntry.EntryNumber,
            EntryDate = journalEntry.EntryDate,
            Reference = journalEntry.Reference,
            Description = journalEntry.Description,
            Status = journalEntry.Status.ToString(),
            TotalDebitAmount = journalEntry.TotalDebitAmount.Amount,
            TotalCreditAmount = journalEntry.TotalCreditAmount.Amount,
            Lines = journalEntry.Lines.Select(MapLineToDto).ToList(),
            Notes = journalEntry.Notes,
            PostedAt = journalEntry.PostedAt,
            PostedBy = journalEntry.PostedBy,
            ReversalJournalId = journalEntry.ReversalJournalId,
            ReversalReason = journalEntry.ReversalReason,
            CreatedAt = DateOnly.FromDateTime(journalEntry.CreatedAt),
            CreatedBy = journalEntry.CreatedBy,
            LastModified = journalEntry.LastModified,
            LastModifiedBy = journalEntry.LastModifiedBy
        };
    }

    private static JournalEntryLineDto MapLineToDto(JournalEntryLine line)
    {
        return new JournalEntryLineDto
        {
            Id = line.Id,
            AccountId = line.AccountId,
            AccountCode = line.AccountCode,
            AccountName = line.AccountName,
            Description = line.Description,
            DebitAmount = line.DebitAmount?.Amount ?? 0,
            CreditAmount = line.CreditAmount?.Amount ?? 0,
            Reference = line.Reference
        };
    }
}

/// <summary>
/// Validator for ReverseJournalEntryCommand
/// </summary>
public class ReverseJournalEntryCommandValidator : AbstractValidator<ReverseJournalEntryCommand>
{
    public ReverseJournalEntryCommandValidator()
    {
        RuleFor(x => x.JournalEntryId)
            .NotEmpty()
            .WithMessage("Journal entry ID is required");

        RuleFor(x => x.ReversalDate)
            .NotEmpty()
            .WithMessage("Reversal date is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reversal reason is required")
            .MaximumLength(500)
            .WithMessage("Reversal reason cannot exceed 500 characters");
    }
}
