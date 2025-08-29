namespace TossErp.Accounts.Application.Queries;

/// <summary>
/// Query to get journal entries with filtering and pagination
/// </summary>
public record GetJournalEntriesQuery(
    JournalEntryStatus? Status = null,
    string? Reference = null,
    DateOnly? EntryDateFrom = null,
    DateOnly? EntryDateTo = null,
    Guid? AccountId = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null,
    int PageNumber = 1,
    int PageSize = 10,
    string SortBy = "EntryDate",
    bool SortDescending = true
) : IRequest<PaginatedResult<JournalEntryDto>>;

/// <summary>
/// Handler for getting journal entries
/// </summary>
public class GetJournalEntriesQueryHandler : IRequestHandler<GetJournalEntriesQuery, PaginatedResult<JournalEntryDto>>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly ILogger<GetJournalEntriesQueryHandler> _logger;

    public GetJournalEntriesQueryHandler(
        IJournalEntryRepository journalEntryRepository,
        ILogger<GetJournalEntriesQueryHandler> logger)
    {
        _journalEntryRepository = journalEntryRepository;
        _logger = logger;
    }

    public async Task<PaginatedResult<JournalEntryDto>> Handle(GetJournalEntriesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting journal entries with status: {Status}, Reference: {Reference}", 
            request.Status, request.Reference);

        var filter = new JournalEntryFilter
        {
            Status = request.Status,
            Reference = request.Reference,
            EntryDateFrom = request.EntryDateFrom,
            EntryDateTo = request.EntryDateTo,
            // AccountId = request.AccountId, // Not available in filter
            MinAmount = request.MinAmount,
            MaxAmount = request.MaxAmount
        };

        var (entriesList, totalCount) = await _journalEntryRepository.GetPagedAsync(
            filter,
            request.PageNumber,
            request.PageSize,
            request.SortBy,
            request.SortDescending,
            cancellationToken);

        var journalEntryDtos = entriesList.Select(MapToDto).ToList();

        return new PaginatedResult<JournalEntryDto>
        {
            Items = journalEntryDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    private static JournalEntryDto MapToDto(JournalEntry journalEntry)
    {
        return new JournalEntryDto
        {
            Id = journalEntry.Id,
            TenantId = journalEntry.TenantId,
            EntryNumber = journalEntry.EntryNumber,
            EntryDate = DateOnly.FromDateTime(journalEntry.EntryDate),
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
            CreatedAt = journalEntry.CreatedAt,
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
/// Query to get a journal entry by ID
/// </summary>
public record GetJournalEntryByIdQuery(Guid JournalEntryId) : IRequest<JournalEntryDto?>;

/// <summary>
/// Handler for getting a journal entry by ID
/// </summary>
public class GetJournalEntryByIdQueryHandler : IRequestHandler<GetJournalEntryByIdQuery, JournalEntryDto?>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly ILogger<GetJournalEntryByIdQueryHandler> _logger;

    public GetJournalEntryByIdQueryHandler(
        IJournalEntryRepository journalEntryRepository,
        ILogger<GetJournalEntryByIdQueryHandler> logger)
    {
        _journalEntryRepository = journalEntryRepository;
        _logger = logger;
    }

    public async Task<JournalEntryDto?> Handle(GetJournalEntryByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting journal entry by ID: {JournalEntryId}", request.JournalEntryId);

        var journalEntry = await _journalEntryRepository.GetByIdAsync(request.JournalEntryId, cancellationToken);
        if (journalEntry == null)
        {
            return null;
        }

        return MapToDto(journalEntry);
    }

    private static JournalEntryDto MapToDto(JournalEntry journalEntry)
    {
        return new JournalEntryDto
        {
            Id = journalEntry.Id,
            TenantId = journalEntry.TenantId,
            EntryNumber = journalEntry.EntryNumber,
            EntryDate = DateOnly.FromDateTime(journalEntry.EntryDate),
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
            CreatedAt = journalEntry.CreatedAt,
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

