using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.DTOs;
using TossErp.Accounting.Application.Common.Interfaces;

namespace TossErp.Accounting.Application.Queries.GetCashbookEntries;

/// <summary>
/// Handler for GetCashbookEntriesQuery
/// </summary>
public class GetCashbookEntriesQueryHandler : IRequestHandler<GetCashbookEntriesQuery, CashbookEntriesResponse>
{
    private readonly ICashbookEntryRepository _entryRepository;
    private readonly ILogger<GetCashbookEntriesQueryHandler> _logger;

    public GetCashbookEntriesQueryHandler(
        ICashbookEntryRepository entryRepository,
        ILogger<GetCashbookEntriesQueryHandler> logger)
    {
        _entryRepository = entryRepository;
        _logger = logger;
    }

    public async Task<CashbookEntriesResponse> Handle(GetCashbookEntriesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting cashbook entries with filters: From={FromDate}, To={ToDate}, Category={Category}, Type={Type}, Page={Page}, PageSize={PageSize}", 
            request.FromDate, request.ToDate, request.Category, request.Type, request.Page, request.PageSize);

        // For MVP, we'll use a hardcoded tenant ID
        // In a real implementation, this would come from the current user context
        var tenantId = "tenant-001";

        // Get all entries for the tenant (in a real implementation, this would use proper filtering)
        var allEntries = await _entryRepository.GetByTenantAsync(tenantId, cancellationToken);

        // Apply filters
        var filteredEntries = allEntries.AsEnumerable();

        if (request.FromDate.HasValue)
        {
            filteredEntries = filteredEntries.Where(e => e.TransactionDate.Date >= request.FromDate.Value.Date);
        }

        if (request.ToDate.HasValue)
        {
            filteredEntries = filteredEntries.Where(e => e.TransactionDate.Date <= request.ToDate.Value.Date);
        }

        if (!string.IsNullOrEmpty(request.Category))
        {
            filteredEntries = filteredEntries.Where(e => e.Category.ToString() == request.Category);
        }

        if (!string.IsNullOrEmpty(request.Type))
        {
            filteredEntries = filteredEntries.Where(e => e.Type.ToString() == request.Type);
        }

        if (request.MinAmount.HasValue)
        {
            filteredEntries = filteredEntries.Where(e => e.Amount.Amount >= request.MinAmount.Value);
        }

        if (request.MaxAmount.HasValue)
        {
            filteredEntries = filteredEntries.Where(e => e.Amount.Amount <= request.MaxAmount.Value);
        }

        if (!string.IsNullOrEmpty(request.Reference))
        {
            filteredEntries = filteredEntries.Where(e => e.Reference?.Contains(request.Reference, StringComparison.OrdinalIgnoreCase) == true);
        }

        if (!string.IsNullOrEmpty(request.Description))
        {
            filteredEntries = filteredEntries.Where(e => e.Description?.Contains(request.Description, StringComparison.OrdinalIgnoreCase) == true);
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            filteredEntries = request.SortBy.ToLower() switch
            {
                "date" => request.SortDirection?.ToLower() == "desc" 
                    ? filteredEntries.OrderByDescending(e => e.TransactionDate)
                    : filteredEntries.OrderBy(e => e.TransactionDate),
                "amount" => request.SortDirection?.ToLower() == "desc"
                    ? filteredEntries.OrderByDescending(e => e.Amount.Amount)
                    : filteredEntries.OrderBy(e => e.Amount.Amount),
                "category" => request.SortDirection?.ToLower() == "desc"
                    ? filteredEntries.OrderByDescending(e => e.Category)
                    : filteredEntries.OrderBy(e => e.Category),
                "type" => request.SortDirection?.ToLower() == "desc"
                    ? filteredEntries.OrderByDescending(e => e.Type)
                    : filteredEntries.OrderBy(e => e.Type),
                _ => filteredEntries.OrderByDescending(e => e.TransactionDate)
            };
        }
        else
        {
            // Default sorting by date descending
            filteredEntries = filteredEntries.OrderByDescending(e => e.TransactionDate);
        }

        var totalCount = filteredEntries.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
        var hasNextPage = request.Page < totalPages;
        var hasPreviousPage = request.Page > 1;

        // Apply pagination
        var pagedEntries = filteredEntries
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

                       // Convert to DTOs
               var entryDtos = pagedEntries.Select(e => new CashbookEntryDto
               {
                   Id = e.Id,
                   CashbookId = Guid.Empty, // For MVP, we'll use a default cashbook ID
                   Amount = e.Amount.Amount,
                   Type = e.Type.ToString(),
                   Category = e.Category.ToString(),
                   Description = e.Description,
                   Reference = e.Reference,
                   TransactionDate = e.TransactionDate,
                   CreatedAt = e.CreatedAt,
                   CreatedBy = e.CreatedBy,
                   UpdatedAt = e.UpdatedAt,
                   UpdatedBy = e.UpdatedBy
               }).ToList();

        var response = new CashbookEntriesResponse
        {
            Entries = entryDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            HasNextPage = hasNextPage,
            HasPreviousPage = hasPreviousPage
        };

        _logger.LogInformation("Retrieved {Count} cashbook entries out of {TotalCount} total", 
            entryDtos.Count, totalCount);

        return response;
    }
}
